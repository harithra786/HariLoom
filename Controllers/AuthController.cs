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
        public async Task<ApiResponseDTO> CheckUserExists(string email, string phoneNumber)
        {
            return await _authRepository.CheckUserExistsAsync(email, phoneNumber);
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
        public async Task<IActionResult> SendOtpEmail([FromBody] SendOtpRequestDTO request, [FromServices] IEmailService emailService)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
                return BadRequest(new { success = false, message = "Invalid request parameters" });

            try
            {
                var success = await emailService.SendOtpEmailAsync(request.Email, request.UserName ?? "User", request.Otp);
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
