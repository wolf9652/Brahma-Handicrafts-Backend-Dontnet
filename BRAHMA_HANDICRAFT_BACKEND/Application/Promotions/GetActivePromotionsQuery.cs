using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Promotions
{
    public record GetActivePromotionsQuery : IRequest<List<CustomerPromotionDto>>;

    public class GetActivePromotionsQueryHandler : IRequestHandler<GetActivePromotionsQuery, List<CustomerPromotionDto>>
    {
        private readonly IPromotionRepository _promotionRepository;

        public GetActivePromotionsQueryHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public Task<List<CustomerPromotionDto>> Handle(GetActivePromotionsQuery request, CancellationToken cancellationToken)
        {
            return _promotionRepository.GetActivePromotionsAsync(cancellationToken);
        }
    }
}
