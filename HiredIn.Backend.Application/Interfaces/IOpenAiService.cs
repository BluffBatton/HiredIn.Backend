namespace HiredIn.Backend.Application.Interfaces
{
    public interface IOpenAiService
    {
        Task<string> GenerateTextAsync(
            string instructions,
            string input,
            CancellationToken cancellationToken = default);
    }
}