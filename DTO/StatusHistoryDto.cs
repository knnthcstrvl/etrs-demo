namespace Financial.DTO
{
    public class StatusHistoryDto
    {
        public int TransactionId { get; set; }
        public string Module { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string Remarks { get; set; }
    }
}
