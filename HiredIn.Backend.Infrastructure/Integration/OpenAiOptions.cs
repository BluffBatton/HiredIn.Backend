namespace HiredIn.Backend.Infrastructure.Integration
{
    public class OpenAiOptions
    {
        public const string SectionName = "OpenAI";

        public string ApiKey { get; set; } = null!;
        public string Model { get; set; } = "gpt-4.1-mini";
    }
}