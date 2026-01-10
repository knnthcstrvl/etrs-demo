namespace Financial.DTO
{
    public class TransactionReadDto
    {
        public int TransactionId { get; set; }
        public string PrNumber { get; set; }
        public string ContractNumber { get; set; }
        public string OrsNumber { get; set; }
        public string DvNumber { get; set; }

        public string SupplierName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
