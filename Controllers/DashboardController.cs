using Microsoft.EntityFrameworkCore;
using hariloom.Helpers.DbContexts;
using hariloom.Models.DTOs;
using hariloom.Models.Enums;
using nova_attire.Helpers.Middlewares;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace hariloom.Controllers
{
    public class DashboardController : Controller
    {
        private readonly appDBContext _context;

        public DashboardController(appDBContext context)
        {
            _context = context;
        }

        [RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        public async Task<IActionResult> AdminDashboard()
        {
            var vm = new AdminDashboardViewModel();
            vm.ActiveProducts = await _context.mstProduct.CountAsync(p => p.isActive);
            vm.PendingOrders = await _context.trnOrder.CountAsync(o => o.orderStatus == 1);
            vm.InTransitOrders = await _context.trnOrder.CountAsync(o => o.orderStatus == 2);
            vm.CompletedOrders = await _context.trnOrder.CountAsync(o => o.orderStatus == 3);
            vm.CancelledOrders = await _context.trnOrder.CountAsync(o => o.orderStatus == 4);
            vm.TotalUsers = await _context.mstUser.CountAsync(u => u.accessLevel == 1);

            vm.RecentOrders = await _context.trnOrder.Include(o => o.user)
                .Where(o => o.orderStatus != 0 && o.isActive)
                .OrderByDescending(o => o.createdDate)
                .Take(5)
                .Select(o => new DashboardRecentOrderDTO
                {
                    OrderId = o.orderNumber ?? "",
                    CustomerName = o.user != null ? o.user.name : "Unknown",
                    Date = o.createdDate.ToString("yyyy-MM-dd"),
                    Status = o.orderStatus,
                    Total = o.totalAmount
                }).ToListAsync();

            var paidOrderStatuses = new[] { 1, 2, 3 }; // Paid, InTransit, Completed
            
            var products = await _context.mstProduct
                .Where(p => p.isActive)
                .ToListAsync();

            var productSizes = await _context.Set<hariloom.Models.Entity.trnProductSize>()
                .Where(s => s.isActive)
                .ToListAsync();

            var orderItems = await _context.trnOrderItem
                .Include(oi => oi.order)
                .Where(oi => oi.isActive && oi.order != null && paidOrderStatuses.Contains(oi.order.orderStatus))
                .ToListAsync();

            var productSalesList = new List<ProductSalesReportDTO>();
            
            foreach (var p in products)
            {
                var sizesForProduct = productSizes.Where(s => s.mstProductId == p.mstProductId).ToList();
                
                if (sizesForProduct.Any())
                {
                    foreach (var s in sizesForProduct)
                    {
                        productSalesList.Add(new ProductSalesReportDTO
                        {
                            ProductId = p.mstProductId,
                            ProductDisplayId = p.productDisplayId,
                            ProductName = p.productName ?? "Unknown",
                            Size = string.IsNullOrEmpty(s.size) ? "N/A" : s.size,
                            QuantityAvailable = s.quantityAvailable,
                            Revenue = orderItems
                                .Where(oi => oi.mstProductId == p.mstProductId && oi.size == s.size)
                                .Sum(oi => (decimal)(oi.price * oi.quantity))
                        });
                    }
                }
                else
                {
                    productSalesList.Add(new ProductSalesReportDTO
                    {
                        ProductId = p.mstProductId,
                        ProductDisplayId = p.productDisplayId,
                        ProductName = p.productName ?? "Unknown",
                        Size = "N/A",
                        QuantityAvailable = p.quantityAvailable,
                        Revenue = orderItems
                            .Where(oi => oi.mstProductId == p.mstProductId)
                            .Sum(oi => (decimal)(oi.price * oi.quantity))
                    });
                }
            }

            vm.ProductSales = productSalesList.OrderByDescending(x => x.Revenue).ToList();

            return View(vm);
        }

        [RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        public IActionResult UserDatabase()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllUserAccounts([FromServices] hariloom.Interfaces.IMstUserRepository userRepository)
        {
            return await userRepository.getAllUserAccountsAsync();
        }
    }
}
