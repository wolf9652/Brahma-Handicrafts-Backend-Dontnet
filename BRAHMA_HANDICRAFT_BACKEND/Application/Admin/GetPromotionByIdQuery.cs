using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record GetPromotionByIdQuery(Guid PromotionId) : IRequest<AdminPromotionDetailDto?>;

    public class GetPromotionByIdQueryHandler : IRequestHandler<GetPromotionByIdQuery, AdminPromotionDetailDto?>
    {
        private readonly IPromotionRepository _promotionRepository;

        public GetPromotionByIdQueryHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<AdminPromotionDetailDto?> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
        {
            var detail = await _promotionRepository.GetPromotionDetailAsync(request.PromotionId, cancellationToken);

            if (detail is null)
            {
                return null;
            }

            detail.DiscountTag = PromotionDisplayHelper.ComputeDiscountTag(detail.DiscountType, detail.DiscountValue);
            return detail;
        }
    }
}
