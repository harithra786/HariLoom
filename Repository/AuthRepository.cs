using Microsoft.EntityFrameworkCore;
using hariloom.Helpers.DbContexts;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Entity;
using hariloom.Models.Enums;

namespace hariloom.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly appDBContext _context;
        private readonly IApiResponseRepository _apiResponseRepository;

        public AuthRepository(appDBContext context, IApiResponseRepository apiResponseRepository)
        {
            _context = context;
            _apiResponseRepository = apiResponseRepository;
        }

        public async Task<ApiResponseDTO> userLoginAsync(LoginDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.phoneNumber) || string.IsNullOrWhiteSpace(model.password))
                return _apiResponseRepository.UnauthorizedResponse(new ApiResponseDTO { message = "Phone Number / Email and Password are required." });

            var input = model.phoneNumber.Trim();
            var pass = model.password.Trim();

            var user = await _context.mstUser.FirstOrDefaultAsync(u =>
                u.phoneNumber == input || (u.email != null && u.email.ToLower() == input.ToLower()));

            if (user == null)
            {
                return _apiResponseRepository.UnauthorizedResponse(new ApiResponseDTO { message = "Account not found. Please check your credentials or register." });
            }

            if (!user.isActive)
            {
                return _apiResponseRepository.UnauthorizedResponse(new ApiResponseDTO { message = "Account is inactive. Please contact administrator." });
            }

            if (user.password != pass && user.password != model.password)
            {
                return _apiResponseRepository.UnauthorizedResponse(new ApiResponseDTO { message = "Incorrect password. Please try again." });
            }

            if (string.IsNullOrWhiteSpace(user.ipAddress) && !string.IsNullOrWhiteSpace(model.ipAddress))
            {
                user.ipAddress = model.ipAddress;
            }

            user.visitCount += 1;
            await _context.SaveChangesAsync();

            string defaultUrl = user.accessLevel == (int)accessLevelEnum.AdminUser ? "/Dashboard/AdminDashboard" : "/Website/Home";
            string redirectUrl = defaultUrl;

            if (!string.IsNullOrWhiteSpace(model.returnUrl))
            {
                if (model.returnUrl.StartsWith("/") && !model.returnUrl.StartsWith("//") && !model.returnUrl.StartsWith("/\\"))
                {
                    if (user.accessLevel == (int)accessLevelEnum.AdminUser)
                    {
                        var lowerReturn = model.returnUrl.ToLower();
                        if (lowerReturn.StartsWith("/dashboard") || lowerReturn.StartsWith("/productsmanagement") || lowerReturn.StartsWith("/ordermanagement"))
                        {
                            redirectUrl = model.returnUrl;
                        }
                    }
                    else
                    {
                        redirectUrl = model.returnUrl;
                    }
                }
            }

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Login successful.", data = new { userId = user.mstUserId, name = user.name, accessLevel = user.accessLevel, phoneNumber = user.phoneNumber, email = user.email, url = redirectUrl } });
        }

        public async Task<ApiResponseDTO> userRegisterAsync(RegisterUserDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.name) || string.IsNullOrWhiteSpace(model.phoneNumber) || string.IsNullOrWhiteSpace(model.password))
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Name, Phone Number, and Password are required." });

            if (!string.IsNullOrWhiteSpace(model.email))
            {
                var emailExists = await _context.mstUser.Where(u => u.email == model.email).FirstOrDefaultAsync();
                if (emailExists != null)
                    return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Email ID already in use" });
            }

            var phoneExists = await _context.mstUser.Where(u => u.phoneNumber == model.phoneNumber).FirstOrDefaultAsync();
            if (phoneExists != null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Phone number already exists" });

            var user = new mstUser
            {
                name = model.name,
                phoneNumber = model.phoneNumber,
                password = model.password,
                email = model.email,
                isPromotionalEmailOptIn = model.isPromotionalEmailOptIn,
                ipAddress = model.ipAddress,
                visitCount = 1,
                isActive = true
            };
            await _context.mstUser.AddAsync(user);
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Account created successfully." });
        }

        public async Task<ApiResponseDTO> userForgotPasswordAsync(ForgotPasswordDTO model)
        {

            var user = await _context.mstUser.FirstOrDefaultAsync(u => u.phoneNumber == model.phoneNumber && u.isActive);
            if (user == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "User not found." });

            user.password = model.newPassword; // Hash in production
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Password reset successful." });
        }

        public async Task<ApiResponseDTO> VerifyUserForResetAsync(string phoneNumber, string email)
        {
            var user = await _context.mstUser.FirstOrDefaultAsync(u => u.phoneNumber == phoneNumber && u.isActive);
            if (user == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "User not found." });
            
            if (string.IsNullOrEmpty(user.email) || user.email.ToLower() != email.ToLower())
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Phone number and email do not match." });
            
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "User verified.", data = new { name = user.name } });
        }

        public async Task<ApiResponseDTO> CheckUserExistsAsync(string email, string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailExists = await _context.mstUser.FirstOrDefaultAsync(u => u.email == email);
                if (emailExists != null)
                    return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Email ID already in use" });
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var phoneExists = await _context.mstUser.FirstOrDefaultAsync(u => u.phoneNumber == phoneNumber);
                if (phoneExists != null)
                    return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Phone number already exists" });
            }

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "User does not exist." });
        }
    }
}
