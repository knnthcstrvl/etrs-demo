namespace Financial.DTO
{
    public class PropertyDto
    {
        public int TransactionId { get; set; }

        public string IarNo { get; set; }
        public DateTime? IarDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public DateTime? InspectionDate { get; set; }
        public DateTime? AcceptanceDate { get; set; }

        public string Status { get; set; }
    }
}
