using Microsoft.EntityFrameworkCore;
using hariloom.Helpers.DbContexts;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Entity;
using hariloom.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hariloom.Repository
{
    public class OrderManagementRepository : IOrderManagementRepository
    {
        #region Interface and Repository Implementations       
        private readonly appDBContext _context;
        private readonly IApiResponseRepository _apiResponseRepository;
        public OrderManagementRepository(appDBContext context, IApiResponseRepository apiResponseRepository)
        {
            _context = context;
            _apiResponseRepository = apiResponseRepository;
        }
        #endregion

        public async Task<List<OrderDashboardDTO>> GetOrdersByStatusAsync(int statusId)
        {
            var orderDetails = await _context.trnOrder.Include(a => a.orderItems).ThenInclude(oi => oi.product).Include(a => a.user)
                .Where(o => o.orderStatus == statusId).OrderByDescending(o => o.createdDate).ToListAsync();
            var userIds = orderDetails.Select(o => o.mstUserId).Distinct().ToList();
            var userAddresses = await _context.trnUserAddress.Where(a => userIds.Contains(a.mstUserId) && a.label == "Delivery").ToListAsync();

            var result = orderDetails.Select(o =>
            {
                var address = userAddresses.FirstOrDefault(a => a.mstUserId == o.mstUserId);
                string addressStr = address != null ? $"{address.addressLine1}{(string.IsNullOrEmpty(address.addressLine2) ? "" : ", " + address.addressLine2)}, {address.city}, {address.state} - {address.zipCode}" : (o.user?.address ?? "N/A");

                return new OrderDashboardDTO
                {
                    orderId = o.trnOrderId,
                    trackingNumber = o.orderNumber ?? "",
                    customerName = o.user?.name ?? "",
                    customerPhone = o.user?.phoneNumber ?? "",
                    customerEmail = o.user?.email ?? "",
                    customerAddress = addressStr,
                    adminNotes = o.adminNotes ?? "",
                    totalAmount = o.totalAmount,
                    statusId = o.orderStatus,
                    items = o.orderItems.Select(oi => new DashboardOrderItemDTO
                    {
                        productName = oi.product?.productName ?? "Unknown Product",
                        size = oi.size ?? "",
                        quantity = oi.quantity
                    }).ToList()
                };
            }).ToList();

            return result;
        }

        public async Task<List<OrderDashboardDTO>> GetAllOrdersAdminAsync()
        {
            var orderDetails = await _context.trnOrder.Include(a => a.orderItems).ThenInclude(oi => oi.product).Include(a => a.user)
                .OrderByDescending(o => o.createdDate).ToListAsync();
            var userIds = orderDetails.Select(o => o.mstUserId).Distinct().ToList();
            var userAddresses = await _context.trnUserAddress.Where(a => userIds.Contains(a.mstUserId) && a.label == "Delivery").ToListAsync();

            var result = orderDetails.Select(o =>
            {
                var address = userAddresses.FirstOrDefault(a => a.mstUserId == o.mstUserId);
                string addressStr = address != null ? $"{address.addressLine1}{(string.IsNullOrEmpty(address.addressLine2) ? "" : ", " + address.addressLine2)}, {address.city}, {address.state} - {address.zipCode}" : (o.user?.address ?? "N/A");

                return new OrderDashboardDTO
                {
                    orderId = o.trnOrderId,
                    trackingNumber = o.orderNumber ?? "",
                    customerName = o.user?.name ?? "",
                    customerPhone = o.user?.phoneNumber ?? "",
                    customerEmail = o.user?.email ?? "",
                    customerAddress = addressStr,
                    adminNotes = o.adminNotes ?? "",
                    totalAmount = o.totalAmount,
                    statusId = o.orderStatus,
                    items = o.orderItems.Select(oi => new DashboardOrderItemDTO
                    {
                        productName = oi.product?.productName ?? "Unknown Product",
                        size = oi.size ?? "",
                        quantity = oi.quantity
                    }).ToList()
                };
            }).ToList();

            return result;
        }

        public async Task<trnOrder> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.trnOrder.Include(a => a.orderItems).FirstOrDefaultAsync(o => o.trnOrderId == orderId);
            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int statusId)
        {
            var order = await _context.trnOrder.FindAsync(orderId);
            if (order == null) return false;

            order.orderStatus = statusId;
            _context.trnOrder.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderNotesAsync(int orderId, string notes)
        {
            var order = await _context.trnOrder.FindAsync(orderId);
            if (order == null) return false;

            order.adminNotes = notes;
            _context.trnOrder.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
