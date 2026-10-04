using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Promotions
{
    public record GetActivePromotionBannersQuery : IRequest<List<PublicPromotionBannerDto>>;

    public class GetActivePromotionBannersQueryHandler : IRequestHandler<GetActivePromotionBannersQuery, List<PublicPromotionBannerDto>>
    {
        private readonly IPromotionRepository _promotionRepository;

        public GetActivePromotionBannersQueryHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public Task<List<PublicPromotionBannerDto>> Handle(GetActivePromotionBannersQuery request, CancellationToken cancellationToken)
        {
            return _promotionRepository.GetActivePromotionBannersAsync(cancellationToken);
        }
    }
}
