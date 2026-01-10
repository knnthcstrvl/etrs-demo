namespace Financial.DTO.Report
{
    public class ProcurementAgingReportDto
    {
        public int TransactionId { get; set; }
        public string PrNumber { get; set; }
        public string EndUser { get; set; }
        public string Status { get; set; }

        public DateTime? PrDateReceived { get; set; }
        public DateTime? LastDayDelivery { get; set; }

        public int AgingDays { get; set; }
        public bool IsDelayed { get; set; }
    }
}
