using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string password, string passwordHash);
    }
}
