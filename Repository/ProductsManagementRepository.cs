using Microsoft.EntityFrameworkCore;
using hariloom.Helpers.DbContexts;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using hariloom.Models.Entity;
using System.Drawing;

namespace hariloom.Repository
{
    public class ProductsManagementRepository : IProductsManagementRepository
    {
        #region Interface and Repository Implementations       
        private readonly appDBContext _context;
        private readonly IApiResponseRepository _apiResponseRepository;
        public ProductsManagementRepository(appDBContext context, IApiResponseRepository apiResponseRepository)
        {
            _context = context;
            _apiResponseRepository = apiResponseRepository;
        }
        #endregion

        #region Repository Implementations of Product Grouping
        //public async Task<List<mstProductGrouping>> GetAllProductGroupingAsync()
        //{
        //    return await _context.mstProductGrouping.ToListAsync();
        //}

        //public async Task<List<mstProductGrouping>> GetAllActiveProductGroupingAsync()
        //{
        //    return await _context.mstProductGrouping.Where(x => x.isActive).ToListAsync();
        //}

        //public async Task<ApiResponseDTO> SaveOrUpdateProductGroupingAsync(mstProductGrouping model)
        //{
        //    var checkforDuplicate = _context.mstProductGrouping.Where(x => x.groupingName == model.groupingName && x.isActive && x.mstProductGroupingId != model.mstProductGroupingId).FirstOrDefault();
        //    if (checkforDuplicate != null)
        //    {
        //        return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Duplicate Already Exists" });
        //    }

        //    if (model.mstProductGroupingId == 0)
        //    {
        //        model.groupingName = model.groupingName;
        //        await _context.mstProductGrouping.AddAsync(model);
        //    }
        //    else
        //    {
        //        var existing = await _context.mstProductGrouping.FindAsync(model.mstProductGroupingId);
        //        if (existing == null)
        //        {
        //            return new ApiResponseDTO { success = false, message = "Record Not Found", statusCode = 404 };
        //        }
        //        existing.groupingName = model.groupingName;
        //        existing.updatedBy = model.updatedBy;
        //        existing.updatedDate = DateTime.UtcNow;
        //    }
        //    await _context.SaveChangesAsync();
        //    return new ApiResponseDTO { success = true, message = "Saved Successfully", statusCode = 200 };
        //}

        //public async Task<mstProductGrouping> GetGroupingByIdAsync(int id)
        //{
        //    return await _context.mstProductGrouping.FindAsync(id);
        //}

        //public async Task<ApiResponseDTO> DeactivateProductGroupingAsync(int id, bool isActive, int updatedBy)
        //{
        //    var grouping = await _context.mstProductGrouping.FindAsync(id);
        //    if (grouping == null)
        //    {
        //        return new ApiResponseDTO { success = false, message = "Record Not Found", statusCode = 404 };
        //    }
        //    grouping.isActive = isActive;
        //    grouping.updatedBy = updatedBy;
        //    grouping.updatedDate = DateTime.Now;
        //    await _context.SaveChangesAsync();
        //    return new ApiResponseDTO { success = true, message = "Status Updated", statusCode = 200 };
        //}
        #endregion

        #region Repository Implementation of Product Main Category
        public async Task<List<ProductMainCategoryDetailsDto>> GetAllProductMainCategoriesAsync()
        {
            var productList = await _context.mstProductMainCategory.Select(p => new ProductMainCategoryDetailsDto
            {
                mstProductMainCategoryId = p.mstProductMainCategoryId,
                mainCategoryName = p.mainCategoryName,
                mainCategoryImagePath = p.mainCategoryImagePath,
                isActive = p.isActive,
                createdBy = p.createdBy
            }).ToListAsync();

            var returnResult = productList.Select(p => new ProductMainCategoryDetailsDto
            {
                mstProductMainCategoryId = p.mstProductMainCategoryId,
                mainCategoryName = p.mainCategoryName,
                mainCategoryImagePath = GetImageFromPathAndConvertToBase64(p.mainCategoryImagePath),
                isActive = p.isActive,
                createdBy = p.createdBy
            }).ToList();

            return returnResult;
        }

