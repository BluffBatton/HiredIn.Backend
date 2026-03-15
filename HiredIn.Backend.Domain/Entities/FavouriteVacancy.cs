namespace HiredIn.Backend.Domain.Entities
{
    public class FavouriteVacancy
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }

        public User User { get; set; } = null!;
        public Vacancy Vacancy { get; set; } = null!;
    }
}