using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record DeletePromotionCommand(Guid PromotionId) : IRequest<bool>;

    public class DeletePromotionCommandHandler : IRequestHandler<DeletePromotionCommand, bool>
    {
        private readonly IPromotionRepository _promotionRepository;

        public DeletePromotionCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public Task<bool> Handle(DeletePromotionCommand command, CancellationToken cancellationToken)
        {
            return _promotionRepository.DeletePromotionAsync(command.PromotionId, cancellationToken);
        }
    }
}
