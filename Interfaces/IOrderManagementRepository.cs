using hariloom.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hariloom.Interfaces
{
    public interface IOrderManagementRepository
    {
        Task<List<OrderDashboardDTO>> GetOrdersByStatusAsync(int statusId);
        Task<List<OrderDashboardDTO>> GetAllOrdersAdminAsync();
        Task<Models.Entity.trnOrder> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, int statusId);
        Task<bool> UpdateOrderNotesAsync(int orderId, string notes);
    }
}
