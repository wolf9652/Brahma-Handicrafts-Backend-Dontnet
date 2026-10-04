using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using BRAHMA_HANDICRAFT_BACKEND.Domain;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record CreatePromotionCommand(
        string Name,
        DiscountType DiscountType,
        decimal DiscountValue,
        DateTime StartDate,
        DateTime EndDate,
        bool IsActive,
        List<Guid> ProductIds) : IRequest<CreatePromotionResponseDto>;

    public class CreatePromotionCommandHandler : IRequestHandler<CreatePromotionCommand, CreatePromotionResponseDto>
    {
        private readonly IPromotionRepository _promotionRepository;

        public CreatePromotionCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<CreatePromotionResponseDto> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
        {
            var promotion = new Promotion
            {
                Name = request.Name,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive
            };

            var (created, productCount) = await _promotionRepository.CreatePromotionAsync(
                promotion, request.ProductIds, cancellationToken);

            return new CreatePromotionResponseDto
            {
                PromotionId = created.Id,
                Name = created.Name,
                DiscountType = created.DiscountType,
                DiscountValue = created.DiscountValue,
                DiscountTag = PromotionDisplayHelper.ComputeDiscountTag(created.DiscountType, created.DiscountValue),
                StartDate = created.StartDate,
                EndDate = created.EndDate,
                IsActive = created.IsActive,
                ProductCount = productCount
            };
        }
    }
}
