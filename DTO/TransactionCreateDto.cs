namespace Financial.DTO
{
    public class TransactionCreateDto
    {
        public string PrNumber { get; set; }
        public string ContractNumber { get; set; }
        public string OrsNumber { get; set; }
        public string DvNumber { get; set; }

        public int? SupplierId { get; set; }
    }
}
