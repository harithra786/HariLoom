using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Razorpay.Api;
using hariloom.Helpers.DbContexts;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Entity;
using hariloom.Models.Enums;
using System.Text;
using Microsoft.Extensions.Logging;

namespace hariloom.Repository
{
    public class WebsiteUserRepository : IWebsiteUserRepository
    {
        #region Interface and Repository Implementations       
        private readonly appDBContext _context;
        private readonly IApiResponseRepository _apiResponseRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<WebsiteUserRepository> _logger;
        public WebsiteUserRepository(appDBContext context, IApiResponseRepository apiResponseRepository, IConfiguration configuration, IEmailService emailService, ILogger<WebsiteUserRepository> logger)
        {
            _context = context;
            _apiResponseRepository = apiResponseRepository;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }
        #endregion

        #region Helpers : Image Download From Path
        private string ResolveImagePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return null;

            if (File.Exists(imagePath))
                return imagePath;

            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var resolvedPath = Path.Combine(wwwrootPath, imagePath.TrimStart('/', '\\'));
            if (File.Exists(resolvedPath))
                return resolvedPath;

            var wwwrootMarkers = new[] { "wwwroot/", "wwwroot\\" };
            foreach (var marker in wwwrootMarkers)
            {
                var idx = imagePath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    var relativePart = imagePath.Substring(idx + marker.Length);
                    var fallbackPath = Path.Combine(wwwrootPath, relativePart.TrimStart('/', '\\'));
                    if (File.Exists(fallbackPath))
                        return fallbackPath;
                }
            }

