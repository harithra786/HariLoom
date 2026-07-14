using hariloom.Models.DTOs;

namespace hariloom.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApiResponseDTO> userLoginAsync(LoginDTO model);
        Task<ApiResponseDTO> userRegisterAsync(RegisterUserDTO model);
        Task<ApiResponseDTO> userForgotPasswordAsync(ForgotPasswordDTO model);
        Task<ApiResponseDTO> VerifyUserForResetAsync(string phoneNumber, string email);
        Task<ApiResponseDTO> CheckUserExistsAsync(string email, string phoneNumber);
    }
}
