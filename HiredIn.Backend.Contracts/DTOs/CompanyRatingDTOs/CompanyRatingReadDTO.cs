namespace HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs
{
    public class CompanyRatingReadDTO
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public required string CompanyName { get; set; }
        public Guid UserId { get; set; }
        public required string FullName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
