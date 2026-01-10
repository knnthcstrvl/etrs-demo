namespace Financial.DTO.Report
{
    public class AccountingAgingReportDto
    {
        public int TransactionId { get; set; }
        public string DvNumber { get; set; }
        public string Payee { get; set; }

        public DateTime? DvDateReceived { get; set; }
        public decimal? NetAmount { get; set; }

        public int AgingDays { get; set; }
        public string Status { get; set; }
    }
}
