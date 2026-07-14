using hariloom.Models.DTOs;

namespace hariloom.Interfaces
{
    public interface IApiResponseRepository
    {
        ApiResponseDTO SuccessResponse(ApiResponseDTO responseInfo);
        ApiResponseDTO FailureResponse(ApiResponseDTO responseInfo);
        ApiResponseDTO UnauthorizedResponse(ApiResponseDTO responseInfo);
    }
}
