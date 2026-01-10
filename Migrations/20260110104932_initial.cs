using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundClusters",
                columns: table => new
                {
                    FundClusterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundClusterName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundClusters", x => x.FundClusterId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentModes",
                columns: table => new
                {
                    PaymentModeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentModeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModes", x => x.PaymentModeId);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementModes",
                columns: table => new
                {
                    ModeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementModes", x => x.ModeId);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementTypes", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrsNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DvNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accountings",
                columns: table => new
                {
                    AccountingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CafNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CafDateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CafDateReleased = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PoDateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PoDateReleased = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DvDateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DvDateReleased = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LddapNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LddapDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Retention = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accountings", x => x.AccountingId);
                    table.ForeignKey(
                        name: "FK_Accountings_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    MfoPapFundSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UacsCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectOfExpenditure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundClusterId = table.Column<int>(type: "int", nullable: true),
                    AllotmentClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BudgetSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EarmarkCafCaaNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateEarmarked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateObligated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdjustedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateReleased = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedToOffice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.BudgetId);
                    table.ForeignKey(
                        name: "FK_Budgets_FundClusters_FundClusterId",
                        column: x => x.FundClusterId,
                        principalTable: "FundClusters",
                        principalColumn: "FundClusterId");
                    table.ForeignKey(
                        name: "FK_Budgets_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cashiers",
                columns: table => new
                {
                    CashierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    LddapAdaOrCheckNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LddapOrCheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FundClusterId = table.Column<int>(type: "int", nullable: true),
                    PaymentModeId = table.Column<int>(type: "int", nullable: true),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UacsObjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Particulars = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cashiers", x => x.CashierId);
                    table.ForeignKey(
                        name: "FK_Cashiers_FundClusters_FundClusterId",
                        column: x => x.FundClusterId,
                        principalTable: "FundClusters",
                        principalColumn: "FundClusterId");
                    table.ForeignKey(
                        name: "FK_Cashiers_PaymentModes_PaymentModeId",
                        column: x => x.PaymentModeId,
                        principalTable: "PaymentModes",
                        principalColumn: "PaymentModeId");
                    table.ForeignKey(
                        name: "FK_Cashiers_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Documents_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procurements",
                columns: table => new
                {
                    ProcurementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    PpmpNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailedDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AbcTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PrDateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcurementCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    ModeId = table.Column<int>(type: "int", nullable: true),
                    ContractAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateConforme = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryTerms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastDayDeliveryOrEventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procurements", x => x.ProcurementId);
                    table.ForeignKey(
                        name: "FK_Procurements_ProcurementModes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "ProcurementModes",
                        principalColumn: "ModeId");
                    table.ForeignKey(
                        name: "FK_Procurements_ProcurementTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProcurementTypes",
                        principalColumn: "TypeId");
                    table.ForeignKey(
                        name: "FK_Procurements_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    PoContractDateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PoContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IarNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IarDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DrNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcceptanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoaControlNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateForwardedToPms = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDayDeliveryTerm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_Properties_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatusHistories",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusHistories", x => x.StatusId);
                    table.ForeignKey(
                        name: "FK_StatusHistories_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accountings_TransactionId",
                table: "Accountings",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_FundClusterId",
                table: "Budgets",
                column: "FundClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_TransactionId",
                table: "Budgets",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cashiers_FundClusterId",
                table: "Cashiers",
                column: "FundClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashiers_PaymentModeId",
                table: "Cashiers",
                column: "PaymentModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cashiers_TransactionId",
                table: "Cashiers",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TransactionId",
                table: "Documents",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FundClusters_FundClusterName",
                table: "FundClusters",
                column: "FundClusterName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentModes_PaymentModeName",
                table: "PaymentModes",
                column: "PaymentModeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementModes_ModeName",
                table: "ProcurementModes",
                column: "ModeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Procurements_ModeId",
                table: "Procurements",
                column: "ModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Procurements_TransactionId",
                table: "Procurements",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Procurements_TypeId",
                table: "Procurements",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementTypes_TypeName",
                table: "ProcurementTypes",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_TransactionId",
                table: "Properties",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusHistories_TransactionId",
                table: "StatusHistories",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierName",
                table: "Suppliers",
                column: "SupplierName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PrNumber",
                table: "Transactions",
                column: "PrNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SupplierId",
                table: "Transactions",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accountings");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Cashiers");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Procurements");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "StatusHistories");

            migrationBuilder.DropTable(
                name: "FundClusters");

            migrationBuilder.DropTable(
                name: "PaymentModes");

            migrationBuilder.DropTable(
                name: "ProcurementModes");

            migrationBuilder.DropTable(
                name: "ProcurementTypes");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
