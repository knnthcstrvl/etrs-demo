using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Trace
{
    public class StatusHistory
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public string Module { get; set; }
        public string Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public string Remarks { get; set; }
    }
}
