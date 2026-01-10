using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Modules
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public DateTime? PoContractDateReceived { get; set; }
        public DateTime? PoContractDate { get; set; }

        public string IarNo { get; set; }
        public DateTime? IarDate { get; set; }

        public string DrNo { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string InvoiceNo { get; set; }
        public decimal? Amount { get; set; }

        public DateTime? InspectionDate { get; set; }
        public string InspectedBy { get; set; }

        public DateTime? AcceptanceDate { get; set; }
        public string AcceptedBy { get; set; }

        public string CoaControlNo { get; set; }
        public DateTime? NoadDate { get; set; }

        public DateTime? DateForwardedToPms { get; set; }
        public DateTime? LastDayDeliveryTerm { get; set; }

        public string Status { get; set; }
    }
}
