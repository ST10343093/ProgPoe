namespace ProgPoe.Models
{
    public class DashboardViewModel
    {
        public int TotalClaims { get; set; }
        public int PendingClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal PendingPayments { get; set; }
        public decimal CompletedPayments { get; set; }
    }
}

