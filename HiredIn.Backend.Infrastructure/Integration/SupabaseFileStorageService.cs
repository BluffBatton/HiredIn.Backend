using HiredIn.Backend.Application.Interfaces;
using Microsoft.Extensions.Options;
using Supabase;
using Supabase.Storage;

namespace HiredIn.Backend.Infrastructure.Integration
{
    public class SupabaseFileStorageService : IFileStorageService
    {
        private readonly Supabase.Client _client;

        public SupabaseFileStorageService(IOptions<SupabaseStorageOptions> options)
        {
            var value = options.Value;

            _client = new Supabase.Client(
                value.Url,
                value.ServiceRoleKey,
                new SupabaseOptions
                {
                    AutoConnectRealtime = false
                });
        }

        public async Task<string> UploadAsync(
            string bucket,
            string path,
            byte[] fileBytes,
            CancellationToken cancellationToken = default)
        {
            await _client.InitializeAsync();

            var tempFilePath = Path.GetTempFileName();

            try
            {
                await File.WriteAllBytesAsync(tempFilePath, fileBytes, cancellationToken);

                await _client.Storage
                    .From(bucket)
                    .Upload(tempFilePath, path, new Supabase.Storage.FileOptions
                    {
                        CacheControl = "3600",
                        Upsert = true
                    });

                return path;
            }
            finally
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        public async Task DeleteAsync(
            string bucket,
            string path,
            CancellationToken cancellationToken = default)
        {
            await _client.InitializeAsync();

            await _client.Storage
                .From(bucket)
                .Remove(new List<string> { path });
        }

        public string GetPublicUrl(string bucket, string path)
        {
            return _client.Storage
                .From(bucket)
                .GetPublicUrl(path);
        }

        public async Task<string> CreateSignedUrlAsync(
            string bucket,
            string path,
            int expiresInSeconds,
            CancellationToken cancellationToken = default)
        {
            await _client.InitializeAsync();

            return await _client.Storage
                .From(bucket)
                .CreateSignedUrl(path, expiresInSeconds);
        }
    }
}