using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using hariloom.Helpers.DbContexts;
using hariloom.Models.DTOs;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Entity;

namespace hariloom.Repository
{
    public class MstUserRepository : IMstUserRepository
    {
        private readonly appDBContext _context;
        private readonly IApiResponseRepository _apiResponseRepository;

        public MstUserRepository(appDBContext context, IApiResponseRepository apiResponseRepository)
        {
            _context = context;
            _apiResponseRepository = apiResponseRepository;
        }

        public async Task<List<CartItemDTO>> getUserCartAsync(int mstUserId)
        {
            var cartItems = await _context.trnCart.Where(c => c.mstUserId == mstUserId && c.isActive).Include(c => c.product).ToListAsync();

            var result = cartItems.Select(c =>
            {
                var basePrice = c.product?.basePrice ?? 0;
                var unit = Math.Max(0, basePrice);
                var imagesList = new List<hariloom.Models.DTOs.ProductImageDTO>();
                try
                {
                    var paths = (c.product?.productImages ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);
                    imagesList = paths.Select(p => new hariloom.Models.DTOs.ProductImageDTO { imageBase64 = p.Trim(), name = string.Empty }).ToList();
                }
                catch { }

                return new CartItemDTO
                {
                    trnCartId = c.trnCartId,
                    mstProductId = c.mstProductId,
                    productName = c.product?.productName,
                    coverImageBase64 = string.Empty,
                    coverImagePath = c.product?.coverImagePath,
                    productImages = imagesList,
                    quantity = c.quantity,
                    size = c.size,
                    unitPrice = (decimal)unit,
                    totalPrice = (decimal)(unit * c.quantity),
                    basePrice = c.product?.basePrice ?? 0,
                };
            }).ToList();

            return result;

        }

