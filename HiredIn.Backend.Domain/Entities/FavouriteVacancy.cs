using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class FavouriteVacancy : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }

        public User User { get; set; } = null!;
        public Vacancy Vacancy { get; set; } = null!;
    }
}