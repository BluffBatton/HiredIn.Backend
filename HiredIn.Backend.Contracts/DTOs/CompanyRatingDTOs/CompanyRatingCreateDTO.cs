namespace HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs
{
    public class CompanyRatingCreateDTO
    {
        public Guid CompanyId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
