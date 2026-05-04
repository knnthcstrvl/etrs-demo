using Financial;
using Financial.Components;
using Financial.Data;
using Financial.Hubs;
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

builder.Services.AddDbContext<ProcurementDbContext>(options =>
    options.UseSqlServer(connectionString));

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
builder.Services.AddSignalR();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddSyncfusionBlazor();
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1ceXRdQ2VeUUJ3WEdWYEs=");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DTR Monitoring API",
        Version = "v1",
        Description = "Daily time record monitoring with biometric attendance sample data",
        Contact = new OpenApiContact
        {
            Name = "DTR Monitoring Team"
        }
    });
});

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IProcurementService, ProcurementService>();
builder.Services.AddScoped<IStatusHistoryService, StatusHistoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IAccountingService, AccountingService>();
builder.Services.AddScoped<ICashierService, CashierService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISupplierServices, SupplierServices>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IConstantState, ConstantState>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DTR Monitoring API v1");
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

app.MapHub<TransactionNotificationHub>("/transactionHub");

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ProcurementDbContext>();
        db.Database.EnsureCreated();
        await DatabaseSchemaUpdater.EnsureAttendanceLogWfhColumnsAsync(db);
        await UserAccountSeeder.SeedAdminAsync(db);
        await EmployeeSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error while preparing database: " + ex.Message);
    }
}

app.Run();
