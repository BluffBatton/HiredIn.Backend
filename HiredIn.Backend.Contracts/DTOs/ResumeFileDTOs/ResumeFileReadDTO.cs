namespace HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs
{
    public class ResumeFileReadDTO
    {
        public Guid Id { get; set; }
        public Guid ResumeId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
    }
}