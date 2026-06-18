using Financial;
using Financial.Components;
using Financial.Data;
// Financial.Hubs removed
using Financial.Repositories.Implement;
using Financial.Repositories.Interface;
using Financial.Services.Implements;
using Financial.Services.Interfaces;
using Financial.State;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register EtrsDbContext (project-specific) instead of ProcurementDbContext
builder.Services.AddDbContext<EtrsDbContext>(options =>
    options.UseSqlServer(connectionString));
// Provide factory to create DbContext instances for long-lived services
builder.Services.AddDbContextFactory<EtrsDbContext>(
    options => options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddCors();
builder.Services.AddLocalization();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});
builder.Services.AddControllers();
builder.Services.AddSyncfusionBlazor();
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1ceXRdQ2VeUUJ3WEdWYEs=");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ETRS API",
        Version = "v1",
        Description = "Electronic Time Recording System (ETRS) API with sample attendance data",
        Contact = new OpenApiContact
        {
            Name = "ETRS Team"
        }
    });
});

// Procurement/transaction services removed
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IConstantState, ConstantState>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
// Current user helper
builder.Services.AddScoped<Financial.Services.CurrentUserService>();
builder.Services.AddScoped(sp => new HttpClient());
// Toast notifications (scoped so JS interop can be used per-circuit)
builder.Services.AddScoped<Financial.Services.IToastService, Financial.Services.ToastService>();
builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

MappingProfile.RegisterMappings();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETRS API v1");
    c.DisplayRequestDuration();
});

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "en-PH" })
    .AddSupportedUICultures(new[] { "en-PH" }));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// TransactionNotificationHub removed

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<EtrsDbContext>();
        await Financial.Data.DatabaseSchemaUpdater.EnsureUserAccountColumnsAsync(db);
        // Ensure EF migrations are applied
        await db.Database.MigrateAsync();

        // Run only critical seeders on startup. Other seeders are disabled to
        // avoid inserting demo/legacy data in production. Keep UserAccount seeding.
        await Financial.Data.Seeders.UserAccountSeeder.SeedAdminAsync(db);
        // EmployeeSeeder, ExceptionConditionSeeder, BiometricDeviceSeeder,
        // AttendanceLogSeeder and AttendanceLogWfhSeeder are intentionally
        // disabled. Run them manually if needed.
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while preparing database: " + ex);
    }
}

app.Run();
