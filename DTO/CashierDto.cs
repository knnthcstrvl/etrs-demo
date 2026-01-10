namespace Financial.DTO
{
    public class CashierDto
    {
        public int TransactionId { get; set; }

        public string LddapNo { get; set; }
        public DateTime? LddapDate { get; set; }

        public string Payee { get; set; }
        public decimal? Amount { get; set; }
        public decimal? FinalBalance { get; set; }
    }
}
