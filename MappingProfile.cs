using Financial.DTO;
using Financial.Model;
using Financial.Model.Modules;
using Financial.Model.Trace;
using Mapster;

namespace Financial
{
    public class MappingProfile
    {
        public static void RegisterMappings()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            // ======================
            // TRANSACTION
            // ======================
            config.NewConfig<TransactionCreateDto, Transaction>();

            config.NewConfig<Transaction, TransactionReadDto>()
                .Map(dest => dest.SupplierName, src => src.Supplier != null ? src.Supplier.SupplierName : null);

            // ======================
            // PROCUREMENT
            // ======================
            config.NewConfig<ProcurementCreateUpdateDto, Procurement>();

            config.NewConfig<Procurement, ProcurementReadDto>()
                .Map(dest => dest.PrNumber, src => src.Transaction.PrNumber)
                .Map(dest => dest.Mode, src => src.Mode.ModeName)
                .Map(dest => dest.Type, src => src.Type.TypeName);

            // ======================
            // BUDGET
            // ======================
            config.NewConfig<BudgetDto, Budget>();

            config.NewConfig<Budget, BudgetDto>()
                .Map(dest => dest.FundCluster, src => src.FundCluster.FundClusterName);

            // ======================
            // PROPERTY
            // ======================
            config.NewConfig<PropertyDto, Property>();
            config.NewConfig<Property, PropertyDto>();

            // ======================
            // ACCOUNTING
            // ======================
            config.NewConfig<AccountingDto, Accounting>();
            config.NewConfig<Accounting, AccountingDto>();

            // ======================
            // CAHIER
            // ======================
            config.NewConfig<CashierDto, Cashier>();
            config.NewConfig<Cashier, CashierDto>();

            // ======================
            // DOCUMENT & STATUS
            // ======================
            config.NewConfig<DocumentDto, Document>();
            config.NewConfig<StatusHistoryDto, StatusHistory>();
        }
    }

}
