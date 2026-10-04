using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record UpdatePromotionCommand(
        Guid PromotionId,
        string Name,
        DiscountType DiscountType,
        decimal DiscountValue,
        DateTime StartDate,
        DateTime EndDate) : IRequest<CreatePromotionResponseDto?>;

    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand, CreatePromotionResponseDto?>
    {
        private readonly IPromotionRepository _promotionRepository;

        public UpdatePromotionCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<CreatePromotionResponseDto?> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            var (promotion, productCount) = await _promotionRepository.UpdatePromotionAsync(
                request.PromotionId, request.Name, request.DiscountType, request.DiscountValue,
                request.StartDate, request.EndDate, cancellationToken);

            if (promotion is null)
            {
                return null;
            }

            return new CreatePromotionResponseDto
            {
                PromotionId = promotion.Id,
                Name = promotion.Name,
                DiscountType = promotion.DiscountType,
                DiscountValue = promotion.DiscountValue,
                DiscountTag = PromotionDisplayHelper.ComputeDiscountTag(promotion.DiscountType, promotion.DiscountValue),
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                ProductCount = productCount
            };
        }
    }
}
