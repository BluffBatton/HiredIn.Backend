using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Interfaces
{
    public interface IVacancyRecommendationScoringService
    {
        int CalculateScore(
            Resume resume,
            Vacancy vacancy,
            List<string> vacancySkills,
            List<string> matchedSkills);
    }
}