namespace hariloom.Models.DTOs
{
    public class AdminDashboardViewModel
    {
        public int ActiveProducts { get; set; }
        public int PendingOrders { get; set; }
        public int InTransitOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalUsers { get; set; }
        public List<DashboardRecentOrderDTO> RecentOrders { get; set; } = new List<DashboardRecentOrderDTO>();
        public List<ProductSalesReportDTO> ProductSales { get; set; } = new List<ProductSalesReportDTO>();
    }

    public class ProductSalesReportDTO
    {
        public int ProductId { get; set; }
        public string? ProductDisplayId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public int QuantityAvailable { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardRecentOrderDTO
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Date { get; set; }
        public int Status { get; set; }
        public decimal Total { get; set; }
    }
}
