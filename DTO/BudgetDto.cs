namespace Financial.DTO
{
    public class BudgetDto
    {
        public int TransactionId { get; set; }

        public string MfoPapFundSource { get; set; }
        public string Rc { get; set; }
        public string UacsCode { get; set; }

        public string FundCluster { get; set; }
        public decimal? AdjustedAmount { get; set; }
        public DateTime? DateObligated { get; set; }

        public string Status { get; set; }
    }
}
