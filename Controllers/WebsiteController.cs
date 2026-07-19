using Microsoft.AspNetCore.Mvc;
using hariloom.Interfaces;
using hariloom.Models.DTOs;

namespace hariloom.Controllers
{
    public class WebsiteController : Controller
    {
        #region Interface Implementations
        private readonly IMstUserRepository _mstUserRepository;
        private readonly IProductsManagementRepository _productRepository;
        private readonly IApiResponseRepository _apiResponseRepository;
        private readonly IWebsiteUserRepository _websiteUserRepository;

        public WebsiteController(IMstUserRepository mstUserRepository, IProductsManagementRepository productRepository, IApiResponseRepository apiResponseRepository, IWebsiteUserRepository websiteUserRepository)
        {
            _mstUserRepository = mstUserRepository;
            _productRepository = productRepository;
            _apiResponseRepository = apiResponseRepository;
            _websiteUserRepository = websiteUserRepository;
        }
        #endregion

        #region Home Page Functionalities

        #region Functions to load Home Page
        public IActionResult Home()
        {
            return View();
        }
        #endregion

        #region Functions for user to add to wishlist 
        [HttpPost]
        public async Task<ApiResponseDTO> addToWishlist([FromBody] AddToWishlistDTO wishlistData)
        {
            var reponse = await _websiteUserRepository.addToWishlistAsync(wishlistData.mstUserId, wishlistData.mstProductId);
            return reponse;
        }
        #endregion

        #region Functions for user to remove from wishlist 
        [HttpPost]
        public async Task<ApiResponseDTO> removeFromWishlist([FromBody] AddToWishlistDTO wishlistData)
        {
            var reponse = await _websiteUserRepository.removeFromWishlistAsync(wishlistData.mstUserId, wishlistData.mstProductId);
            return reponse;
        }
        #endregion

        #region Functions to check wishlist
        [HttpPost]
        public async Task<ApiResponseDTO> checkWishlistStatus([FromBody] WishlistCheckDTO checkData)
        {

            if (checkData == null || checkData.mstUserId <= 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Invalid input data." });

            var wishlistStatus = await _mstUserRepository.checkWishlistStatusAsync(checkData.mstUserId, checkData.productIds);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { data = wishlistStatus, message = "Wishlist status fetched successfully." });
        }
        #endregion

        #endregion

        #region Wishlist Page Functionality

        #region Functions to load Wishlist Page
        public IActionResult Wishlist()
        {
            return View();
        }
        #endregion

        #region Function to load user wishlist
        [HttpGet]
        public async Task<ApiResponseDTO> getUserWishlist(int mstUserId)
        {
            if (mstUserId == 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Invalid user ID" });

            var wishlist = await _websiteUserRepository.getUserWishlistAsync(mstUserId);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { data = wishlist });
        }
        #endregion

        #endregion

        #region Product Details Page Functionality

        #region Function to load page
        public async Task<IActionResult> ProductDetail()
        {
            return View();
        }
        #endregion

        #region Functionality for buy now button
        public async Task<IActionResult> BuyNow(int mstProductId, string size, int quantity, decimal price, string productName)
        {
            var product = await _productRepository.GetProductByIdAsync(mstProductId);
            if (product == null)
                return RedirectToAction("Home", "Website");

            var productDetails = new BuyNowDTO
            {
                mstProductId = mstProductId,
                size = size ?? "",
                quantity = quantity,
                price = price,
                productName = productName,
                coverImageBase64 = string.Empty,
                coverImagePath = product.coverImagePath ?? string.Empty,
                mrp = product.basePrice,
                discountedPrice = product.discountedPrice,
                quantityAvailable = product.quantityAvailable,
                sizes = product.sizes ?? new List<SizeDTO>()
            };

            if (price == 0)
                return RedirectToAction("Home", "Website");

            return View(productDetails);
        }
        #endregion

        #region Function to add to bag
        [HttpPost]
        public async Task<ApiResponseDTO> AddToBag([FromBody] AddToCartDTO payload)
        {
            var result = await _websiteUserRepository.addToBagAsync(payload);
            return result;
        }
        #endregion

        #region Function to update cart quantity
        [HttpPost]
        public async Task<ApiResponseDTO> UpdateCartQuantity([FromBody] UpdateCartQuantityDTO payload)
        {
            var result = await _websiteUserRepository.updateCartQuantityAsync(payload);
            return result;
        }
        #endregion

