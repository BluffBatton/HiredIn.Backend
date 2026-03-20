namespace HiredIn.Backend.Application.Interfaces
{
    public interface IUserContextService
    {
        Guid? GetCurrentUserId();
        string? GetCurrentUserRole();
        bool IsAuthenticated();
    }
}
