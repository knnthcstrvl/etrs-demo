using Financial.Model.Library;
using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Modules
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public string MfoPapFundSource { get; set; }
        public string Rc { get; set; }
        public string UacsCode { get; set; }
        public string ObjectOfExpenditure { get; set; }

        public int? FundClusterId { get; set; }
        public FundCluster FundCluster { get; set; }

        public string AllotmentClass { get; set; }
        public string BudgetSource { get; set; }

        public string EarmarkCafCaaNumber { get; set; }
        public DateTime? DateEarmarked { get; set; }

        public DateTime? DateObligated { get; set; }
        public decimal? AdjustedAmount { get; set; }

        public DateTime? DateReleased { get; set; }
        public string ReleasedToOffice { get; set; }

        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
