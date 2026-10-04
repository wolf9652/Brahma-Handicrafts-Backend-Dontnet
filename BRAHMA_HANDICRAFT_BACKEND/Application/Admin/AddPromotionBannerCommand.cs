using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record AddPromotionBannerCommand(Guid PromotionId, string ImageUrl, string AltText)
        : IRequest<AddPromotionBannerResponseDto?>;

    public class AddPromotionBannerCommandHandler : IRequestHandler<AddPromotionBannerCommand, AddPromotionBannerResponseDto?>
    {
        private readonly IPromotionRepository _promotionRepository;

        public AddPromotionBannerCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<AddPromotionBannerResponseDto?> Handle(AddPromotionBannerCommand request, CancellationToken cancellationToken)
        {
            var banner = await _promotionRepository.AddPromotionBannerAsync(
                request.PromotionId, request.ImageUrl, request.AltText, cancellationToken);

            if (banner is null)
            {
                return null;
            }

            return new AddPromotionBannerResponseDto
            {
                BannerId = banner.Id,
                PromotionId = banner.PromotionId,
                ImageUrl = banner.ImageURL,
                AltText = banner.AltText
            };
        }
    }
}
