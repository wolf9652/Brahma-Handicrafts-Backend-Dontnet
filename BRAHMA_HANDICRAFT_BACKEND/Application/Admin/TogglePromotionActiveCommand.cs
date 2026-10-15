using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record TogglePromotionActiveCommand(Guid PromotionId)
        : IRequest<(TogglePromotionActiveResult Result, bool IsActive)>;

    public class TogglePromotionActiveCommandHandler
        : IRequestHandler<TogglePromotionActiveCommand, (TogglePromotionActiveResult Result, bool IsActive)>
    {
        private readonly IPromotionRepository _promotionRepository;

        public TogglePromotionActiveCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public Task<(TogglePromotionActiveResult Result, bool IsActive)> Handle(
            TogglePromotionActiveCommand command, CancellationToken cancellationToken)
        {
            return _promotionRepository.TogglePromotionActiveAsync(command.PromotionId, cancellationToken);
        }
    }
}