        #region Function to remove from bag
        [HttpPost]
        public async Task<ApiResponseDTO> RemoveFromBag([FromBody] RemoveFromCartDTO payload)
        {
            if (payload == null || payload.userId <= 0 || payload.productId <= 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid request." });

            var result = await _mstUserRepository.removeFromBagAsync(payload.userId, payload.productId);
            if (result)
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Item removed successfully." });
            else
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Failed to remove item." });
        }
        #endregion

        #endregion

        #region Bag Page Functionality

        #region Function to load page
        public async Task<IActionResult> Bag()
        {
            return View();
        }
        #endregion

        #region Get User Saved Bag Details By UserId
        [HttpGet]
        public async Task<ApiResponseDTO> GetUserCart(int userId)
        {
            if (userId == 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Invalid user ID" });

            var userCartDetils = await _websiteUserRepository.getUserCartAsync(userId);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { data = userCartDetils });
        }
        #endregion

        #endregion

        #region Buy Now Page Functionality

        #region Create Razor page Link and Send to Frontend
        [HttpGet]
        public async Task<ApiResponseDTO> CreateOrderMarkStatus(int userId,int productId,int quantity,string size)
        {
            var request = new CreateOrderDTO
            {
                userId = userId,
                productId = productId,
                quantity = quantity,
                size = size
            };

            var result = await _websiteUserRepository.CreateOrderMarkStatusAsync(request);
            return result;
        }

        [HttpGet]
        public async Task<ApiResponseDTO> CreateCartOrderMarkStatus(int userId)
        {
            var result = await _websiteUserRepository.CreateCartOrderMarkStatusAsync(userId);
            return result;
        }
        #endregion

        #region Verify Payment
        [HttpPost]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDTO request)
        {
            var result = await _websiteUserRepository.VerifyPaymentAsync(request);
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> MarkPaymentFailed(int orderId)
        {
            var result = await _websiteUserRepository.MarkOrderFailedAsync(orderId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendOrderConfirmationEmail([FromBody] SendOrderConfirmationRequestDTO request, [FromServices] IEmailService emailService)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.OrderId))
                return BadRequest(new { success = false, message = "Invalid request" });

            var success = await emailService.SendOrderConfirmationEmailAsync(request.Email, request.FirstName ?? "Customer", request.OrderId, request.OrderDate ?? DateTime.Now.ToString("dd MMM yyyy"), request.Items ?? new List<OrderEmailItemDTO>());
            return Ok(new { success = success });
        }
        #endregion

        #endregion




        #region Functions to load Account Page

        public IActionResult Account()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetUserAddresses(int mstUserId)
        {
            if (mstUserId <= 0) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid User" });
            var data = await _mstUserRepository.getUserAddressesAsync(mstUserId);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { data = data });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> AddUserAddress([FromBody] UserAddressDTO request)
        {
            if (request.mstUserId <= 0) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid User" });
            var res = await _mstUserRepository.addUserAddressAsync(request);
            if (res) return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Address added successfully" });
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Failed to add address" });
        }

        #endregion



        #region Functions to load Info Pages

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Shipping()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        #endregion

        #region Functions to get User Dashboard Details

        [HttpGet]
        public async Task<ApiResponseDTO> getUserDashboardDetails(int mstUserId)
        {
            ApiResponseDTO response = new ApiResponseDTO();

            try
            {
                var result = await _mstUserRepository.getUserDashboardDetails(mstUserId);

                if (result == null)
                {
                    response.success = false;
                    response.statusCode = 404;
                    response.message = "User not found";
                    response.data = null;

                    return response;
                }

                response.success = true;
                response.statusCode = 200;
                response.message = "User details fetched successfully";
                response.data = result;

                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = 500;
                response.message = "Internal server error";
                response.data = ex.Message;

                return response;
            }
        }

        #endregion

        #region Functions to get User Completed Orders

        [HttpGet]
        public async Task<ApiResponseDTO> getUserCompletedOrders(int mstUserId)
        {
            ApiResponseDTO response = new ApiResponseDTO();

            try
            {
                var result = await _mstUserRepository.getUserCompletedOrders(mstUserId);

                response.success = true;
                response.statusCode = 200;
                response.message = "User completed orders fetched successfully";
                response.data = result;

                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = 500;
                response.message = "Internal server error";
                response.data = ex.Message;

                return response;
            }
        }

        #endregion

        #region Functions to update User Address

        [HttpPost]
        public async Task<ApiResponseDTO> updateUserAddress([FromBody] UserDashboardDTO addressData)
        {
            ApiResponseDTO response = new ApiResponseDTO();

            try
            {
                if (addressData == null || addressData.mstUserId <= 0)
                {
                    response.success = false;
                    response.statusCode = 400;
                    response.message = "Invalid user ID";
                    response.data = null;

                    return response;
                }

                var result = await _mstUserRepository.updateUserAddress(addressData);

                if (!result)
                {
                    response.success = false;
                    response.statusCode = 404;
                    response.message = "User not found or address update failed";
                    response.data = null;

                    return response;
                }

                response.success = true;
                response.statusCode = 200;
                response.message = "Address updated successfully";
                response.data = null;

                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.statusCode = 500;
                response.message = "Internal server error";
                response.data = ex.Message;

                return response;
            }
        }

        #endregion

        #region Orders Page Functionality

        #region Function to load Orders Page
        public IActionResult Orders()
        {
            return View();
        }
        #endregion

        #region Function to get user orders
        [HttpGet]
        public async Task<ApiResponseDTO> getUserOrders(int mstUserId)
        {
            if (mstUserId == 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Invalid user ID" });

            var orders = await _websiteUserRepository.getUserOrdersAsync(mstUserId);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { data = orders });
        }
        #endregion

        #endregion

    }
}