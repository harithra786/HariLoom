using hariloom.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hariloom.Interfaces
{
    public interface IMstUserRepository
    {
        Task<UserDashboardDTO?> getUserDashboardDetails(int mstUserId);

        Task<List<UserOrdersDTO>> getUserCompletedOrders(int mstUserId);

        Task<bool> updateUserAddress(UserDashboardDTO addressData);

        Task<bool> addToWishlistAsync(int mstUserId, int mstProductId);

        Task<bool> removeFromWishlistAsync(int mstUserId, int mstProductId);

        Task<List<WishlistDTO>> getUserWishlistAsync(int mstUserId);

        Task<bool> isProductInWishlistAsync(int mstUserId, int mstProductId);

        Task<List<WishlistStatusDTO>> checkWishlistStatusAsync(int mstUserId, List<int> productIds);

        Task<bool> removeFromBagAsync(int mstUserId, int mstProductId);
        
        Task<ApiResponseDTO> getAllUserAccountsAsync();
        
        Task<List<UserAddressDTO>> getUserAddressesAsync(int mstUserId);
        Task<bool> addUserAddressAsync(UserAddressDTO payload);
    }
}
