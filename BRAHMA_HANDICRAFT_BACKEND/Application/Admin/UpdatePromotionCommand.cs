using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record UpdatePromotionCommand(Guid PromotionId, UpdatePromotionRequest Request)
        : IRequest<(UpdatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)>;

    public class UpdatePromotionCommandHandler
        : IRequestHandler<UpdatePromotionCommand, (UpdatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IFileUploadService _fileUploadService;

        public UpdatePromotionCommandHandler(IPromotionRepository promotionRepository, IFileUploadService fileUploadService)
        {
            _promotionRepository = promotionRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<(UpdatePromotionResult Result, PromotionDto? Promotion, List<Guid>? InvalidProductIds)> Handle(
            UpdatePromotionCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            List<PromotionBannerCreationData>? banners = null;
            if (request.Banners is not null)
            {
                banners = new List<PromotionBannerCreationData>();
                foreach (var bannerRequest in request.Banners)
                {
                    var url = await _fileUploadService.SaveProductImageAsync(bannerRequest.File, cancellationToken);
                    banners.Add(new PromotionBannerCreationData
                    {
                        Url = url,
                        AltText = bannerRequest.AltText
                    });
                }
            }

            return await _promotionRepository.UpdatePromotionAsync(
                command.PromotionId,
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
