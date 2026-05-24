using HiredIn.Backend.Application.Interfaces;

namespace HiredIn.Backend.Application.Services.Recommendation
{
    public class VacancyRecommendationScoringService : IVacancyRecommendationScoringService
    {
        public int CalculateScore(
            Domain.Entities.Resume resume,
            Domain.Entities.Vacancy vacancy,
            List<Domain.Entities.VacancySkill> vacancySkills,
            List<string> matchedSkills)
        {
            var score = 0;

            if (vacancySkills.Count > 0)
            {
                var totalSkillWeight = vacancySkills.Sum(vs => vs.IsRequired ? 2 : 1);
                var matchedSkillWeight = vacancySkills
                    .Where(vs => matchedSkills.Contains(vs.Skill.Name, StringComparer.OrdinalIgnoreCase))
                    .Sum(vs => vs.IsRequired ? 2 : 1);

                var skillScore = (double)matchedSkillWeight / totalSkillWeight * 50;
                score += (int)Math.Round(skillScore);
            }

            score += CalculatePositionScore(resume.DesiredPosition, vacancy.Title);

            if (resume.WorkFormat == vacancy.WorkFormat)
                score += 10;

            if (resume.EmploymentType == vacancy.EmploymentType)
                score += 5;

            score += CalculateExperienceScore(resume.ExperienceLevel, vacancy.ExperienceLevel);

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

        private static int CalculateExperienceScore(
            Domain.Enums.ExperienceLevel resumeExperience,
            Domain.Enums.ExperienceLevel vacancyExperience)
        {
            var difference = Math.Abs((int)resumeExperience - (int)vacancyExperience);

            return difference switch
            {
                0 => 10,
                1 => 6,
                2 => 3,
                _ => 0
            };
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
