using System.Net;
using hariloom.Models.DTOs;
using hariloom.Interfaces;

namespace hariloom.Repository
{
    public class ApiResponseRepository : IApiResponseRepository
    {
        //Standard Success response
        public ApiResponseDTO SuccessResponse(ApiResponseDTO responseInfo)
        {
            ApiResponseDTO responseDTO = new ApiResponseDTO();
            responseDTO.statusCode = (int)HttpStatusCode.OK;
            responseDTO.success = true;
            responseDTO.message = responseInfo.message is null ? "Data fetched successfully" : responseInfo.message;
            responseDTO.data = responseInfo.data;
            return responseDTO;
        }

        //Standard Failure Response
        public ApiResponseDTO FailureResponse(ApiResponseDTO responseInfo)
        {
            ApiResponseDTO responseDTO = new ApiResponseDTO();
            responseDTO.statusCode = (int)HttpStatusCode.InternalServerError;
            responseDTO.success = false;
            responseDTO.message = responseInfo.message;
            responseDTO.data = responseInfo.data;

            return responseDTO;
        }

        //Standard Unauthorized Response
        public ApiResponseDTO UnauthorizedResponse(ApiResponseDTO responseInfo)
        {
            ApiResponseDTO responseDTO = new ApiResponseDTO();
            responseDTO.statusCode = (int)HttpStatusCode.Unauthorized;
            responseDTO.success = false;
            responseDTO.message = "Unauthorized request. Retry Login";
            responseDTO.data = responseInfo.data;
            return responseDTO;
        }
    }
}
