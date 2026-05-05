using HiredIn.Backend.Application.Interfaces;

namespace HiredIn.Backend.Application.Services.Recommendation
{
    public class VacancyRecommendationScoringService : IVacancyRecommendationScoringService
    {
        public int CalculateScore(
            Domain.Entities.Resume resume,
            Domain.Entities.Vacancy vacancy,
            List<string> vacancySkills,
            List<string> matchedSkills)
        {
            var score = 0;

            if (vacancySkills.Count > 0)
            {
                var skillScore = (double)matchedSkills.Count / vacancySkills.Count * 50;
                score += (int)Math.Round(skillScore);
            }

            score += CalculatePositionScore(resume.DesiredPosition, vacancy.Title);

            if (resume.WorkFormat == vacancy.WorkFormat)
                score += 10;

            if (resume.EmploymentType == vacancy.EmploymentType)
                score += 5;

            if (resume.ExperienceLevel == vacancy.ExperienceLevel)
                score += 10;

            if (!string.IsNullOrWhiteSpace(resume.CandidateProfile.City) &&
                !string.IsNullOrWhiteSpace(vacancy.City) &&
                string.Equals(
                    resume.CandidateProfile.City.Trim(),
                    vacancy.City.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                score += 5;
            }

            return Math.Min(score, 100);
        }

        private static int CalculatePositionScore(string? desiredPosition, string vacancyTitle)
        {
            if (string.IsNullOrWhiteSpace(desiredPosition) ||
                string.IsNullOrWhiteSpace(vacancyTitle))
            {
                return 0;
            }

            var desired = desiredPosition.ToLower().Trim();
            var title = vacancyTitle.ToLower().Trim();

            if (title.Contains(desired) || desired.Contains(title))
                return 20;

            var desiredWords = desired
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .ToList();

            var titleWords = title
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .ToList();

            var commonWordsCount = desiredWords
                .Intersect(titleWords)
                .Count();

            if (commonWordsCount >= 2)
                return 15;

            if (commonWordsCount == 1)
                return 10;

            return 0;
        }
    }
}