using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.RecommendationDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Recommendation
{
    public class GetRecommendedVacanciesQuery : IRequest<List<VacancyRecommendationReadDTO>>
    {
        public Guid ResumeId { get; set; }
        public int Limit { get; set; }

        public GetRecommendedVacanciesQuery(Guid resumeId, int limit = 20)
        {
            ResumeId = resumeId;
            Limit = limit;
        }
    }

    public class GetRecommendedVacanciesQueryHandler
        : IRequestHandler<GetRecommendedVacanciesQuery, List<VacancyRecommendationReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IVacancyRecommendationScoringService _scoringService;

        public GetRecommendedVacanciesQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IVacancyRecommendationScoringService scoringService)
        {
            _context = context;
            _userContextService = userContextService;
            _scoringService = scoringService;
        }

        public async Task<List<VacancyRecommendationReadDTO>> Handle(
            GetRecommendedVacanciesQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var limit = NormalizeLimit(request.Limit);

            var resume = await _context.Resumes
                .AsNoTracking()
                .Include(r => r.CandidateProfile)
                .Include(r => r.ResumeSkills)
                    .ThenInclude(rs => rs.Skill)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.ResumeId &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You can get recommendations only for your own resume.");

            var resumeSkills = resume.ResumeSkills
                .Select(rs => rs.Skill.Name.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var vacancies = await _context.Vacancies
                .AsNoTracking()
                .Include(v => v.Company)
                .Include(v => v.VacancySkills)
                    .ThenInclude(vs => vs.Skill)
                .Where(v =>
                    v.DeletedAtUtc == null &&
                    v.Status == VacancyStatus.Published)
                .ToListAsync(cancellationToken);

            var recommendations = vacancies
                .Select(vacancy =>
                {
                    var vacancySkills = vacancy.VacancySkills
                        .Select(vs => vs.Skill.Name.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    var matchedSkills = resumeSkills
                        .Intersect(vacancySkills, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    var missingSkills = vacancySkills
                        .Except(resumeSkills, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    var score = _scoringService.CalculateScore(
                        resume,
                        vacancy,
                        vacancySkills,
                        matchedSkills);

                    return new VacancyRecommendationReadDTO
                    {
                        VacancyId = vacancy.Id,
                        Title = vacancy.Title,
                        CompanyName = vacancy.Company.Name,
                        City = vacancy.City,
                        SalaryMin = vacancy.SalaryMin,
                        SalaryMax = vacancy.SalaryMax,
                        EmploymentType = vacancy.EmploymentType.ToString(),
                        WorkFormat = vacancy.WorkFormat.ToString(),
                        ExperienceLevel = vacancy.ExperienceLevel.ToString(),
                        MatchScore = score,
                        MatchedSkills = matchedSkills,
                        MissingSkills = missingSkills
                    };
                })
                .Where(r => r.MatchScore > 0)
                .OrderByDescending(r => r.MatchScore)
                .Take(limit)
                .ToList();

            return recommendations;
        }

        private static int NormalizeLimit(int limit)
        {
            if (limit <= 0)
                return 20;

            if (limit > 50)
                return 50;

            return limit;
        }
    }
}