        public async Task<List<ProductMainCategoryDetailsDto>> GetAllActiveProductMainCategoriesAsync()
        {
            var productList = await _context.mstProductMainCategory.Where(a => a.isActive == true).Select(p => new ProductMainCategoryDetailsDto
            {
                mstProductMainCategoryId = p.mstProductMainCategoryId,
                mainCategoryName = p.mainCategoryName,
                mainCategoryImagePath = p.mainCategoryImagePath,
                isActive = p.isActive,
                createdBy = p.createdBy
            }).ToListAsync();

            var returnResult = productList.Select(p => new ProductMainCategoryDetailsDto
            {
                mstProductMainCategoryId = p.mstProductMainCategoryId,
                mainCategoryName = p.mainCategoryName,
                mainCategoryImagePath = GetImageFromPathAndConvertToBase64(p.mainCategoryImagePath),
                isActive = p.isActive,
                createdBy = p.createdBy
            }).ToList();

            return returnResult;
        }


        public async Task<ApiResponseDTO> SaveOrUpdateProductMainCategoryAsync(ProductMainCategoryDetailsDto model)
        {
            var checkforDuplicate = _context.mstProductMainCategory.Where(x => x.mainCategoryName == model.mainCategoryName && x.isActive && x.mstProductMainCategoryId != model.mstProductMainCategoryId).FirstOrDefault();
            if (checkforDuplicate != null)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Duplicate Already Exists" });
            }

