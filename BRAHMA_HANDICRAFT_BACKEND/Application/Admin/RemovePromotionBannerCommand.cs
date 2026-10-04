using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Admin
{
    public record RemovePromotionBannerCommand(Guid PromotionId, Guid BannerId) : IRequest<bool>;

    public class RemovePromotionBannerCommandHandler : IRequestHandler<RemovePromotionBannerCommand, bool>
    {
        private readonly IPromotionRepository _promotionRepository;

        public RemovePromotionBannerCommandHandler(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public Task<bool> Handle(RemovePromotionBannerCommand request, CancellationToken cancellationToken)
        {
            return _promotionRepository.RemovePromotionBannerAsync(request.PromotionId, request.BannerId, cancellationToken);
        }
    }
}
