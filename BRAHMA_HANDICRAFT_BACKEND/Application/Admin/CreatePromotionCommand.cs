using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record CreatePromotionCommand(CreatePromotionRequest Request)
        : IRequest<(CreatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)>;

    public class CreatePromotionCommandHandler
        : IRequestHandler<CreatePromotionCommand, (CreatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IFileUploadService _fileUploadService;

        public CreatePromotionCommandHandler(IPromotionRepository promotionRepository, IFileUploadService fileUploadService)
        {
            _promotionRepository = promotionRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<(CreatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)> Handle(
            CreatePromotionCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var banners = new List<PromotionBannerCreationData>();
            foreach (var bannerRequest in request.Banners)
            {
                var url = await _fileUploadService.SaveProductImageAsync(bannerRequest.File, cancellationToken);
                banners.Add(new PromotionBannerCreationData
                {
                    Url = url,
                    AltText = bannerRequest.AltText
                });
            }

            return await _promotionRepository.CreatePromotionAsync(
                request.Name,
                request.DiscountType,
                request.DiscountValue,
                request.StartDate,
                request.EndDate,
                request.IsActive,
                request.ProductIds,
                banners,
                cancellationToken);
        }
    }
}
