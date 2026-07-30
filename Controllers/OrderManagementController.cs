using Microsoft.AspNetCore.Mvc;
using nova_attire.Helpers.Middlewares;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Enums;
using System.Threading.Tasks;

namespace hariloom.Controllers
{
    public class OrderManagementController : Controller
    {
        #region Interface Implementations
        private readonly IOrderManagementRepository _orderManagementRepository;
        private readonly IApiResponseRepository _apiResponseRepository;

        public OrderManagementController(IOrderManagementRepository repo, IApiResponseRepository apiResponseRepository)
        {
            _orderManagementRepository = repo;
            _apiResponseRepository = apiResponseRepository;
        }
        #endregion  
      
        [HttpGet]
        public async Task<IActionResult> GetOrdersByStatus(int statusId)
        {
            var orders = await _orderManagementRepository.GetOrdersByStatusAsync(statusId);
            return Json(_apiResponseRepository.SuccessResponse(new ApiResponseDTO() { data = orders }));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderManagementRepository.GetAllOrdersAdminAsync();
            return Json(_apiResponseRepository.SuccessResponse(new ApiResponseDTO() { data = orders }));
        }

        public async Task<IActionResult> OrderManagement()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _orderManagementRepository.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return PartialView("_OrderDetails", order);
        }

        [HttpPost]
        public async Task<ApiResponseDTO> UpdateStatus(int orderId, int statusId)
        {
            var ok = await _orderManagementRepository.UpdateOrderStatusAsync(orderId, statusId);
            if (ok) return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Status updated successfully" });

            return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Failed to update status" });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> UpdateNotes(int orderId, string notes)
        {
            var ok = await _orderManagementRepository.UpdateOrderNotesAsync(orderId, notes ?? "");
            if (ok) return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Notes updated successfully" });

            return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Failed to update notes" });
        }
    }
}
