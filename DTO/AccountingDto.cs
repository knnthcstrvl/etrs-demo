namespace Financial.DTO
{
    public class AccountingDto
    {
        public int TransactionId { get; set; }

        public string DvNumber { get; set; }
        public DateTime? DvDateReceived { get; set; }

        public decimal? GrossAmount { get; set; }
        public decimal? NetAmount { get; set; }

        public string Status { get; set; }
    }
}
