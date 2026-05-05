using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Ai
{
    public class ExplainVacancyRecommendationQuery : IRequest<string>
    {
        public Guid ResumeId { get; set; }
        public Guid VacancyId { get; set; }

        public ExplainVacancyRecommendationQuery(Guid resumeId, Guid vacancyId)
        {
            ResumeId = resumeId;
            VacancyId = vacancyId;
        }
    }

    public class ExplainVacancyRecommendationQueryHandler
        : IRequestHandler<ExplainVacancyRecommendationQuery, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IOpenAiService _openAiService;

        public ExplainVacancyRecommendationQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IOpenAiService openAiService)
        {
            _context = context;
            _userContextService = userContextService;
            _openAiService = openAiService;
        }

        public async Task<string> Handle(
            ExplainVacancyRecommendationQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var resume = await _context.Resumes
                .AsNoTracking()
                .Include(r => r.CandidateProfile)
                .Include(r => r.Educations)
                .Include(r => r.WorkExperiences)
                .Include(r => r.ResumeSkills)
                    .ThenInclude(rs => rs.Skill)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.ResumeId &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You can analyze only your own resume.");

            var vacancy = await _context.Vacancies
                .AsNoTracking()
                .Include(v => v.Company)
                .Include(v => v.VacancySkills)
                    .ThenInclude(vs => vs.Skill)
                .FirstOrDefaultAsync(v =>
                    v.Id == request.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            var resumeSkills = resume.ResumeSkills
                .Select(rs => rs.Skill.Name)
                .ToList();

            var vacancySkills = vacancy.VacancySkills
                .Select(vs => vs.Skill.Name)
                .ToList();

            var matchedSkills = resumeSkills
                .Intersect(vacancySkills, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var missingSkills = vacancySkills
                .Except(resumeSkills, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var instructions =
                "Ти є AI-помічником у веб-системі HiredIn. " +
                "Поясни кандидату, чому конкретна вакансія може йому підходити. " +
                "Не вигадуй досвід, навички або факти, яких немає у вхідних даних. " +
                "Пиши українською мовою, просто і коротко. " +
                "Відповідь має бути 2-4 речення.";

            var input = $"""
            Дані резюме кандидата:
            Назва резюме: {resume.Title}
            Бажана посада: {resume.DesiredPosition}
            Опис кандидата: {resume.Summary}
            Тип зайнятості: {resume.EmploymentType}
            Формат роботи: {resume.WorkFormat}
            Рівень досвіду: {resume.ExperienceLevel}
            Навички кандидата: {string.Join(", ", resumeSkills)}

            Освіта:
            {string.Join("\n", resume.Educations.Select(e =>
                $"- {e.InstitutionName}, {e.Degree}, {e.FieldOfStudy}. {e.Description}"))}

            Досвід роботи:
            {string.Join("\n", resume.WorkExperiences.Select(w =>
                $"- {w.PositionTitle} у {w.CompanyName}. {w.Description}"))}

            Дані вакансії:
            Компанія: {vacancy.Company.Name}
            Назва вакансії: {vacancy.Title}
            Опис вакансії: {vacancy.Description}
            Тип зайнятості: {vacancy.EmploymentType}
            Формат роботи: {vacancy.WorkFormat}
            Місто: {vacancy.City}
            Рівень досвіду: {vacancy.ExperienceLevel}
            Зарплата: {vacancy.SalaryMin} - {vacancy.SalaryMax}
            Вимоги до навичок: {string.Join(", ", vacancySkills)}

            Спільні навички: {string.Join(", ", matchedSkills)}
            Відсутні навички: {string.Join(", ", missingSkills)}
            """;

            return await _openAiService.GenerateTextAsync(
                instructions,
                input,
                cancellationToken);
        }
    }
}