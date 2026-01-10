namespace Financial.DTO
{
    public class ProcurementCreateUpdateDto
    {
        public int TransactionId { get; set; }

        public string PpmpNumber { get; set; }
        public string DetailedDescription { get; set; }
        public decimal? AbcTotal { get; set; }

        public DateTime? PrDateReceived { get; set; }
        public string EndUser { get; set; }

        public string ProcurementCategory { get; set; }
        public int? TypeId { get; set; }
        public int? ModeId { get; set; }

        public decimal? ContractAmount { get; set; }
        public DateTime? DateConforme { get; set; }
        public string DeliveryTerms { get; set; }

        public DateTime? LastDayDeliveryOrEventDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
