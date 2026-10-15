using BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces;
using BRAHMA_HANDICRAFT_BACKEND.Application.Models;
using MediatR;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Designs
{
    public record GetDesignDetailsByDesignIdQuery(Guid ProductId, Guid DesignId)
        : IRequest<DesignDetailsDto?>;

    public class GetDesignDetailsByDesignIdQueryHandler : IRequestHandler<GetDesignDetailsByDesignIdQuery, DesignDetailsDto?>
    {
        private readonly IDesignRepository _designRepository;

        public GetDesignDetailsByDesignIdQueryHandler(IDesignRepository designRepository)
        {
            _designRepository = designRepository;
        }

        public Task<DesignDetailsDto?> Handle(GetDesignDetailsByDesignIdQuery request, CancellationToken cancellationToken)
        {
            return _designRepository.GetDesignDetailsByIdAsync(request.ProductId, request.DesignId, cancellationToken);
        }
    }
}
