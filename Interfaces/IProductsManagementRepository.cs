using hariloom.Models.DTOs;
using hariloom.Models.Entity;

namespace hariloom.Interfaces
{
    public interface IProductsManagementRepository
    {
        #region Product Main Category Interface Functions
        Task<List<ProductMainCategoryDetailsDto>> GetAllProductMainCategoriesAsync();
        Task<List<ProductMainCategoryDetailsDto>> GetAllActiveProductMainCategoriesAsync();
        Task<ApiResponseDTO> SaveOrUpdateProductMainCategoryAsync(ProductMainCategoryDetailsDto model);
        Task<mstProductMainCategory> GetMainCategoryByIdAsync(int id);
        Task<ApiResponseDTO> DeactivateProductMainCategoryAsync(int id, bool isActive, int updatedBy);
        #endregion

        #region Product Sub Category Interface Functions
        Task<List<ProductSubCategoryDetailsDto>> GetAllProductSubCategoriesAsync();
        Task<List<ProductSubCategoryDetailsDto>> GetAllActiveProductSubCategoriesAsync();
        Task<List<mstProductSubCategory>> GetAllActiveSubCategoryByIDWithImageAsync(int id);
        Task<List<mstProductSubCategory>> GetAllActiveProductSubCategoriesByMainCategoryIdAsync(int id);
        Task<ApiResponseDTO> SaveOrUpdateProductSubCategoryAsync(ProductSubCategoryDetailsDto model);
        Task<mstProductSubCategory> GetSubCategoryByIdAsync(int id);
        Task<ApiResponseDTO> DeactivateProductSubCategoryAsync(int id, bool isActive, int updatedBy);
        #endregion

        #region Products Details Interface Functions
        Task<List<ProductDetailsDTO>> GetAllProductDetailsAsync();
        Task<List<ProductDetailsDTO>> GetAllActiveProductsAsync();
        Task<List<ProductDetailsDTO>> GetActiveProductsByMainCategoryIdAsync(int mainCategoryId);
        Task<List<ProductDetailsDTO>> GetActiveProductsBySubCategoryIdAsync(int subCategoryId);
        Task<ApiResponseDTO> SaveOrUpdateProductAsync(CreateProductDTO model);
        Task<ProductDetailsDTO> GetProductByIdAsync(int id);
        Task<ApiResponseDTO> DeactivateProductAsync(int id, bool isActive, int updatedBy);
        Task<ApiResponseDTO> SetProductAvailabilityAsync(int id, bool isAvailable, int updatedBy);
        Task<List<SizeDTO>> GetAvailableSizesForProductAsync(int productId);
        #endregion

        #region Product Inventory Interface Functions
        Task<List<ProductInventoryDTO>> GetProductInventoryAsync();
        Task<ApiResponseDTO> UpdateProductInventoryAsync(UpdateProductInventoryDTO model);
        #endregion

    }
}