            return null;
        }

        private string GetImageFromPathAndConvertToBase64(string imagePaths)
        {
            var resolved = ResolveImagePath(imagePaths);
            if (resolved == null)
                return string.Empty;

            return Convert.ToBase64String(File.ReadAllBytes(resolved));
        }
        #endregion

        #region Function Implementations Of Home Page
        public async Task<ApiResponseDTO> addToWishlistAsync(int mstUserId, int mstProductId)
        {
            // Check if user exists
            var userExists = await _context.mstUser.AnyAsync(u => u.mstUserId == mstUserId && u.isActive);
            if (!userExists)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "User not found" });

            // Check if product exists
            var productExists = await _context.mstProduct.AnyAsync(p => p.mstProductId == mstProductId && p.isActive && p.isAvailable);
            if (!productExists)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Product not found" });

            // Check if already in wishlist
            var existingWishlist = await _context.trnWishlist.FirstOrDefaultAsync(w => w.mstUserId == mstUserId && w.mstProductId == mstProductId && w.isActive);

            if (existingWishlist != null)
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Already in wishlist" }); // Already in wishlist

            // Add to wishlist
            var wishlistItem = new trnWishlist
            {
                mstUserId = mstUserId,
                mstProductId = mstProductId
            };

            _context.trnWishlist.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Added to wishlist" });
        }

        public async Task<ApiResponseDTO> removeFromWishlistAsync(int mstUserId, int mstProductId)
        {
            var wishlistItem = await _context.trnWishlist.FirstOrDefaultAsync(w => w.mstUserId == mstUserId && w.mstProductId == mstProductId && w.isActive);
            if (wishlistItem == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO() { message = "Wishlist item not found" });

            wishlistItem.isActive = false;
            _context.trnWishlist.Update(wishlistItem);
            await _context.SaveChangesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Removed from wishlist" });
        }

        public async Task<List<WishlistDTO>> getUserWishlistAsync(int mstUserId)
        {
            var wishlistData = await _context.trnWishlist
                .Where(w => w.mstUserId == mstUserId && w.isActive)
                .Select(w => new
                {
                    trnWishlistId = w.trnWishlistId,
                    mstUserId = w.mstUserId,
                    mstProductId = w.mstProductId,
                    productName = w.product.productName,
                    coverImagePath = w.product.coverImagePath,
                    basePrice = w.product.basePrice,
                    addedDate = w.createdDate,
                    isActive = w.isActive
                })
                .OrderByDescending(w => w.addedDate)
                .ToListAsync();

            var wishlist = wishlistData.Select(w => new WishlistDTO
            {
                trnWishlistId = w.trnWishlistId,
                mstUserId = w.mstUserId,
                mstProductId = w.mstProductId,
                productName = w.productName,
                coverImageBase64 = string.Empty,
                coverImagePath = w.coverImagePath != null ? w.coverImagePath.Replace("\\", "/") : null,
                basePrice = w.basePrice,
                addedDate = w.addedDate,
                isActive = w.isActive
            }).ToList();

            return wishlist;
        }
        #endregion

        #region Function Implementations Of Buy Now
        public async Task<ApiResponseDTO> CreateOrderMarkStatusAsync(CreateOrderDTO payload)
        {
            // Validate payload
            if (payload.userId <= 0 || payload.productId <= 0 || payload.quantity <= 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid order data" });

            // Check if user exists
            var userExists = await _context.mstUser.AnyAsync(u => u.mstUserId == payload.userId && u.isActive);
            if (!userExists) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "User not found" });

            var productDetails = await _context.mstProduct.FirstOrDefaultAsync(p => p.mstProductId == payload.productId && p.isActive && p.isAvailable);
            if (productDetails == null) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Product not found" });

            var totalAvailable = await _context.trnProductSize
                .Where(s => s.mstProductId == payload.productId && s.size == payload.size && s.isActive)
                .SumAsync(s => s.quantityAvailable);
            
            if (totalAvailable < payload.quantity)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = $"Only {totalAvailable} items available in stock." });

            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var orderCount = await _context.trnOrder.CountAsync(o => o.orderDate >= today && o.orderDate < tomorrow);
            var nextSequence = (orderCount + 1).ToString("D4");

            // Create order
            var order = new trnOrder
            {
                mstUserId = payload.userId,
                orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{nextSequence}",
                orderDate = DateTime.Now,
                totalAmount = productDetails.basePrice * payload.quantity,
                orderStatus = (int)orderStatusEnum.Unpaid,
                isActive = true,
                createdBy = payload.userId,
                createdDate = DateTime.Now
            };
            _context.trnOrder.Add(order);
            await _context.SaveChangesAsync();

            // Add order items
            var orderItem = new trnOrderItems
            {
                trnOrderId = order.trnOrderId,
                mstProductId = productDetails.mstProductId,
                quantity = payload.quantity,
                price = productDetails.basePrice,
                size = payload.size ?? "",
                isDelivered = false,
                isActive = true,
                createdBy = payload.userId,
                createdDate = DateTime.Now
            };
            _context.trnOrderItem.Add(orderItem);
            await _context.SaveChangesAsync();


            decimal totalAmount = productDetails.basePrice * payload.quantity;
            string key = _configuration["Razorpay:Key"];
            string secret = _configuration["Razorpay:Secret"];

            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToInt32(totalAmount * 100));
            options.Add("currency", "INR");
            options.Add("receipt", $"TEMP_{DateTime.Now.Ticks}");
            Razorpay.Api.Order orderOptions = client.Order.Create(options);

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO
            {
                message = "Order created successfully",
                data = new
                {
                    orderId = order.trnOrderId,
                    razorpayOrderId = orderOptions["id"].ToString(),
                    amount = totalAmount * 100,
                    key = key
                }
            });
        }

        public async Task<ApiResponseDTO> CreateCartOrderMarkStatusAsync(int userId)
        {
            if (userId <= 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid user data" });

            var userExists = await _context.mstUser.AnyAsync(u => u.mstUserId == userId && u.isActive);
            if (!userExists) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "User not found" });

            var cartItems = await _context.trnCart.Where(c => c.mstUserId == userId && c.isActive).ToListAsync();
            if (!cartItems.Any()) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Cart is empty" });

            // Validate inventory for all cart items first
            foreach(var item in cartItems)
            {
                var productDetails = await _context.mstProduct.FirstOrDefaultAsync(p => p.mstProductId == item.mstProductId);
                if (productDetails != null)
                {
                    var totalAvailable = await _context.trnProductSize
                        .Where(s => s.mstProductId == item.mstProductId && s.size == item.size && s.isActive)
                        .SumAsync(s => s.quantityAvailable);

                    if (totalAvailable < item.quantity)
                    {
                        return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = $"Only {totalAvailable} items available for {productDetails.productName} ({item.size})." });
                    }
                }
            }

            decimal totalAmountDecimal = 0;
            int totalAmount = 0;
            var orderItemsList = new List<trnOrderItems>();

            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var orderCount = await _context.trnOrder.CountAsync(o => o.orderDate >= today && o.orderDate < tomorrow);
            var nextSequence = (orderCount + 1).ToString("D4");

            var order = new trnOrder
            {
                mstUserId = userId,
                orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{nextSequence}",
                orderDate = DateTime.Now,
                orderStatus = (int)orderStatusEnum.Unpaid,
                isActive = true,
                createdBy = userId,
                createdDate = DateTime.Now
            };
            _context.trnOrder.Add(order);
            await _context.SaveChangesAsync();

            foreach(var item in cartItems)
            {
                var productDetails = await _context.mstProduct.FirstOrDefaultAsync(p => p.mstProductId == item.mstProductId);
                if (productDetails != null)
                {
                    int price = productDetails.basePrice; 
                    totalAmount += price * item.quantity;
                    
                    var orderItem = new trnOrderItems
                    {
                        trnOrderId = order.trnOrderId,
                        mstProductId = productDetails.mstProductId,
                        quantity = item.quantity,
                        price = price,
                        size = item.size ?? "",
                        isDelivered = false,
                        isActive = true,
                        createdBy = userId,
                        createdDate = DateTime.Now
                    };
                    orderItemsList.Add(orderItem);
                }
            }

            order.totalAmount = totalAmount;
            _context.trnOrder.Update(order);
            _context.trnOrderItem.AddRange(orderItemsList);
            await _context.SaveChangesAsync();

            string key = _configuration["Razorpay:Key"];
            string secret = _configuration["Razorpay:Secret"];
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToInt32(totalAmount * 100));
            options.Add("currency", "INR");
            options.Add("receipt", $"TEMP_{DateTime.Now.Ticks}");
            Razorpay.Api.Order orderOptions = client.Order.Create(options);

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO
            {
                message = "Order created successfully",
                data = new
                {
                    orderId = order.trnOrderId,
                    razorpayOrderId = orderOptions["id"].ToString(),
                    amount = totalAmount * 100,
                    key = key
                }
            });
        }


        public async Task<ApiResponseDTO> VerifyPaymentAsync(VerifyPaymentDTO request)
        {
            string secret = _configuration["Razorpay:Secret"];
            string generatedSignature;

            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{request.razorpayOrderId}|{request.razorpayPaymentId}"));
                generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            }

            if (generatedSignature != request.razorpaySignature)
            {
                // Log failed payment due to invalid signature
                var failureLog = new trnPaymentLog
                {
                    trnOrderId = request.orderId,
                    razorpayOrderId = request.razorpayOrderId,
                    razorpayPaymentId = request.razorpayPaymentId,
                    razorpaySignature = request.razorpaySignature,
                    paymentStatus = "Failed",
                    failureReason = "Invalid payment signature",
                    amount = null,
                    isActive = true,
                    createdDate = DateTime.Now
                };

                _context.Set<trnPaymentLog>().Add(failureLog);
                await _context.SaveChangesAsync();

                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid payment signature" });
            }

            var order = await _context.trnOrder.FirstOrDefaultAsync(x => x.trnOrderId == request.orderId && x.isActive);
            if (order == null)
            {
                // Log failed payment due to missing order
                var failureLog = new trnPaymentLog
                {
                    trnOrderId = request.orderId,
                    razorpayOrderId = request.razorpayOrderId,
                    razorpayPaymentId = request.razorpayPaymentId,
                    razorpaySignature = request.razorpaySignature,
                    paymentStatus = "Failed",
                    failureReason = "Order not found",
                    amount = null,
                    isActive = true,
                    createdDate = DateTime.Now
                };

                _context.Set<trnPaymentLog>().Add(failureLog);
                await _context.SaveChangesAsync();

                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Order not found" });
            }

            order.orderStatus = (int)orderStatusEnum.Paid;
            order.updatedDate = DateTime.Now;
            _context.trnOrder.Update(order);

            // Log successful payment
            var successLog = new trnPaymentLog
            {
                trnOrderId = order.trnOrderId,
                mstUserId = order.mstUserId,
                razorpayOrderId = request.razorpayOrderId,
                razorpayPaymentId = request.razorpayPaymentId,
                razorpaySignature = request.razorpaySignature,
                paymentStatus = "Success",
                failureReason = null,
                amount = order.totalAmount,
                isActive = true,
                createdDate = DateTime.Now
            };

            _context.Set<trnPaymentLog>().Add(successLog);

            if (request.isCartCheckout)
            {
                var cartItems = await _context.trnCart.Where(c => c.mstUserId == order.mstUserId && c.isActive).ToListAsync();
                foreach (var item in cartItems)
                {
                    item.isActive = false;
                    item.updatedDate = DateTime.Now;
                }
                _context.trnCart.UpdateRange(cartItems);
            }

            // Update product inventory (deducting from sizes, taking from any matching color)
            var orderItems = await _context.trnOrderItem.Where(oi => oi.trnOrderId == order.trnOrderId && oi.isActive).ToListAsync();
            foreach (var item in orderItems)
            {
                var sizes = await _context.trnProductSize
                    .Where(p => p.mstProductId == item.mstProductId && p.size == item.size && p.isActive && p.quantityAvailable > 0)
                    .OrderByDescending(p => p.quantityAvailable)
                    .ToListAsync();

                int qtyToDeduct = item.quantity;
                foreach (var s in sizes)
                {
                    if (qtyToDeduct <= 0) break;
                    
                    int deduct = Math.Min(s.quantityAvailable, qtyToDeduct);
                    s.quantityAvailable -= deduct;
                    qtyToDeduct -= deduct;
                    _context.trnProductSize.Update(s);
                }
            }

            await _context.SaveChangesAsync();

            try
            {
                var user = await _context.mstUser.FirstOrDefaultAsync(u => u.mstUserId == order.mstUserId);
                
                var emailItems = new List<OrderEmailItemDTO>();
                foreach (var item in orderItems)
                {
                    var product = await _context.mstProduct.FirstOrDefaultAsync(p => p.mstProductId == item.mstProductId);
                    emailItems.Add(new OrderEmailItemDTO
                    {
                        ImageUrl = product?.coverImagePath != null ? "https://hariloom.in/" + product.coverImagePath : "",
                        Name = product?.productName ?? "Product",
                        Units = item.quantity,
                        Price = item.price.ToString("0.00")
                    });
                }

                if (user != null && !string.IsNullOrEmpty(user.email))
                {
                    _logger.LogInformation("Starting order confirmation email");
                    var emailSent = await _emailService.SendOrderConfirmationEmailAsync(user.email, user.name, order.orderNumber, order.orderDate.ToString("dd MMM yyyy"), emailItems);
                    if (emailSent)
                    {
                        _logger.LogInformation("Order confirmation email sent");
                    }
                    else
                    {
                        _logger.LogError("Order confirmation email failed to send (check EmailService logs)");
                    }
                }
                
                await _emailService.SendNewOrderAdminNotificationAsync(order.orderNumber, order.totalAmount.ToString("0.00"), emailItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in email sending block");
            }

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Payment verified successfully" });
        }

        public async Task<ApiResponseDTO> MarkOrderFailedAsync(int orderId)
        {
            var order = await _context.trnOrder.FirstOrDefaultAsync(x => x.trnOrderId == orderId && x.isActive);
            if (order == null)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Order not found" });
            }

            // Only update if it's Unpaid
            if (order.orderStatus == (int)orderStatusEnum.Unpaid)
            {
                order.orderStatus = (int)orderStatusEnum.Cancelled;
                order.updatedDate = DateTime.Now;
                _context.trnOrder.Update(order);
                await _context.SaveChangesAsync();
            }

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Order marked as failed successfully" });
        }
        #endregion


        public async Task<ApiResponseDTO> addToBagAsync(AddToCartDTO payload)
        {
            if (payload.userId <= 0 || payload.productId <= 0 || payload.quantity <= 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid payload" });

            var userExists = await _context.mstUser.AnyAsync(u => u.mstUserId == payload.userId && u.isActive);
            if (!userExists) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "User not found" });

            var product = await _context.mstProduct.FirstOrDefaultAsync(p => p.mstProductId == payload.productId && p.isActive && p.isAvailable);
            if (product == null) return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Product not found" });

            var totalAvailable = await _context.trnProductSize
                .Where(s => s.mstProductId == payload.productId && s.size == payload.size && s.isActive)
                .SumAsync(s => s.quantityAvailable);

            // Check if existing cart item for same user/product/size
            var existing = await _context.trnCart.FirstOrDefaultAsync(c => c.mstUserId == payload.userId && c.mstProductId == payload.productId && c.size == payload.size && c.isActive);
            int newQuantity = payload.quantity + (existing != null ? existing.quantity : 0);

            if (totalAvailable < newQuantity)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = $"Only {totalAvailable} items available in stock." });

            if (existing != null)
            {
                existing.quantity += payload.quantity;
                _context.trnCart.Update(existing);
            }
            else
            {
                var cartItem = new trnCart
                {
                    mstUserId = payload.userId,
                    mstProductId = payload.productId,
                    size = payload.size,
                    quantity = payload.quantity,
                    isActive = true,
                    createdBy = payload.userId,
                    createdDate = DateTime.Now
                };

                _context.trnCart.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Item added to cart successfully" });
        }

        public async Task<ApiResponseDTO> updateCartQuantityAsync(UpdateCartQuantityDTO payload)
        {
            if (payload.userId <= 0 || payload.cartId <= 0)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Invalid payload" });

            var cartItem = await _context.trnCart.FirstOrDefaultAsync(c => c.trnCartId == payload.cartId && c.mstUserId == payload.userId && c.isActive);
            if (cartItem == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Cart item not found" });

            var totalAvailable = await _context.trnProductSize
                .Where(p => p.mstProductId == cartItem.mstProductId && p.size == cartItem.size && p.isActive)
                .SumAsync(s => s.quantityAvailable);

            if (payload.quantity > 0 && totalAvailable < payload.quantity)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = $"Only {totalAvailable} items available in stock." });
            }

            if (payload.quantity <= 0)
            {
                cartItem.isActive = false; // Remove item if quantity is 0 or less
                _context.trnCart.Update(cartItem);
            }
            else
            {
                cartItem.quantity = payload.quantity;
                _context.trnCart.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO() { message = "Cart updated successfully" });
        }

        public async Task<List<CartItemDTO>> getUserCartAsync(int mstUserId)
        {
            var cartItems = await _context.trnCart
                .Where(c => c.mstUserId == mstUserId && c.isActive)
                .Include(c => c.product)
                .Select(c => new
                {
                    trnCartId = c.trnCartId,
                    mstProductId = c.mstProductId,
                    productName = c.product.productName,
                    coverImagePath = c.product.coverImagePath,
                    quantity = c.quantity,
                    size = c.size,
                    basePrice = c.product.basePrice,
                    quantityAvailable = _context.trnProductSize.Where(s => s.mstProductId == c.mstProductId && s.size == c.size && s.isActive).Sum(s => s.quantityAvailable)
                })
                .ToListAsync();

            var result = cartItems.Select(c =>
            {
                var basePrice = c.basePrice;
                var unit = Math.Max(0, basePrice);

                return new CartItemDTO
                {
                    trnCartId = c.trnCartId,
                    mstProductId = c.mstProductId,
                    productName = c.productName,
                    coverImageBase64 = string.Empty,
                    coverImagePath = c.coverImagePath != null ? c.coverImagePath.Replace("\\", "/") : string.Empty,
                    quantity = c.quantity,
                    size = c.size,
                    unitPrice = (decimal)unit,
                    totalPrice = (decimal)(unit * c.quantity),
                    basePrice = c.basePrice,
                    quantityAvailable = c.quantityAvailable
                };
            }).ToList();

            return result;
        }

        #region Functions for User Orders
        public async Task<List<UserOrdersDTO>> getUserOrdersAsync(int mstUserId)
        {
            var ordersData = await _context.trnOrder
                .Where(o => o.mstUserId == mstUserId && o.isActive)
                .Select(o => new
                {
                    trnOrderId = o.trnOrderId,
                    orderNumber = o.orderNumber,
                    orderDate = o.orderDate,
                    totalAmount = o.totalAmount,
                    orderStatus = o.orderStatus,
                    orderItems = o.orderItems.Where(oi => oi.isActive).Select(oi => new
                    {
                        trnOrderItemsId = oi.trnOrderItemsId,
                        trnOrderId = oi.trnOrderId,
                        mstProductId = oi.mstProductId,
                        productName = oi.product.productName,
                        coverImagePath = oi.product.coverImagePath,
                        quantity = oi.quantity,
                        price = oi.price,
                        size = oi.size,
                        isDelivered = oi.isDelivered,
                        deliveredDate = oi.deliveredDate
                    }).ToList()
                })
                .OrderByDescending(o => o.orderDate)
                .ToListAsync();

            var orders = ordersData.Select(o => new UserOrdersDTO
            {
                trnOrderId = o.trnOrderId,
                orderNumber = o.orderNumber,
                orderDate = o.orderDate,
                totalAmount = o.totalAmount,
                orderStatus = o.orderStatus,
                orderItems = o.orderItems.Select(oi => new OrderItemDTO
                {
                    trnOrderItemId = oi.trnOrderItemsId,
                    trnOrderId = oi.trnOrderId,
                    mstProductId = oi.mstProductId,
                    productName = oi.productName,
                    coverImagePath = oi.coverImagePath != null ? oi.coverImagePath.Replace("\\", "/") : null,
                    quantity = oi.quantity,
                    price = oi.price,
                    size = oi.size,
                    isDelivered = oi.isDelivered,
                    deliveredDate = oi.deliveredDate,
                    orderDate = o.orderDate,
                    orderNumber = o.orderNumber
                }).ToList()
            }).ToList();

            return orders;
        }
        #endregion

        #region Functions for FAQ
        public async Task<ApiResponseDTO> addFaqAsync(AddFaqDTO payload)
        {
            if (string.IsNullOrWhiteSpace(payload.question) || string.IsNullOrWhiteSpace(payload.answer))
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Question and Answer are required." });

            var faq = new mstFAQ
            {
                question = payload.question,
                answer = payload.answer,
                isActive = true,
                createdBy = payload.userId,
                createdDate = DateTime.Now
            };

            _context.mstFAQ.Add(faq);
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "FAQ added successfully." });
        }

        public async Task<ApiResponseDTO> getAllFaqsAsync()
        {
            var faqs = await _context.mstFAQ
                .Where(f => f.isActive)
                .Select(f => new FaqDTO
                {
                    mstFAQId = f.mstFAQId,
                    question = f.question,
                    answer = f.answer,
                    isActive = f.isActive,
                    createdDate = f.createdDate
                })
                .OrderByDescending(f => f.createdDate)
                .ToListAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { data = faqs, message = "FAQs retrieved successfully." });
        }
        public async Task<ApiResponseDTO> getAdminFaqsAsync()
        {
            var faqs = await _context.mstFAQ
                .Select(f => new FaqDTO
                {
                    mstFAQId = f.mstFAQId,
                    question = f.question,
                    answer = f.answer,
                    isActive = f.isActive,
                    createdDate = f.createdDate
                })
                .OrderByDescending(f => f.createdDate)
                .ToListAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { data = faqs, message = "FAQs retrieved successfully." });
        }

        public async Task<ApiResponseDTO> updateFaqStatusAsync(int faqId, bool isActive)
        {
            var faq = await _context.mstFAQ.FindAsync(faqId);
            if (faq == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "FAQ not found." });

            faq.isActive = isActive;
            faq.updatedDate = DateTime.Now;
            _context.mstFAQ.Update(faq);
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "FAQ status updated successfully." });
        }

        public async Task<ApiResponseDTO> updateFaqAsync(UpdateFaqDTO payload)
        {
            if (string.IsNullOrWhiteSpace(payload.question) || string.IsNullOrWhiteSpace(payload.answer))
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Question and Answer are required." });

            var faq = await _context.mstFAQ.FindAsync(payload.mstFAQId);
            if (faq == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "FAQ not found." });

            faq.question = payload.question;
            faq.answer = payload.answer;
            faq.updatedBy = payload.userId;
            faq.updatedDate = DateTime.Now;

            _context.mstFAQ.Update(faq);
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "FAQ updated successfully." });
        }

        public async Task<ApiResponseDTO> deleteFaqAsync(int faqId)
        {
            var faq = await _context.mstFAQ.FindAsync(faqId);
            if (faq == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "FAQ not found." });

            _context.mstFAQ.Remove(faq);
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "FAQ deleted successfully." });
        }
        #endregion
    }
}
