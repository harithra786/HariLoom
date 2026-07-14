using hariloom.Models.DTOs;

namespace hariloom.Interfaces
{
    public interface IWebsiteUserRepository
    {
        #region Functions of Wishlist
        Task<ApiResponseDTO> addToWishlistAsync(int mstUserId, int mstProductId);
        Task<ApiResponseDTO> removeFromWishlistAsync(int mstUserId, int mstProductId);
        Task<List<WishlistDTO>> getUserWishlistAsync(int mstUserId);
        #endregion

        Task<ApiResponseDTO> addToBagAsync(AddToCartDTO payload);
        Task<ApiResponseDTO> updateCartQuantityAsync(UpdateCartQuantityDTO payload);
        Task<List<CartItemDTO>> getUserCartAsync(int mstUserId);

        #region Functions of User Orders
        Task<List<UserOrdersDTO>> getUserOrdersAsync(int mstUserId);
        #endregion

        Task<ApiResponseDTO> CreateOrderMarkStatusAsync(CreateOrderDTO payload);
        Task<ApiResponseDTO> CreateCartOrderMarkStatusAsync(int mstUserId);
        Task<ApiResponseDTO> VerifyPaymentAsync(VerifyPaymentDTO request);
        Task<ApiResponseDTO> MarkOrderFailedAsync(int orderId);

        #region Functions of FAQ
        Task<ApiResponseDTO> addFaqAsync(AddFaqDTO payload);
        Task<ApiResponseDTO> getAllFaqsAsync();
        Task<ApiResponseDTO> getAdminFaqsAsync();
        Task<ApiResponseDTO> updateFaqStatusAsync(int faqId, bool isActive);
        Task<ApiResponseDTO> updateFaqAsync(UpdateFaqDTO payload);
        Task<ApiResponseDTO> deleteFaqAsync(int faqId);
        #endregion
    }
}

