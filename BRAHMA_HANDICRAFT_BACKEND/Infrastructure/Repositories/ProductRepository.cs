using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Data;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;

namespace BRAHMA_HANDICRAFT_BACKEND.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public ProductRepository(AppDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<List<ProductByCategoryDto>> GetProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderBy(p => p.CreatedAt).ThenBy(p => p.Id)
                .Select(p => new ProductByCategoryDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive,
                    Designs = p.Designs.Select(d => new DesignSummaryDto
                    {
                        DesignId = d.Id,
                        DesignName = d.DesignName,
                        DesignDescription = d.DesignDescription,
                        EstimatedPrice = d.EstimatedPrice,
                        // Sum across every Size's inventory belonging to this Design.
                        TotalQuantity = d.InventoryStocks.Sum(i => (int?)i.Quantity) ?? 0,
                        // Count of Sizes belonging to this Design.
                        TotalSizes = d.Sizes.Count(),
                        // Prefer the primary-flagged image; among duplicates or when none is primary,
                        // fall back deterministically to the lowest Id.
                        Image = d.ProductImages
                            .OrderByDescending(pi => pi.IsPrimary)
                            .ThenBy(pi => pi.Id)
                            .Select(pi => new ProductImageDto
                            {
                                ImageId = pi.Id,
                                Url = pi.ImageURL,
                                AltText = pi.AltText,
                                IsPrimary = pi.IsPrimary
                            })
                            .FirstOrDefault()
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<ProductSummaryDto> Items, int TotalCount)> GetProductSummariesAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(p => p.CreatedAt).ThenBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductSummaryDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    IsActive = p.IsActive,
                    Category = new CategoryDto { CategoryId = p.CategoryId, CategoryName = p.Category.CategoryName },
                    Designs = p.Designs.Select(d => new DesignSummaryDto
                    {
                        DesignId = d.Id,
                        DesignName = d.DesignName,
                        DesignDescription = d.DesignDescription,
                        EstimatedPrice = d.EstimatedPrice,
                        // Sum across every Size's inventory belonging to this Design.
                        TotalQuantity = d.InventoryStocks.Sum(i => (int?)i.Quantity) ?? 0,
                        // Count of Sizes belonging to this Design.
                        TotalSizes = d.Sizes.Count(),
                        // Prefer the primary-flagged image; among duplicates or when none is primary,
                        // fall back deterministically to the lowest Id.
                        Image = d.ProductImages
                            .OrderByDescending(pi => pi.IsPrimary)
                            .ThenBy(pi => pi.Id)
                            .Select(pi => new ProductImageDto
                            {
                                ImageId = pi.Id,
                                Url = pi.ImageURL,
                                AltText = pi.AltText,
                                IsPrimary = pi.IsPrimary
                            })
                            .FirstOrDefault()
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(List<AdminProductDto> Items, int TotalCount)> GetAdminProductsAsync(
            bool includeInactive,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => includeInactive || p.IsActive)
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new AdminProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    Image = p.Designs
                        .OrderBy(d => d.CreatedAt)
                        .ThenBy(d => d.Id)
                        .Take(1)
                        .SelectMany(d => d.ProductImages)
                        .OrderByDescending(pi => pi.IsPrimary)
                        .ThenBy(pi => pi.Id)
                        .Select(pi => new ProductImageDto
                        {
                            ImageId = pi.Id,
                            Url = pi.ImageURL,
                            AltText = pi.AltText,
                            IsPrimary = pi.IsPrimary
                        })
                        .FirstOrDefault(),
                    TotalQuantity = p.InventoryStocks.Sum(i => (int?)i.Quantity) ?? 0,
                    DesignCount = p.Designs.Count()
                })
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<CreatedProductDto> CreateProductAsync(
            Guid categoryId,
            string categoryName,
            string productName,
            List<DesignCreationData> designs,
            CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productName,
                CategoryId = categoryId,
                IsActive = true
            };
            _context.Products.Add(product);

            var responseDesigns = new List<CreatedProductDesignDto>();

            foreach (var designData in designs)
            {
                var design = new Design
                {
                    Id = Guid.NewGuid(),
                    DesignName = designData.DesignName,
                    DesignDescription = designData.DesignDescription,
                    EstimatedPrice = designData.EstimatedPrice,
                    ProductId = product.Id
                };
                _context.Designs.Add(design);

                var responseImages = new List<ProductImageDto>();
                foreach (var imageData in designData.Images)
                {
                    var image = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ImageURL = imageData.Url,
                        AltText = imageData.AltText,
                        IsPrimary = imageData.IsPrimary,
                        ProductId = product.Id,
                        DesignId = design.Id
                    };
                    _context.ProductImages.Add(image);

                    responseImages.Add(new ProductImageDto
                    {
                        ImageId = image.Id,
                        Url = image.ImageURL,
                        AltText = image.AltText,
                        IsPrimary = image.IsPrimary
                    });
                }

                var responseSizes = new List<SizeWithInventoryDto>();
                foreach (var sizeData in designData.Sizes)
                {
                    var size = new Size
                    {
                        Id = Guid.NewGuid(),
                        SizeName = sizeData.SizeName,
                        Length = sizeData.Length,
                        Breadth = sizeData.Breadth,
                        Height = sizeData.Height,
                        Weight = sizeData.Weight,
                        BasePrice = sizeData.BasePrice,
                        MRP = sizeData.MRP,
                        ProductId = product.Id,
                        DesignId = design.Id
                    };
                    _context.Sizes.Add(size);

                    var inventoryStock = new InventoryStock
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        DesignId = design.Id,
                        SizeId = size.Id,
                        Quantity = sizeData.Quantity,
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.InventoryStocks.Add(inventoryStock);

                    responseSizes.Add(new SizeWithInventoryDto
                    {
                        SizeId = size.Id,
                        SizeName = size.SizeName,
                        Length = size.Length,
                        Breadth = size.Breadth,
                        Height = size.Height,
                        Weight = size.Weight,
                        BasePrice = size.BasePrice,
                        MRP = size.MRP,
                        InventoryStockId = inventoryStock.Id,
                        Quantity = inventoryStock.Quantity
                    });
                }

                responseDesigns.Add(new CreatedProductDesignDto
                {
                    DesignId = design.Id,
                    DesignName = design.DesignName,
                    DesignDescription = design.DesignDescription,
                    EstimatedPrice = design.EstimatedPrice,
                    Images = responseImages,
                    Sizes = responseSizes
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new CreatedProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                IsActive = product.IsActive,
                Category = new CategoryDto { CategoryId = categoryId, CategoryName = categoryName },
                Designs = responseDesigns
            };
        }

        public async Task<List<NewestProductDesignDto>> GetNewestProductDesignsAsync(int count, CancellationToken cancellationToken)
        {
            return await _context.Designs
                .AsNoTracking()
                .Where(d => d.Product.IsActive)
                .OrderByDescending(d => d.CreatedAt)
                .ThenByDescending(d => d.Id)
                .Take(count)
                .Select(d => new NewestProductDesignDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product.Name,
                    CategoryId = d.Product.CategoryId,
                    CategoryName = d.Product.Category.CategoryName,
                    DesignId = d.Id,
                    DesignName = d.DesignName,
                    DesignDescription = d.DesignDescription,
                    EstimatedPrice = d.EstimatedPrice,
                    Image = d.ProductImages
                        .OrderByDescending(pi => pi.IsPrimary)
                        .ThenBy(pi => pi.Id)
                        .Select(pi => new ProductImageDto
                        {
                            ImageId = pi.Id,
                            Url = pi.ImageURL,
                            AltText = pi.AltText,
                            IsPrimary = pi.IsPrimary
                        })
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CreatedProductDto?> GetProductDetailsAsync(Guid productId, CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .Select(p => new CreatedProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    IsActive = p.IsActive,
                    Category = new CategoryDto { CategoryId = p.CategoryId, CategoryName = p.Category.CategoryName },
                    Designs = p.Designs
                        .OrderBy(d => d.CreatedAt)
                        .ThenBy(d => d.Id)
                        .Select(d => new CreatedProductDesignDto
                        {
                            DesignId = d.Id,
                            DesignName = d.DesignName,
                            DesignDescription = d.DesignDescription,
                            EstimatedPrice = d.EstimatedPrice,
                            Images = d.ProductImages
                                .OrderByDescending(pi => pi.IsPrimary)
                                .ThenBy(pi => pi.Id)
                                .Select(pi => new ProductImageDto
                                {
                                    ImageId = pi.Id,
                                    Url = pi.ImageURL,
                                    AltText = pi.AltText,
                                    IsPrimary = pi.IsPrimary
                                }).ToList(),
                            Sizes = d.Sizes
                                .OrderBy(s => s.CreatedAt)
                                .ThenBy(s => s.Id)
                                .Select(s => new SizeWithInventoryDto
                                {
                                    SizeId = s.Id,
                                    SizeName = s.SizeName,
                                    Length = s.Length,
                                    Breadth = s.Breadth,
                                    Height = s.Height,
                                    Weight = s.Weight,
                                    BasePrice = s.BasePrice,
                                    MRP = s.MRP,
                                    InventoryStockId = s.InventoryStocks.Select(i => (Guid?)i.Id).FirstOrDefault(),
                                    Quantity = s.InventoryStocks.Select(i => (int?)i.Quantity).FirstOrDefault()
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(UpdateProductResult Result, CreatedProductDto? Product, string? ErrorMessage)> UpdateProductAsync(
            Guid productId,
            ProductUpdateData data,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Designs).ThenInclude(d => d.Sizes).ThenInclude(s => s.InventoryStocks)
                .Include(p => p.Designs).ThenInclude(d => d.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product is null)
            {
                return (UpdateProductResult.ProductNotFound, null, null);
            }

            var existingDesignIds = product.Designs.Select(d => d.Id).ToHashSet();
            var existingImageIds = product.Designs.SelectMany(d => d.ProductImages).Select(i => i.Id).ToHashSet();
            var existingSizeIds = product.Designs.SelectMany(d => d.Sizes).Select(s => s.Id).ToHashSet();

            if (data.RemoveDesignIds.Any(id => !existingDesignIds.Contains(id)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more removeDesignIds do not belong to this product.");
            }

            if (data.RemoveImageIds.Any(id => !existingImageIds.Contains(id)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more removeImageIds do not belong to this product.");
            }

            if (data.RemoveSizeIds.Any(id => !existingSizeIds.Contains(id)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more removeSizeIds do not belong to this product.");
            }

            if (data.UpdateDesigns.Any(d => !existingDesignIds.Contains(d.DesignId) || data.RemoveDesignIds.Contains(d.DesignId)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more updateDesigns entries reference a design that does not exist or is being removed.");
            }

            if (data.UpdateImages.Any(i => !existingImageIds.Contains(i.ImageId) || data.RemoveImageIds.Contains(i.ImageId)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more updateImages entries reference an image that does not exist or is being removed.");
            }

            if (data.UpdateSizes.Any(s => !existingSizeIds.Contains(s.SizeId) || data.RemoveSizeIds.Contains(s.SizeId)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more updateSizes entries reference a size that does not exist or is being removed.");
            }

            var targetableDesignIds = existingDesignIds.Where(id => !data.RemoveDesignIds.Contains(id)).ToHashSet();

            if (data.NewImages.Any(i => !targetableDesignIds.Contains(i.DesignId)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more newImages entries reference a design that does not exist or is being removed.");
            }

            if (data.NewSizes.Any(s => !targetableDesignIds.Contains(s.DesignId)))
            {
                return (UpdateProductResult.InvalidReference, null, "One or more newSizes entries reference a design that does not exist or is being removed.");
            }

            var filesToDelete = new List<string>();

            // 1. Removals: sizes (+ their inventory) first, then images, then designs (cascading their remaining sizes/images).
            foreach (var sizeId in data.RemoveSizeIds)
            {
                var size = product.Designs.SelectMany(d => d.Sizes).First(s => s.Id == sizeId);
                _context.InventoryStocks.RemoveRange(size.InventoryStocks);
                _context.Sizes.Remove(size);
            }

            foreach (var imageId in data.RemoveImageIds)
            {
                var image = product.Designs.SelectMany(d => d.ProductImages).First(i => i.Id == imageId);
                filesToDelete.Add(image.ImageURL);
                _context.ProductImages.Remove(image);
            }

            foreach (var designId in data.RemoveDesignIds)
            {
                var design = product.Designs.First(d => d.Id == designId);

                foreach (var size in design.Sizes.Where(s => !data.RemoveSizeIds.Contains(s.Id)))
                {
                    _context.InventoryStocks.RemoveRange(size.InventoryStocks);
                    _context.Sizes.Remove(size);
                }

                foreach (var image in design.ProductImages.Where(i => !data.RemoveImageIds.Contains(i.Id)))
                {
                    filesToDelete.Add(image.ImageURL);
                    _context.ProductImages.Remove(image);
                }

                _context.Designs.Remove(design);
            }

            // 2. Updates
            foreach (var designUpdate in data.UpdateDesigns)
            {
                var design = product.Designs.First(d => d.Id == designUpdate.DesignId);
                design.DesignName = designUpdate.DesignName;
                design.DesignDescription = designUpdate.DesignDescription;
                design.EstimatedPrice = designUpdate.EstimatedPrice;
            }

            foreach (var imageUpdate in data.UpdateImages)
            {
                var image = product.Designs.SelectMany(d => d.ProductImages).First(i => i.Id == imageUpdate.ImageId);

                if (!string.IsNullOrEmpty(imageUpdate.NewUrl))
                {
                    filesToDelete.Add(image.ImageURL);
                    image.ImageURL = imageUpdate.NewUrl;
                }

                image.AltText = imageUpdate.AltText;
                image.IsPrimary = imageUpdate.IsPrimary;
            }

            foreach (var sizeUpdate in data.UpdateSizes)
            {
                var size = product.Designs.SelectMany(d => d.Sizes).First(s => s.Id == sizeUpdate.SizeId);
                size.SizeName = sizeUpdate.SizeName;
                size.Length = sizeUpdate.Length;
                size.Breadth = sizeUpdate.Breadth;
                size.Height = sizeUpdate.Height;
                size.Weight = sizeUpdate.Weight;
                size.BasePrice = sizeUpdate.BasePrice;
                size.MRP = sizeUpdate.MRP;

                var inventoryStock = size.InventoryStocks.FirstOrDefault();
                if (inventoryStock is not null)
                {
                    inventoryStock.Quantity = sizeUpdate.Quantity;
                    inventoryStock.LastUpdated = DateTime.UtcNow;
                }
                else
                {
                    _context.InventoryStocks.Add(new InventoryStock
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        DesignId = size.DesignId,
                        SizeId = size.Id,
                        Quantity = sizeUpdate.Quantity,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            // 3. Additions
            foreach (var imageAddition in data.NewImages)
            {
                var design = product.Designs.First(d => d.Id == imageAddition.DesignId);
                var image = new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageURL = imageAddition.Url,
                    AltText = imageAddition.AltText,
                    IsPrimary = imageAddition.IsPrimary,
                    ProductId = product.Id,
                    DesignId = design.Id
                };
                design.ProductImages.Add(image);
                _context.ProductImages.Add(image);
            }

            foreach (var sizeAddition in data.NewSizes)
            {
                var design = product.Designs.First(d => d.Id == sizeAddition.DesignId);
                var size = new Size
                {
                    Id = Guid.NewGuid(),
                    SizeName = sizeAddition.SizeName,
                    Length = sizeAddition.Length,
                    Breadth = sizeAddition.Breadth,
                    Height = sizeAddition.Height,
                    Weight = sizeAddition.Weight,
                    BasePrice = sizeAddition.BasePrice,
                    MRP = sizeAddition.MRP,
                    ProductId = product.Id,
                    DesignId = design.Id
                };
                design.Sizes.Add(size);
                _context.Sizes.Add(size);

                _context.InventoryStocks.Add(new InventoryStock
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    DesignId = design.Id,
                    SizeId = size.Id,
                    Quantity = sizeAddition.Quantity,
                    LastUpdated = DateTime.UtcNow
                });
            }

            foreach (var designData in data.NewDesigns)
            {
                var design = new Design
                {
                    Id = Guid.NewGuid(),
                    DesignName = designData.DesignName,
                    DesignDescription = designData.DesignDescription,
                    EstimatedPrice = designData.EstimatedPrice,
                    ProductId = product.Id
                };

                foreach (var imageData in designData.Images)
                {
                    var image = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ImageURL = imageData.Url,
                        AltText = imageData.AltText,
                        IsPrimary = imageData.IsPrimary,
                        ProductId = product.Id,
                        DesignId = design.Id
                    };
                    design.ProductImages.Add(image);
                    _context.ProductImages.Add(image);
                }

                foreach (var sizeData in designData.Sizes)
                {
                    var size = new Size
                    {
                        Id = Guid.NewGuid(),
                        SizeName = sizeData.SizeName,
                        Length = sizeData.Length,
                        Breadth = sizeData.Breadth,
                        Height = sizeData.Height,
                        Weight = sizeData.Weight,
                        BasePrice = sizeData.BasePrice,
                        MRP = sizeData.MRP,
                        ProductId = product.Id,
                        DesignId = design.Id
                    };
                    design.Sizes.Add(size);
                    _context.Sizes.Add(size);

                    _context.InventoryStocks.Add(new InventoryStock
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        DesignId = design.Id,
                        SizeId = size.Id,
                        Quantity = sizeData.Quantity,
                        LastUpdated = DateTime.UtcNow
                    });
                }

                product.Designs.Add(design);
                _context.Designs.Add(design);
            }

            // 4. Top-level fields
            if (!string.IsNullOrWhiteSpace(data.ProductName))
            {
                product.Name = data.ProductName;
            }

            if (data.CategoryId.HasValue)
            {
                product.CategoryId = data.CategoryId.Value;
            }

            if (data.IsActive.HasValue)
            {
                product.IsActive = data.IsActive.Value;
            }

            // 5. Final invariant check: product needs >=1 design, every remaining design needs >=1 size.
            var finalDesigns = product.Designs.Where(d => !data.RemoveDesignIds.Contains(d.Id)).ToList();

            if (finalDesigns.Count == 0)
            {
                return (UpdateProductResult.WouldHaveNoDesigns, null, "Product must have at least one design remaining.");
            }

            foreach (var design in finalDesigns)
            {
                var finalSizeCount = design.Sizes.Count(s => !data.RemoveSizeIds.Contains(s.Id));
                if (finalSizeCount == 0)
                {
                    return (UpdateProductResult.DesignWouldHaveNoSizes, null, $"Design '{design.DesignName}' would have no sizes remaining.");
                }
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                return (UpdateProductResult.ReferencedElsewhere, null,
                    "One or more items could not be removed because they are referenced by existing cart, wishlist, or order data.");
            }

            foreach (var url in filesToDelete)
            {
                _fileUploadService.DeleteProductImage(url);
            }

            var categoryName = data.CategoryName ?? product.Category.CategoryName;

            var responseDesigns = finalDesigns.Select(d => new CreatedProductDesignDto
            {
                DesignId = d.Id,
                DesignName = d.DesignName,
                DesignDescription = d.DesignDescription,
                EstimatedPrice = d.EstimatedPrice,
                Images = d.ProductImages
                    .Where(i => !data.RemoveImageIds.Contains(i.Id))
                    .Select(i => new ProductImageDto
                    {
                        ImageId = i.Id,
                        Url = i.ImageURL,
                        AltText = i.AltText,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                Sizes = d.Sizes
                    .Where(s => !data.RemoveSizeIds.Contains(s.Id))
                    .Select(s => new SizeWithInventoryDto
                    {
                        SizeId = s.Id,
                        SizeName = s.SizeName,
                        Length = s.Length,
                        Breadth = s.Breadth,
                        Height = s.Height,
                        Weight = s.Weight,
                        BasePrice = s.BasePrice,
                        MRP = s.MRP,
                        InventoryStockId = s.InventoryStocks.Select(i => (Guid?)i.Id).FirstOrDefault(),
                        Quantity = s.InventoryStocks.Select(i => (int?)i.Quantity).FirstOrDefault()
                    }).ToList()
            }).ToList();

            return (UpdateProductResult.Success, new CreatedProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                IsActive = product.IsActive,
                Category = new CategoryDto { CategoryId = product.CategoryId, CategoryName = categoryName },
                Designs = responseDesigns
            }, null);
        }
    }
}
