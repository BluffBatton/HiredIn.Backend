using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Ai
{
    public class GetResumeImprovementTipsQuery : IRequest<string>
    {
        public Guid ResumeId { get; set; }

        public GetResumeImprovementTipsQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeImprovementTipsQueryHandler
        : IRequestHandler<GetResumeImprovementTipsQuery, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IOpenAiService _openAiService;

        public GetResumeImprovementTipsQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IOpenAiService openAiService)
        {
            _context = context;
            _userContextService = userContextService;
            _openAiService = openAiService;
        }

        public async Task<string> Handle(
            GetResumeImprovementTipsQuery request,
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

            var skills = resume.ResumeSkills
                .Select(rs => $"{rs.Skill.Name} ({rs.Level})")
                .ToList();

            var instructions =
                "Ти є AI-помічником у веб-системі HiredIn. " +
                "Проаналізуй резюме кандидата і дай короткі практичні поради щодо покращення. " +
                "Не вигадуй фактів і не додавай досвід, якого немає у вхідних даних. " +
                "Пиши українською мовою. " +
                "Дай 3-5 порад.";

            var input = $"""
            Резюме кандидата:
            Назва: {resume.Title}
            Бажана посада: {resume.DesiredPosition}
            Опис: {resume.Summary}
            Тип зайнятості: {resume.EmploymentType}
            Формат роботи: {resume.WorkFormat}
            Рівень досвіду: {resume.ExperienceLevel}
            Навички: {string.Join(", ", skills)}

            Освіта:
            {string.Join("\n", resume.Educations.Select(e =>
                $"- {e.InstitutionName}, {e.Degree}, {e.FieldOfStudy}. {e.Description}"))}

            Досвід роботи:
            {string.Join("\n", resume.WorkExperiences.Select(w =>
                $"- {w.PositionTitle} у {w.CompanyName}. {w.Description}"))}
            """;

            return await _openAiService.GenerateTextAsync(
                instructions,
                input,
                cancellationToken);
        }
    }
}