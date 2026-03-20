using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
    }
}
