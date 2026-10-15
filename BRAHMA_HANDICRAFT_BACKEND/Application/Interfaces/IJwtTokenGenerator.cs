using BRAHMA_HANDICRAFT_BACKEND.Domain;

namespace BRAHMA_HANDICRAFT_BACKEND.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Users user);
    }
}