            if (model.mstProductMainCategoryId > 0)
            {
                var entity = await _context.mstProductMainCategory.FirstOrDefaultAsync(x => x.mstProductMainCategoryId == model.mstProductMainCategoryId);
                if (entity != null)
                {
                    entity.mainCategoryName = model.mainCategoryName;
                    entity.isActive = model.isActive;
                    entity.updatedBy = model.createdBy;
                    entity.updatedDate = DateTime.Now;

                    if (model.mainCategoryImageFile != null)
                    {
                        entity.mainCategoryImagePath = await SaveMainCategoryImageAsync(model.mainCategoryImageFile);
                    }

                    _context.SaveChanges();
                    return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Updated Successfully" });
                }
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });
            }
            else
            {
                mstProductMainCategory newCategory = new mstProductMainCategory();
                newCategory.mainCategoryName = model.mainCategoryName;

                if (model.mainCategoryImageFile != null)
                {
                    newCategory.mainCategoryImagePath = await SaveMainCategoryImageAsync(model.mainCategoryImageFile);
                }
                await _context.mstProductMainCategory.AddAsync(newCategory);
                _context.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Saved Successfully" });
            }
        }

        private async Task<string> SaveMainCategoryImageAsync(IFormFile image)
        {
            if (image == null) return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "maincategories");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return relative path for use in <img src="...">
            return filePath.Replace("\\", "/");
        }

        public async Task<mstProductMainCategory> GetMainCategoryByIdAsync(int id)
        {
            return await _context.mstProductMainCategory.FirstOrDefaultAsync(x => x.mstProductMainCategoryId == id);
        }

        public async Task<ApiResponseDTO> DeactivateProductMainCategoryAsync(int id, bool isActive, int updatedBy)
        {
            var entity = await _context.mstProductMainCategory.FirstOrDefaultAsync(x => x.mstProductMainCategoryId == id);
            if (entity != null)
            {
                entity.isActive = isActive;
                entity.updatedBy = updatedBy;
                entity.updatedDate = DateTime.Now;
                _context.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Updated Successfully" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });
        }
        #endregion

        #region Repository Implementation of Product Sub Category
        public async Task<List<ProductSubCategoryDetailsDto>> GetAllProductSubCategoriesAsync()
        {
            var subCategories = await _context.mstProductSubCategory.Include(x => x.MainCategory).Where(x => x.MainCategory.isActive).ToListAsync();

            return subCategories.Select(x => new ProductSubCategoryDetailsDto
            {
                mstProductSubCategoryId = x.mstProductSubCategoryId,
                mstProductMainCategoryId = x.mstProductMainCategoryId,
                mainCategoryName = x.MainCategory.mainCategoryName,
                subCategoryName = x.subCategoryName,
                subCategoryImagePath = GetImageFromPathAndConvertToBase64(x.subCategoryImagePath),
                isActive = x.isActive,
                createdBy = x.createdBy
            }).ToList();
        }


        public async Task<List<ProductSubCategoryDetailsDto>> GetAllActiveProductSubCategoriesAsync()
        {
            var subCategories = await _context.mstProductSubCategory.Include(x => x.MainCategory).Where(x => x.MainCategory.isActive).Where(x => x.isActive).ToListAsync();

            return subCategories.Select(x => new ProductSubCategoryDetailsDto
            {
                mstProductSubCategoryId = x.mstProductSubCategoryId,
                mstProductMainCategoryId = x.mstProductMainCategoryId,
                mainCategoryName = x.MainCategory.mainCategoryName,
                subCategoryName = x.subCategoryName,
                subCategoryImagePath = GetImageFromPathAndConvertToBase64(x.subCategoryImagePath),
                isActive = x.isActive,
                createdBy = x.createdBy
            }).ToList();
        }


        public async Task<List<mstProductSubCategory>> GetAllActiveProductSubCategoriesByMainCategoryIdAsync(int id)
        {
            return await _context.mstProductSubCategory.Where(a => a.isActive && a.mstProductMainCategoryId == id).ToListAsync();
        }

        public async Task<List<mstProductSubCategory>> GetAllActiveSubCategoryByIDWithImageAsync(int id)
        {
            var subCategories = await _context.mstProductSubCategory.Where(x => x.MainCategory.isActive).Where(x => x.isActive && x.mstProductMainCategoryId == id).ToListAsync();

            return subCategories.Select(x => new mstProductSubCategory
            {
                mstProductSubCategoryId = x.mstProductSubCategoryId,
                mstProductMainCategoryId = x.mstProductMainCategoryId,
                subCategoryName = x.subCategoryName,
                subCategoryImagePath = GetImageFromPathAndConvertToBase64(x.subCategoryImagePath),
                isActive = x.isActive,
                createdBy = x.createdBy
            }).ToList();
        }

        public async Task<ApiResponseDTO> SaveOrUpdateProductSubCategoryAsync(ProductSubCategoryDetailsDto model)
        {
            var duplicate = _context.mstProductSubCategory.FirstOrDefault(x => x.subCategoryName == model.subCategoryName && x.mstProductMainCategoryId == model.mstProductMainCategoryId && x.isActive && x.mstProductSubCategoryId != model.mstProductSubCategoryId);

            if (duplicate != null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Duplicate Already Exists" });

            if (model.mstProductSubCategoryId > 0)
            {
                var entity = await _context.mstProductSubCategory.FirstOrDefaultAsync(x => x.mstProductSubCategoryId == model.mstProductSubCategoryId);
                if (entity != null)
                {
                    entity.subCategoryName = model.subCategoryName;
                    entity.mstProductMainCategoryId = model.mstProductMainCategoryId;
                    entity.updatedBy = model.createdBy;
                    entity.updatedDate = DateTime.Now;

                    if (model.subCategoryImageFile != null)
                    {
                        entity.subCategoryImagePath = await SaveSubCategoryImageAsync(model.subCategoryImageFile);
                    }

                    await _context.SaveChangesAsync();
                    return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Updated Successfully" });
                }
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });
            }
            else
            {
                var entity = new mstProductSubCategory
                {
                    subCategoryName = model.subCategoryName,
                    mstProductMainCategoryId = model.mstProductMainCategoryId,
                };

                if (model.subCategoryImageFile != null)
                {
                    entity.subCategoryImagePath = await SaveSubCategoryImageAsync(model.subCategoryImageFile);
                }

                await _context.mstProductSubCategory.AddAsync(entity);
                await _context.SaveChangesAsync();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Saved Successfully" });
            }
        }

        private async Task<string> SaveSubCategoryImageAsync(IFormFile image)
        {
            if (image == null) return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "subcategories");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return filePath.Replace("\\", "/");
        }

        public async Task<mstProductSubCategory> GetSubCategoryByIdAsync(int id)
        {
            return await _context.mstProductSubCategory.FirstOrDefaultAsync(x => x.mstProductSubCategoryId == id);
        }

        public async Task<ApiResponseDTO> DeactivateProductSubCategoryAsync(int id, bool isActive, int updatedBy)
        {
            var entity = await _context.mstProductSubCategory.FirstOrDefaultAsync(x => x.mstProductSubCategoryId == id);
            if (entity != null)
            {
                entity.isActive = isActive;
                entity.updatedBy = updatedBy;
                entity.updatedDate = DateTime.Now;
                _context.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Updated Successfully" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });
        }
        #endregion

        #region Repository Implementation of Product Details

        #region Get All Product Details 
        public async Task<List<ProductDetailsDTO>> GetAllProductDetailsAsync()
        {
            var productList = await _context.mstProduct.OrderByDescending(p => p.createdDate).Select(p => new ProductDetailsDTO
            {
                productId = p.mstProductId,
                productName = p.productName,
                productDisplayId = p.productDisplayId,
                productDescription = p.description,
                basePrice = p.basePrice,
                coverImagePath = p.coverImagePath != null ? p.coverImagePath.Replace("\\", "/") : null,
                isActive = p.isActive,
                isAvailable = p.isAvailable
            }).ToListAsync();

            // Base64 conversion removed for performance reasons. The frontend will use coverImagePath directly.

            return productList;
        }
        #endregion

        #region Get Active Products By Sub Category Id
        public async Task<List<ProductDetailsDTO>> GetActiveProductsBySubCategoryIdAsync(int subCategoryId)
        {
            var productList = await _context.mstProduct
                .Where(p => p.isActive && p.isAvailable && p.mstProductSubCategoryId == subCategoryId)
                .OrderByDescending(p => p.createdDate)
                .Select(p => new ProductDetailsDTO
                {
                    productId = p.mstProductId,
                    productName = p.productName,
                    productDisplayId = p.productDisplayId,
                    productDescription = p.description,
                    basePrice = p.basePrice,
                    discountedPrice = p.discountedPrice ?? 0,
                    coverImagePath = p.coverImagePath != null ? p.coverImagePath.Replace("\\", "/") : null,
                    isActive = p.isActive,
                    isAvailable = p.isAvailable
                }).ToListAsync();

            return productList;
        }
        #endregion

        #region Get All Active Product Details By Grouping Name
        //public async Task<List<ProductDetailsDTO>> GetAllActiveProductDetailsByGroupingNameAsync(string groupingName)
        //{
        //    if (string.IsNullOrWhiteSpace(groupingName))
        //        return new List<ProductDetailsDTO>();

        //    // 1. Get the matching grouping by name (case-insensitive)
        //    var grouping = await _context.mstProductGrouping.FirstOrDefaultAsync(x => x.isActive && x.groupingName.ToLower() == groupingName.ToLower());

        //    if (grouping == null)
        //        return new List<ProductDetailsDTO>(); // No match found

        //    int targetGroupingId = grouping.mstProductGroupingId;

        //    // 2. Get only the columns we need and coalesce string columns to avoid DBNull -> string casting issues
        //    var productList = await _context.mstProduct
        //        .Where(p => p.isActive && p.isAvailable && p.mstProductGroupingIds != null)
        //        .Select(p => new
        //        {
        //            p.mstProductId,
        //            productName = p.productName ?? string.Empty,
        //            mstProductGroupingIds = p.mstProductGroupingIds ?? string.Empty,
        //            p.basePrice,
        //            p.discountedPrice,
        //            coverImagePath = p.coverImagePath ?? string.Empty
        //        })
        //        .ToListAsync();

        //    // 3. Filter products that contain the target grouping ID (do client-side parsing now that strings are non-null)
        //    var filteredProducts = productList.Where(p => p.mstProductGroupingIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => int.Parse(id)).Contains(targetGroupingId)).ToList();

        //    // 4. Return mapped DTOs
        //    var returnResult = filteredProducts.Select(p =>
        //    {
        //        var productGroupingIds = p.mstProductGroupingIds.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        //        return new ProductDetailsDTO
        //        {
        //            productId = p.mstProductId,
        //            productName = p.productName,
        //            basePrice = p.basePrice,
        //            discountedPrice = p.discountedPrice,
        //            coverImageBase64 = GetImageFromPathAndConvertToBase64(p.coverImagePath),
        //        };

        //    }).ToList();

        //    return returnResult;
        //}
        #endregion

        #region Get Product Details By Id
        public async Task<ProductDetailsDTO?> GetProductByIdAsync(int id)
        {
            var p = await _context.mstProduct.FirstOrDefaultAsync(x => x.mstProductId == id);
            if (p == null) return null;

            var specifications = await _context.trnProductSpecification.Where(s => s.mstProductId == id && s.isActive).Select(s => new SpecificationDTO
            {
                title = s.title,
                value = s.value
            }).ToListAsync();

            var washCares = new List<WashCareDTO>();
            if (!string.IsNullOrEmpty(p.washCareInstructions))
            {
                var instructions = p.washCareInstructions.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach(var inst in instructions)
                {
                    washCares.Add(new WashCareDTO { instruction = inst });
                }
            }

            var sizes = await _context.trnProductSize
                .Where(s => s.mstProductId == id && s.isActive)
                .GroupBy(s => s.size)
                .Select(g => new SizeDTO
                {
                    trnProductSizeId = g.FirstOrDefault().trnProductSizeId,
                    size = g.Key,
                    quantityAvailable = g.Sum(x => x.quantityAvailable)
                }).ToListAsync();

            var productImages = new List<string>();
            if (!string.IsNullOrEmpty(p.productImages))
            {
                productImages = p.productImages.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                               .Select(img => img.Replace("\\", "/"))
                                               .ToList();
            }

            return new ProductDetailsDTO
            {
                productId = p.mstProductId,
                productName = p.productName,
                productDisplayId = p.productDisplayId,
                productDescription = p.description,
                basePrice = p.basePrice,
                discountedPrice = p.discountedPrice ?? 0,
                mstProductGroupingIds = p.mstProductGroupingIds,
                mstProductMainCategoryId = p.mstProductMainCategoryId,
                mstProductSubCategoryId = p.mstProductSubCategoryId,
                coverImageBase64 = string.Empty, // Deprecated, use coverImagePath
                coverImagePath = p.coverImagePath != null ? p.coverImagePath.Replace("\\", "/") : null,
                isActive = p.isActive,
                isAvailable = p.isAvailable,
                productImages = productImages,
                productImagesBase64 = new List<string>(), // Deprecated, use productImages
                specifications = specifications,
                washCares = washCares,
                sizes = sizes,
                quantityAvailable = p.quantityAvailable
            };
        }
        #endregion

        #region Save or Update Product Details
        public async Task<ApiResponseDTO> SaveOrUpdateProductAsync(CreateProductDTO model)
        {
            var duplicate = _context.mstProduct.FirstOrDefault(x => x.productName == model.productName && x.productDisplayId == model.productDisplayId && x.mstProductId != model.productId);

            if (duplicate != null)
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Duplicate Product Name" });

            mstProduct entity;
            if (model.productId > 0)
            {
                entity = await _context.mstProduct.FirstOrDefaultAsync(x => x.mstProductId == model.productId);
                if (entity == null)
                    return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });

                entity.productName = model.productName;
                entity.productDisplayId = model.productDisplayId;
                entity.description = model.productDescription;
                if (model.coverImage != null)
                    entity.coverImagePath = await SaveProductImagesAsync(model.coverImage);

                entity.basePrice = model.basePrice;
                entity.discountedPrice = model.discountedPrice;
                entity.mstProductGroupingIds = model.productGrouping ?? "";
                entity.mstProductMainCategoryId = model.mstProductMainCategoryId;
                entity.mstProductSubCategoryId = model.mstProductSubCategoryId;
                entity.quantityAvailable = model.quantityAvailable;

                if (model.washCares != null && model.washCares.Count > 0)
                {
                    var instructions = model.washCares.Select(w => w.instruction).Where(i => !string.IsNullOrWhiteSpace(i));
                    entity.washCareInstructions = string.Join("|", instructions);
                }
                else
                {
                    entity.washCareInstructions = null;
                }

                if (model.productImages != null && model.productImages.Count > 0)
                {
                    var imagePaths = new List<string>();
                    foreach (var productImage in model.productImages)
                    {
                        if (productImage != null && productImage.Length > 0)
                        {
                            var imagePath = await SaveProductImagesAsync(productImage);
                            imagePaths.Add(imagePath);
                        }
                    }
                    entity.productImages = string.Join(",", imagePaths);
                }
                entity.updatedBy = model.createdBy;
                entity.updatedDate = DateTime.Now;
            }
            else
            {
                var imagePaths = new List<string>();
                if (model.productImages != null && model.productImages.Count > 0)
                {
                    foreach (var productImage in model.productImages)
                    {
                        if (productImage != null && productImage.Length > 0)
                        {
                            var imagePath = await SaveProductImagesAsync(productImage);
                            imagePaths.Add(imagePath);
                        }
                    }
                }

                entity = new mstProduct
                {
                    productName = model.productName,
                    productDisplayId = model.productDisplayId,
                    description = model.productDescription,
                    coverImagePath = await SaveProductImagesAsync(model.coverImage),
                    basePrice = model.basePrice,
                    discountedPrice = model.discountedPrice,
                    mstProductGroupingIds = model.productGrouping ?? "",
                    mstProductMainCategoryId = model.mstProductMainCategoryId,
                    mstProductSubCategoryId = model.mstProductSubCategoryId,
                    quantityAvailable = model.quantityAvailable,
                    productImages = string.Join(",", imagePaths),
                    washCareInstructions = model.washCares != null && model.washCares.Count > 0 
                                            ? string.Join("|", model.washCares.Select(w => w.instruction).Where(i => !string.IsNullOrWhiteSpace(i))) 
                                            : null,
                    createdBy = model.createdBy
                };

                // ensure the entity flags are set explicitly before add
                await _context.mstProduct.AddAsync(entity);
                await _context.SaveChangesAsync();
                // refresh entity values from DB to ensure identity and defaults are populated
                await _context.Entry(entity).ReloadAsync();
                model.productId = entity.mstProductId;
                System.Diagnostics.Debug.WriteLine($"DEBUG SaveOrUpdateProductAsync - After initial Add, new id: {entity.mstProductId}, createdDate: {entity.createdDate}, isActive: {entity.isActive}");
                try {
                    var fromDb = await _context.mstProduct.AsNoTracking().FirstOrDefaultAsync(x => x.mstProductId == entity.mstProductId);
                    System.Diagnostics.Debug.WriteLine($"DEBUG SaveOrUpdateProductAsync - verify exists immediately after add: {fromDb != null}");
                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"ERROR verifying product after Add: {ex.Message}");
                }
            }

            var oldSpecs = _context.trnProductSpecification.Where(s => s.mstProductId == entity.mstProductId && s.isActive).ToList();
            foreach (var spec in oldSpecs)
            {
                spec.isActive = false;
                spec.deletedBy = model.createdBy;
                spec.deletedDate = DateTime.Now;
            }

            if (model.specifications != null)
            {
                foreach (var spec in model.specifications)
                {
                    await _context.trnProductSpecification.AddAsync(new trnProductSpecification
                    {
                        mstProductId = entity.mstProductId,
                        title = spec.title,
                        value = spec.value,
                        createdBy = model.createdBy
                    });
                }
            }

            // Wash cares handled above

            var inputSizes = new List<string>();
            if (!string.IsNullOrEmpty(model.sizeVariants))
            {
                inputSizes = model.sizeVariants.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            }

            var allDbSizes = await _context.trnProductSize.Where(s => s.mstProductId == entity.mstProductId).ToListAsync();

            // Deactivate sizes not in input, reactivate existing ones
            foreach (var dbSize in allDbSizes)
            {
                if (!inputSizes.Contains(dbSize.size))
                {
                    if (dbSize.isActive)
                    {
                        dbSize.isActive = false;
                        dbSize.deletedBy = model.createdBy;
                        dbSize.deletedDate = DateTime.Now;
                    }
                }
                else
                {
                    if (!dbSize.isActive)
                    {
                        dbSize.isActive = true;
                        dbSize.updatedBy = model.createdBy;
                        dbSize.updatedDate = DateTime.Now;
                    }
                }
            }

            // Add new sizes
            var existingSizes = allDbSizes.Select(s => s.size).ToList();
            var newSizes = inputSizes.Except(existingSizes).ToList();

            foreach (var size in newSizes)
            {
                await _context.trnProductSize.AddAsync(new trnProductSize
                {
                    mstProductId = entity.mstProductId,
                    size = size,
                    createdBy = model.createdBy
                });
            }
            await _context.SaveChangesAsync();
            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = model.productId > 0 ? "Updated Successfully" : "Saved Successfully" });
        }
        #endregion

        #region Deactivate Product
        public async Task<ApiResponseDTO> DeactivateProductAsync(int id, bool isActive, int updatedBy)
        {
            var entity = await _context.mstProduct.FirstOrDefaultAsync(x => x.mstProductId == id);
            if (entity != null)
            {
                entity.isActive = isActive;
                entity.updatedBy = updatedBy;
                entity.updatedDate = DateTime.Now;
                _context.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Updated Successfully" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });
        }
        #endregion

        #region Set Product Availability
        public async Task<ApiResponseDTO> SetProductAvailabilityAsync(int id, bool isAvailable, int updatedBy)
        {
            var entity = await _context.mstProduct.FirstOrDefaultAsync(x => x.mstProductId == id);
            if (entity != null)
            {
                entity.isAvailable = isAvailable;
                entity.updatedBy = updatedBy;
                entity.updatedDate = DateTime.Now;
                _context.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Updated Successfully" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Record Not Found" });
        }
        #endregion

        /// <summary>
        /// Resolves an image path (absolute or relative) to a full filesystem path.
        /// Handles both legacy absolute paths and new wwwroot-relative paths.
        /// </summary>
        private string ResolveImagePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return null;

            // If the path exists as-is (absolute path on same machine), use it directly
            if (File.Exists(imagePath))
                return imagePath;

            // Try resolving relative to wwwroot (handles paths like "images/guid.jpg")
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var resolvedPath = Path.Combine(wwwrootPath, imagePath.TrimStart('/', '\\'));
            if (File.Exists(resolvedPath))
                return resolvedPath;

            // Try extracting the relative portion from an old absolute path
            // e.g. "C:/old-server/wwwroot/images/guid.jpg" -> resolve "images/guid.jpg" against current wwwroot
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

        private string GetImageFromPathAndConvertToBase64(string imagePath)
        {
            var resolved = ResolveImagePath(imagePath);
            if (resolved == null)
                return string.Empty;

            return Convert.ToBase64String(File.ReadAllBytes(resolved));
        }

        private List<string> GetImageFromPathAndConvertToListBase64(string imagePaths)
        {
            var base64List = new List<string>();

            if (!string.IsNullOrWhiteSpace(imagePaths))
            {
                var paths = imagePaths.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var path in paths)
                {
                    var resolved = ResolveImagePath(path.Trim());
                    if (resolved != null)
                    {
                        var imageBytes = File.ReadAllBytes(resolved);
                        var base64 = Convert.ToBase64String(imageBytes);
                        base64List.Add(base64);
                    }
                }
            }

            return base64List;
        }


        private async Task<string> SaveProductImagesAsync(IFormFile image)
        {
            if (image == null) return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Store as a relative path (portable across environments)
            return $"images/{fileName}";
        }



        public async Task<List<SizeDTO>> GetAvailableSizesForProductAsync(int productId)
        {
            var sizes = await _context.trnProductSize
                .Where(s => s.mstProductId == productId && s.isActive)
                .GroupBy(s => s.size)
                .Select(g => new SizeDTO
                {
                    trnProductSizeId = g.FirstOrDefault().trnProductSizeId,
                    size = g.Key,
                    quantityAvailable = g.Sum(x => x.quantityAvailable)
                })
                .OrderBy(s => s.size)
                .ToListAsync();

            return sizes;
        }
        #endregion

        #region Product Inventory
        public async Task<List<ProductInventoryDTO>> GetProductInventoryAsync()
        {
            var inventory = await _context.trnProductSize
                .Include(s => s.Product)
                .Where(s => s.isActive && s.Product.isActive)
                .Select(s => new ProductInventoryDTO
                {
                    productId = s.mstProductId,
                    productName = s.Product.productName,
                    productDisplayId = s.Product.productDisplayId,
                    trnProductSizeId = s.trnProductSizeId,
                    size = s.size,
                    quantityAvailable = s.quantityAvailable
                })
                .ToListAsync();

            return inventory;
        }

        public async Task<ApiResponseDTO> UpdateProductInventoryAsync(UpdateProductInventoryDTO model)
        {
            var productSize = await _context.trnProductSize.FirstOrDefaultAsync(s => s.trnProductSizeId == model.trnProductSizeId);
            if (productSize == null)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Size variant not found" });
            }

            productSize.quantityAvailable = model.quantityAvailable;
            await _context.SaveChangesAsync();

            return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Inventory updated successfully" });
        }
        #endregion

    }
}
