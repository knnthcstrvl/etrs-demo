using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Modules
{
    public class Accounting
    {
        [Key]
        public int AccountingId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public string CafNumber { get; set; }
        public DateTime? CafDateReceived { get; set; }
        public DateTime? CafDateReleased { get; set; }

        public DateTime? PoDateReceived { get; set; }
        public DateTime? PoDateReleased { get; set; }

        public DateTime? DvDateReceived { get; set; }
        public DateTime? DvDateReleased { get; set; }

        public string LddapNo { get; set; }
        public DateTime? LddapDate { get; set; }

        public decimal? GrossAmount { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Retention { get; set; }
        public decimal? OtherDeductions { get; set; }

        public decimal? NetAmount { get; set; }
        public string Status { get; set; }
    }
}
