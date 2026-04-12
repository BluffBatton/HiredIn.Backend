namespace HiredIn.Backend.Contracts.DTOs.FavouriteVacancyDTOs
{
    public class FavouriteVacancyReadDTO
    {
        public Guid Id { get; set;  }
        public Guid VacancyId { get; set; }
        public required string VacancyName { get; set; }
        public required string CompanyName { get; set; }
        public string? City { get; set; }
        public decimal? Salary { get; set; }
    }
}
