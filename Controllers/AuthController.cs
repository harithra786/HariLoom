using Microsoft.AspNetCore.Mvc;
using hariloom.Helpers.Middlewares;
using hariloom.Interfaces;
using hariloom.Models.DTOs;

namespace hariloom.Controllers
{
    public class AuthController : Controller
    {
        #region Interface Implementations
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        #endregion

        #region Register Functionalities
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ApiResponseDTO> userRegister(RegisterUserDTO model)
        {
            model.ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            return await _authRepository.userRegisterAsync(model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckUserExists(string email, string phoneNumber)
        {
            try
            {
                var result = await _authRepository.CheckUserExistsAsync(email, phoneNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseDTO { success = false, message = "Server verification error: " + ex.Message });
            }
        }
        #endregion

        #region Login Functionalities
        public IActionResult Login()
        {
            Response.Cookies.Delete("hariloomAuthToken");
            return View();
        }

        [HttpPost]
        public async Task<ApiResponseDTO> userLogin(LoginDTO model)
        {
            var user = await _authRepository.userLoginAsync(model);

            if (user.data != null)
            {

                var tokenPayload = new AuthTokenPayload
                {
                    UserId = user.data.userId,
                    PhoneNumber = user.data.phoneNumber,
                    AccessLevel = user.data.accessLevel
                };

                var encryptedToken = TokenHelper.EncryptToken(tokenPayload);

                Response.Cookies.Append("hariloomAuthToken", encryptedToken, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(8),
                    HttpOnly = false,
                    IsEssential = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });
            }

            return user;
        }
        #endregion

        #region Forget Password Functionalities
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ApiResponseDTO> userForgotPassword(ForgotPasswordDTO model)
        {
            return await _authRepository.userForgotPasswordAsync(model);
        }

        [HttpGet]
        public async Task<ApiResponseDTO> VerifyUserForReset(string phoneNumber, string email)
        {
            return await _authRepository.VerifyUserForResetAsync(phoneNumber, email);
        }
        #endregion

        #region Logout Functinalities
        public IActionResult userLogout()
        {
            Response.Cookies.Delete("hariloomAuthToken");
            return RedirectToAction("Home", "Website");
        }
        #endregion

        #region Email Functionalities
        [HttpPost]
        public async Task<IActionResult> SendOtpEmail([FromBody] SendOtpRequestDTO request, [FromServices] IEmailService emailService, [FromServices] hariloom.Helpers.DbContexts.appDBContext context)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
                return BadRequest(new { success = false, message = "Invalid request parameters" });

            string displayName = request.UserName;
            if (string.IsNullOrWhiteSpace(displayName) || displayName.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(context.mstUser, u => u.email == request.Email || (!string.IsNullOrEmpty(request.PhoneNumber) && u.phoneNumber == request.PhoneNumber));
                if (user != null && !string.IsNullOrWhiteSpace(user.name))
                {
                    displayName = user.name;
                }
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = "Customer";
            }

            try
            {
                var success = await emailService.SendOtpEmailAsync(request.Email, displayName, request.Otp);
                return Ok(new { success = success });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
