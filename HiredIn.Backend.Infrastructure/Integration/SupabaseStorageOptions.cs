namespace HiredIn.Backend.Infrastructure.Integration
{
    public class SupabaseStorageOptions
    {
        public const string SectionName = "Supabase";

        public string Url { get; set; } = null!;
        public string ServiceRoleKey { get; set; } = null!;
    }
}