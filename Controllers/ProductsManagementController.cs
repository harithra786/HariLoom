using Microsoft.AspNetCore.Mvc;
using nova_attire.Helpers.Middlewares;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Entity;
using hariloom.Models.Enums;
using hariloom.Repository;

namespace hariloom.Controllers
{
    public class ProductsManagementController : Controller
    {

        #region Interface Implementations
        private readonly IProductsManagementRepository _productsManagementRepository;
        private readonly IApiResponseRepository _apiResponseRepository;

        public ProductsManagementController(IApiResponseRepository apiResponseRepository, IProductsManagementRepository productsManagementRepository)
        {
            _apiResponseRepository = apiResponseRepository;
            _productsManagementRepository = productsManagementRepository;
        }
        #endregion

        #region Product Grouping Page Functionality
        //[RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        //public IActionResult ProductGrouping()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public async Task<ApiResponseDTO> GetAllProductGrouping()
        //{
        //    var categories = await _productsManagementRepository.GetAllProductGroupingAsync();
        //    return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = categories });
        //}

        //[HttpGet]
        //public async Task<ApiResponseDTO> GetAllActiveProductGrouping()
        //{
        //    var categories = await _productsManagementRepository.GetAllActiveProductGroupingAsync();
        //    return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = categories });
        //}

        //[HttpPost]
        //public async Task<ApiResponseDTO> SaveOrUpdateProductGrouping([FromBody] mstProductGrouping model)
        //{
        //    var response = await _productsManagementRepository.SaveOrUpdateProductGroupingAsync(model);
        //    return response;
        //}

        //[HttpGet]
        //public async Task<ApiResponseDTO> GetGroupingById(int id)
        //{
        //    var category = await _productsManagementRepository.GetGroupingByIdAsync(id);
        //    if (category == null)
        //        return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });

        //    return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = category });
        //}

        //[HttpPost]
        //public async Task<ApiResponseDTO> DeactivateProductGrouping(int id, bool isActive, int updatedBy)
        //{
        //    var response = await _productsManagementRepository.DeactivateProductGroupingAsync(id, isActive, updatedBy);
        //    return response;
        //}
        #endregion

        #region Product Main Category Page Functionality
        [RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        public IActionResult ProductMainCategory()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllProductMainCategories()
        {
            var categories = await _productsManagementRepository.GetAllProductMainCategoriesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = categories });
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllActiveProductMainCategories()
        {
            var categories = await _productsManagementRepository.GetAllActiveProductMainCategoriesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = categories });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> SaveOrUpdateProductMainCategory([FromForm] ProductMainCategoryDetailsDto model)
        {
            var response = await _productsManagementRepository.SaveOrUpdateProductMainCategoryAsync(model);
            return response;
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetMainCategoryById(int id)
        {
            var category = await _productsManagementRepository.GetMainCategoryByIdAsync(id);
            if (category == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = category });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> DeactivateProductMainCategory(int id, bool isActive, int updatedBy)
        {
            var response = await _productsManagementRepository.DeactivateProductMainCategoryAsync(id, isActive, updatedBy);
            return response;
        }
        #endregion

        #region Product Sub Category Page Functionality
        [RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        public IActionResult ProductSubCategory()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllProductSubCategories()
        {
            var subCategories = await _productsManagementRepository.GetAllProductSubCategoriesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = subCategories });
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllActiveProductSubCategories()
        {
            var subCategories = await _productsManagementRepository.GetAllActiveProductSubCategoriesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = subCategories });
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllActiveProductSubCategoriesByMainCategoryId(int id)
        {
            var subCategories = await _productsManagementRepository.GetAllActiveProductSubCategoriesByMainCategoryIdAsync(id);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = subCategories });
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllActiveSubCategoryByIDWithImage(int id)
        {
            var subCategories = await _productsManagementRepository.GetAllActiveSubCategoryByIDWithImageAsync(id);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = subCategories });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> SaveOrUpdateProductSubCategory([FromForm] ProductSubCategoryDetailsDto model)
        {
            var response = await _productsManagementRepository.SaveOrUpdateProductSubCategoryAsync(model);
            return response;
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetSubCategoryById(int id)
        {
            var subCategory = await _productsManagementRepository.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = subCategory });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> DeactivateProductSubCategory(int id, bool isActive, int updatedBy)
        {
            var response = await _productsManagementRepository.DeactivateProductSubCategoryAsync(id, isActive, updatedBy);
            return response;
        }
        #endregion

        #region Product Details Page Functionality
        [RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        public IActionResult ProductDetails()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAllProductDetails()
        {
            var products = await _productsManagementRepository.GetAllProductDetailsAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = products });
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetActiveProductsBySubCategoryId(int subCategoryId)
        {
            var products = await _productsManagementRepository.GetActiveProductsBySubCategoryIdAsync(subCategoryId);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = products });
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetProductById(int id)
        {
            var product = await _productsManagementRepository.GetProductByIdAsync(id);

            if (product == null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = product });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> SaveOrUpdateProduct([FromForm] CreateProductDTO model)
        {
            var response = await _productsManagementRepository.SaveOrUpdateProductAsync(model);
            return response;
        }

        [HttpPost]
        public async Task<ApiResponseDTO> DeactivateProduct(int id, bool isActive, int updatedBy)
        {
            var response = await _productsManagementRepository.DeactivateProductAsync(id, isActive, updatedBy);
            return response;
        }

        [HttpPost]
        public async Task<ApiResponseDTO> SetProductAvailability(int id, bool isAvailable, int updatedBy)
        {
            var response = await _productsManagementRepository.SetProductAvailabilityAsync(id, isAvailable, updatedBy);
            return response;
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetAvailableSizesForProduct(int productId)
        {
            var sizes = await _productsManagementRepository.GetAvailableSizesForProductAsync(productId);
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = sizes });
        }
        #endregion

        #region Product Inventory Page Functionality
        [RestrictToAccessLevel((int)accessLevelEnum.AdminUser)]
        public IActionResult ProductInventory()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponseDTO> GetProductInventory()
        {
            var inventory = await _productsManagementRepository.GetProductInventoryAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Fetched Successfully", data = inventory });
        }

        [HttpPost]
        public async Task<ApiResponseDTO> UpdateProductInventory([FromBody] UpdateProductInventoryDTO model)
        {
            var response = await _productsManagementRepository.UpdateProductInventoryAsync(model);
            return response;
        }
        #endregion

    }
}
