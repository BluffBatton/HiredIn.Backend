using HiredIn.Backend.Application.Interfaces;

namespace HiredIn.Backend.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(
            string bucket,
            string path,
            byte[] fileBytes,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string bucket,
            string path,
            CancellationToken cancellationToken = default);

        string GetPublicUrl(string bucket, string path);

        Task<string> CreateSignedUrlAsync(
            string bucket,
            string path,
            int expiresInSeconds,
            CancellationToken cancellationToken = default);
    }
}
