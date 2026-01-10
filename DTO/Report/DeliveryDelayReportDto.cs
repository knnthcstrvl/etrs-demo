namespace Financial.DTO.Report
{
    public class DeliveryDelayReportDto
    {
        public int TransactionId { get; set; }
        public string ContractNumber { get; set; }
        public string SupplierName { get; set; }

        public DateTime? DeliveryDueDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }

        public int DelayDays { get; set; }
        public string Status { get; set; }
    }
}
