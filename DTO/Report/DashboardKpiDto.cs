namespace Financial.DTO.Report
{
    public class DashboardKpiDto
    {
        // Procurement
        public int TotalPrs { get; set; }
        public int PrsCompleted { get; set; }
        public int PrsPending { get; set; }
        public int PrsDelayed { get; set; }

        // Delivery
        public int TotalDeliveries { get; set; }
        public int DeliveriesOnTime { get; set; }
        public int DeliveriesDelayed { get; set; }

        // Accounting / Payment
        public int TotalDv { get; set; }
        public int DvPaid { get; set; }
        public int DvPending { get; set; }
        public decimal TotalNetAmount { get; set; }
        public decimal TotalOverdueAmount { get; set; }
    }
}
