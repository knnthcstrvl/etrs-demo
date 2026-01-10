using Financial.Model.Library;
using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Modules
{
    public class Procurement
    {
        [Key]
        public int ProcurementId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public string PpmpNumber { get; set; }
        public string DetailedDescription { get; set; }
        public decimal? AbcTotal { get; set; }

        public DateTime? PrDateReceived { get; set; }
        public string EndUser { get; set; }

        public string ProcurementCategory { get; set; }

        public int? TypeId { get; set; }
        public ProcurementType Type { get; set; }

        public int? ModeId { get; set; }
        public ProcurementMode Mode { get; set; }

        public decimal? ContractAmount { get; set; }
        public DateTime? DateConforme { get; set; }
        public string DeliveryTerms { get; set; }

        public DateTime? LastDayDeliveryOrEventDate { get; set; }

        public string Status { get; set; }
        public string Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