        public async Task<bool> removeFromBagAsync(int mstUserId, int mstProductId)
        {
            try
            {
                // mark matching trnCart items inactive to remove from bag
                var cartItem = await _context.trnCart.FirstOrDefaultAsync(w => w.mstUserId == mstUserId && w.mstProductId == mstProductId && w.isActive);

                if (cartItem == null)
                    return false;

                cartItem.isActive = false;
                _context.trnCart.Update(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[removeFromBagAsync] Exception: {ex.Message}");
                return false;
            }
        }



        public async Task<UserDashboardDTO?> getUserDashboardDetails(int mstUserId)
        {
            return await _context.mstUser
                .Where(x => x.mstUserId == mstUserId && x.isActive)
                .Select(x => new UserDashboardDTO
                {
                    mstUserId = x.mstUserId,
                    name = x.name,
                    email = x.email,
                    phoneNumber = x.phoneNumber,
                    profileImageUrl = x.profileImageUrl,
                    memberSinceYear = x.createdDate.Year,
                    addressLine1 = x.addressLine1,
                    addressLine2 = x.addressLine2,
                    city = x.city,
                    state = x.state,
                    zipCode = x.zipCode
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserOrdersDTO>> getUserCompletedOrders(int mstUserId)
        {
            return await _context.trnOrder
                .Where(x => x.mstUserId == mstUserId && x.isActive)
                .OrderByDescending(x => x.orderDate)
                .Select(order => new UserOrdersDTO
                {
                    trnOrderId = order.trnOrderId,
                    orderNumber = order.orderNumber,
                    orderDate = order.orderDate,
                    totalAmount = order.totalAmount,
                    orderItems = order.orderItems
                        .Where(oi => oi.isDelivered && oi.isActive)
                        .Select(oi => new OrderItemDTO
                        {
                            trnOrderItemId = oi.trnOrderItemsId,
                            trnOrderId = oi.trnOrderId,
                            mstProductId = oi.mstProductId,
                            productName = oi.product.productName,
                            coverImagePath = oi.product.coverImagePath,
                            quantity = oi.quantity,
                            price = oi.price,
                            isDelivered = oi.isDelivered,
                            deliveredDate = oi.deliveredDate,
                            orderDate = order.orderDate,
                            orderNumber = order.orderNumber
                        })
                        .ToList()
                })
                .Where(x => x.orderItems.Any())
                .ToListAsync();
        }

        public async Task<bool> updateUserAddress(UserDashboardDTO addressData)
        {
            try
            {
                var user = await _context.mstUser.FirstOrDefaultAsync(x => x.mstUserId == addressData.mstUserId && x.isActive);

                if (user == null)
                {
                    return false;
                }

                user.addressLine1 = addressData.addressLine1;
                user.addressLine2 = addressData.addressLine2;
                user.city = addressData.city;
                user.state = addressData.state;
                user.zipCode = addressData.zipCode;

                _context.mstUser.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[updateUserAddress] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<List<UserAddressDTO>> getUserAddressesAsync(int mstUserId)
        {
            try
            {
                return await _context.trnUserAddress
                    .Where(a => a.mstUserId == mstUserId && a.isActive)
                    .Select(a => new UserAddressDTO
                    {
                        trnUserAddressId = a.trnUserAddressId,
                        mstUserId = a.mstUserId,
                        label = a.label,
                        addressLine1 = a.addressLine1,
                        addressLine2 = a.addressLine2,
                        city = a.city,
                        state = a.state,
                        zipCode = a.zipCode
                    })
                    .OrderByDescending(a => a.trnUserAddressId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[getUserAddressesAsync] Exception: {ex.Message}");
                return new List<UserAddressDTO>();
            }
        }

        public async Task<bool> addUserAddressAsync(UserAddressDTO payload)
        {
            try
            {
                var newAddress = new trnUserAddress
                {
                    mstUserId = payload.mstUserId,
                    label = payload.label,
                    addressLine1 = payload.addressLine1,
                    addressLine2 = payload.addressLine2,
                    city = payload.city,
                    state = payload.state,
                    zipCode = payload.zipCode,
                    isActive = true,
                    createdDate = DateTime.Now
                };

                _context.trnUserAddress.Add(newAddress);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[addUserAddressAsync] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> addToWishlistAsync(int mstUserId, int mstProductId)
        {
            try
            {
                // Check if user exists
                var userExists = await _context.mstUser.AnyAsync(u => u.mstUserId == mstUserId && u.isActive);
                if (!userExists)
                    return false;

                // Check if product exists
                var productExists = await _context.mstProduct.AnyAsync(p => p.mstProductId == mstProductId && p.isActive && p.isAvailable);
                if (!productExists)
                    return false;

                // Check if already in wishlist
                var existingWishlist = await _context.trnWishlist
                    .FirstOrDefaultAsync(w => w.mstUserId == mstUserId && w.mstProductId == mstProductId && w.isActive);

                if (existingWishlist != null)
                    return true; // Already in wishlist

                // Add to wishlist
                var wishlistItem = new trnWishlist
                {
                    mstUserId = mstUserId,
                    mstProductId = mstProductId
                };

                _context.trnWishlist.Add(wishlistItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[addToWishlistAsync] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> removeFromWishlistAsync(int mstUserId, int mstProductId)
        {
            try
            {
                var wishlistItem = await _context.trnWishlist
                    .FirstOrDefaultAsync(w => w.mstUserId == mstUserId && w.mstProductId == mstProductId && w.isActive);

                if (wishlistItem == null)
                    return false;

                wishlistItem.isActive = false;
                _context.trnWishlist.Update(wishlistItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[removeFromWishlistAsync] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<List<WishlistDTO>> getUserWishlistAsync(int mstUserId)
        {
            try
            {
                var wishlist = await _context.trnWishlist
                    .Where(w => w.mstUserId == mstUserId && w.isActive)
                    .Select(w => new WishlistDTO
                    {
                        trnWishlistId = w.trnWishlistId,
                        mstUserId = w.mstUserId,
                        mstProductId = w.mstProductId,
                        productName = w.product.productName,
                        coverImageBase64 = string.Empty,
                        coverImagePath = w.product.coverImagePath,
                        basePrice = w.product.basePrice,
                        addedDate = w.createdDate,
                        isActive = w.isActive
                    })
                    .OrderByDescending(w => w.addedDate)
                    .ToListAsync();

                // Base64 conversion removed for performance reasons

                return wishlist;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[getUserWishlistAsync] Exception: {ex.Message}");
                return new List<WishlistDTO>();
            }
        }

        public async Task<bool> isProductInWishlistAsync(int mstUserId, int mstProductId)
        {
            try
            {
                return await _context.trnWishlist
                    .AnyAsync(w => w.mstUserId == mstUserId && w.mstProductId == mstProductId && w.isActive);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[isProductInWishlistAsync] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<List<WishlistStatusDTO>> checkWishlistStatusAsync(int mstUserId, List<int> productIds)
        {
            try
            {
                var wishlistedProductIds = await _context.trnWishlist
                    .Where(w => w.mstUserId == mstUserId && w.isActive && productIds.Contains(w.mstProductId))
                    .Select(w => w.mstProductId)
                    .ToListAsync();

                var result = productIds.Select(productId => new WishlistStatusDTO
                {
                    mstProductId = productId,
                    isWishlisted = wishlistedProductIds.Contains(productId)
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[checkWishlistStatusAsync] Exception: {ex.Message}");
                return new List<WishlistStatusDTO>();
            }
        }

        private string GetImageFromPathAndConvertToBase64(string imagePath)
        {
            string base64String = string.Empty;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    var imageBytes = File.ReadAllBytes(imagePath);
                    base64String = Convert.ToBase64String(imageBytes);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetImageFromPathAndConvertToBase64] Exception: {ex.Message}");
                }
            }

            return base64String;
        }

        private List<string> GetImageFromPathAndConvertToListBase64(string imagePaths)
        {
            var base64List = new List<string>();

            if (!string.IsNullOrWhiteSpace(imagePaths))
            {
                var paths = imagePaths.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var path in paths)
                {
                    var trimmedPath = path.Trim();
                    if (File.Exists(trimmedPath))
                    {
                        try
                        {
                            var imageBytes = File.ReadAllBytes(trimmedPath);
                            var base64 = Convert.ToBase64String(imageBytes);
                            base64List.Add(base64);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[GetImageFromPathAndConvertToListBase64] Exception: {ex.Message}");
                        }
                    }
                }
            }

            return base64List;
        }

        public async Task<ApiResponseDTO> getAllUserAccountsAsync()
        {
            var users = await _context.mstUser
                .Where(u => u.accessLevel == 1)
                .Select(u => new UserDatabaseDTO
                {
                    mstUserId = u.mstUserId,
                    name = u.name,
                    phoneNumber = u.phoneNumber,
                    email = u.email,
                    createdDate = u.createdDate,
                    isPromotionalEmailOptIn = u.isPromotionalEmailOptIn,
                    ipAddress = u.ipAddress,
                    visitCount = u.visitCount
                })
                .OrderByDescending(u => u.createdDate)
                .ToListAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "User accounts retrieved successfully.", data = users });
        }
    }
}
