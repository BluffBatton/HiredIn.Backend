using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HiredIn.Backend.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace HiredIn.Backend.Infrastructure.Integration
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;

        public OpenAiService(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GenerateTextAsync(
            string instructions,
            string input,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("OpenAI API key is not configured.");

            var requestBody = new
            {
                model = _options.Model,
                instructions = instructions,
                input = input
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/responses");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(
                request,
                cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAI request failed: {json}");

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.TryGetProperty("output_text", out var outputText))
                return outputText.GetString() ?? string.Empty;

            if (root.TryGetProperty("output", out var output))
            {
                foreach (var outputItem in output.EnumerateArray())
                {
                    if (!outputItem.TryGetProperty("content", out var content))
                        continue;

                    foreach (var contentItem in content.EnumerateArray())
                    {
                        if (contentItem.TryGetProperty("text", out var text))
                            return text.GetString() ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }
    }
}