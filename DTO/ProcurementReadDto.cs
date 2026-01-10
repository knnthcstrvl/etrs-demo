namespace Financial.DTO
{
    public class ProcurementReadDto
    {
        public int ProcurementId { get; set; }
        public string PrNumber { get; set; }
        public string EndUser { get; set; }
        public decimal? AbcTotal { get; set; }
        public decimal? ContractAmount { get; set; }
        public string Mode { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
