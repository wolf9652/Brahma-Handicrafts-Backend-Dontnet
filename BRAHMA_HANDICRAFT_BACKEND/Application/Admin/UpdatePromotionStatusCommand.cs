using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record UpdatePromotionStatusCommand(Guid PromotionId, bool IsActive)
        : IRequest<UpdatePromotionStatusResponseDto?>;

    public class UpdatePromotionStatusCommandHandler : IRequestHandler<UpdatePromotionStatusCommand, UpdatePromotionStatusResponseDto?>
    {
        private readonly IPromotionRepository _promotionRepository;

        public UpdatePromotionStatusCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<UpdatePromotionStatusResponseDto?> Handle(UpdatePromotionStatusCommand request, CancellationToken cancellationToken)
        {
            var promotion = await _promotionRepository.UpdatePromotionStatusAsync(request.PromotionId, request.IsActive, cancellationToken);

            if (promotion is null)
            {
                return null;
            }

            return new UpdatePromotionStatusResponseDto
            {
                PromotionId = promotion.Id,
                IsActive = promotion.IsActive
            };
        }
    }
}
