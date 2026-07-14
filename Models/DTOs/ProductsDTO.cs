using System.ComponentModel.DataAnnotations;

namespace hariloom.Models.DTOs
{
    public class ProductMainCategoryDetailsDto
    {
        public int mstProductMainCategoryId { get; set; }
        public string mainCategoryName { get; set; }
        public IFormFile? mainCategoryImageFile { get; set; }
        public string? mainCategoryImagePath { get; set; }
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
    }


    public class ProductSubCategoryDetailsDto
    {
        public int mstProductSubCategoryId { get; set; }
        public int mstProductMainCategoryId { get; set; }
        public string mainCategoryName { get; set; }
        public string subCategoryName { get; set; }
        public IFormFile? subCategoryImageFile { get; set; }
        public string? subCategoryImagePath { get; set; }
        public bool isActive { get; set; }
        public int createdBy { get; set; }
    }

    public class ProductDetailsDTO
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string productDisplayId { get; set; }
        public string productDescription { get; set; }
        public IFormFile coverImage { get; set; }
        public string coverImagePath { get; set; }
        public string coverImageBase64 { get; set; }
        public int basePrice { get; set; }
        public int discountedPrice { get; set; }
        public int quantityAvailable { get; set; }
        public string mstProductGroupingIds { get; set; }
        public string mstProductGroupingNames { get; set; }
        public int mstProductMainCategoryId { get; set; }
        public string mainCategoryName { get; set; }
        public int mstProductSubCategoryId { get; set; }
        public string subCategoryName { get; set; }
        public bool isActive { get; set; }
        public bool isAvailable { get; set; }
        public List<string> tags { get; set; }
        public List<string> productImages { get; set; }
        public List<string> productImagesBase64 { get; set; }
        public List<SpecificationDTO> specifications { get; set; }
        public List<WashCareDTO> washCares { get; set; }
        public List<ColorVariantDTO> colorVariants { get; set; }
        public List<SizeDTO> sizes { get; set; }

    }

    public class ProductColorDTO
    {
        public string colorName { get; set; }
        public List<IFormFile> colorImage { get; set; }
        public List<string> colorImageBase64 { get; set; }
    }

    public class SpecificationDTO
    {
        public string title { get; set; }
        public string value { get; set; }
    }

    public class WashCareDTO
    {
        public string instruction { get; set; }
    }

    public class SizeDTO
    {
        public int trnProductColorId { get; set; }
        public int trnProductSizeId { get; set; }
        public string size { get; set; }
        public int quantityAvailable { get; set; }
    }

    public class ColorVariantDTO
    {
        public int trnProductColorId { get; set; }
        public string colorName { get; set; }
        public string colorCode { get; set; }
        public List<IFormFile> colorImage { get; set; }
        public List<string> colorImageBase64 { get; set; }
        public string colorImageString { get; set; }
        public string availableSizes { get; set; }
    }


    public class CreateProductDTO
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string productDisplayId { get; set; }
        public string productDescription { get; set; }
        public string sizeVariants { get; set; }
        public IFormFile coverImage { get; set; }
        public int basePrice { get; set; }
        public int discountedPrice { get; set; }
        public string productGrouping { get; set; }
        public int mainCategoryId { get; set; }
        public int subCategoryId { get; set; }
        public int quantityAvailable { get; set; }
        public List<IFormFile> productImages { get; set; }
        public List<string> tags { get; set; }
        public List<SpecificationDTO> specifications { get; set; }
        public List<WashCareDTO> washCares { get; set; }
        public List<ColorVariantDTO> colorVariants { get; set; }
        public int createdBy { get; set; }
    }

    public class ProductListCustomFilterDTO
    {
        public string groupingIds { get; set; }
        public string mainCategoryIds { get; set; }
        public string subCategoryIds { get; set; }
    }

    public class ProductHomeDisplayDTO
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string coverImageBase64 { get; set; }
        public int basePrice { get; set; }
        public int discountedPrice { get; set; }
        public string productDescription { get; set; }
    }

    public class BuyNowDTO
    {
        public int mstProductId { get; set; }
        public string size { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string productName { get; set; } = string.Empty;
        public string coverImageBase64 { get; set; } = string.Empty;
        public string coverImagePath { get; set; } = string.Empty;
        public decimal mrp { get; set; }
        public decimal discountedPrice { get; set; }
        public int quantityAvailable { get; set; }
        public List<SizeDTO> sizes { get; set; } = new();
    }
    public class ProductInventoryDTO
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string productDisplayId { get; set; }
        public int trnProductSizeId { get; set; }
        public string size { get; set; }
        public int quantityAvailable { get; set; }
    }

    public class UpdateProductInventoryDTO
    {
        public int trnProductSizeId { get; set; }
        public int quantityAvailable { get; set; }
    }
}
