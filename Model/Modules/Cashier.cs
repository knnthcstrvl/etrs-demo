using Financial.Model.Library;
using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Modules
{
    public class Cashier
    {
        [Key]
        public int CashierId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public string LddapAdaOrCheckNo { get; set; }
        public DateTime? LddapOrCheckDate { get; set; }

        public int? FundClusterId { get; set; }
        public FundCluster FundCluster { get; set; }

        public int? PaymentModeId { get; set; }
        public PaymentMode PaymentMode { get; set; }

        public string Payee { get; set; }
        public string UacsObjectCode { get; set; }

        public string Particulars { get; set; }
        public decimal? Amount { get; set; }
        public decimal? FinalBalance { get; set; }

        public string Remarks { get; set; }
    }
}
