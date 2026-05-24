using HiredIn.Backend.Application.Common.Models;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ContractEmploymentType = HiredIn.Backend.Contracts.DTOs.Enums.EmploymentType;
using ContractExperienceLevel = HiredIn.Backend.Contracts.DTOs.Enums.ExperienceLevel;
using ContractSkillLevel = HiredIn.Backend.Contracts.DTOs.Enums.SkillLevel;
using ContractWorkFormat = HiredIn.Backend.Contracts.DTOs.Enums.WorkFormat;
using DomainCandidateProfile = HiredIn.Backend.Domain.Entities.CandidateProfile;
using DomainResume = HiredIn.Backend.Domain.Entities.Resume;
using DomainEmploymentType = HiredIn.Backend.Domain.Enums.EmploymentType;
using DomainExperienceLevel = HiredIn.Backend.Domain.Enums.ExperienceLevel;
using DomainResumeVisibility = HiredIn.Backend.Domain.Enums.ResumeVisibility;
using DomainUserRole = HiredIn.Backend.Domain.Enums.UserRole;
using DomainUserStatus = HiredIn.Backend.Domain.Enums.UserStatus;
using DomainWorkFormat = HiredIn.Backend.Domain.Enums.WorkFormat;

namespace HiredIn.Backend.Application.Services.CandidateProfile
{
    public class SearchCandidatesQuery : IRequest<PaginatedList<CandidateSearchResultDTO>>
    {
        public string? SearchText { get; set; }
        public string? City { get; set; }
        public bool OpenToWorkOnly { get; set; } = true;
        public Guid? SkillId { get; set; }
        public string? Skill { get; set; }
        public ContractEmploymentType? EmploymentType { get; set; }
        public ContractWorkFormat? WorkFormat { get; set; }
        public ContractExperienceLevel? ExperienceLevel { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SearchCandidatesQueryHandler : IRequestHandler<SearchCandidatesQuery, PaginatedList<CandidateSearchResultDTO>>
    {
        private readonly IApplicationDbContext _context;

        public SearchCandidatesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<CandidateSearchResultDTO>> Handle(
            SearchCandidatesQuery request,
            CancellationToken cancellationToken)
        {
            var pageIndex = request.Page <= 0 ? 0 : request.Page - 1;
            var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);

            var query = _context.CandidateProfiles
                .AsNoTracking()
                .Where(cp =>
                    cp.DeletedAtUtc == null &&
                    cp.User.DeletedAtUtc == null &&
                    cp.User.Status == DomainUserStatus.Active &&
                    cp.User.Role == DomainUserRole.Candidate &&
                    cp.Resumes.Any(r =>
                        r.DeletedAtUtc == null &&
                        (r.Visibility == DomainResumeVisibility.Public ||
                         r.Visibility == DomainResumeVisibility.OnlyEmployers)));

            if (request.OpenToWorkOnly)
                query = query.Where(cp => cp.OpenToWork);

            query = ApplyFilters(query, request);
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);
            var profileIds = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(cp => cp.Id)
                .ToListAsync(cancellationToken);

            if (profileIds.Count == 0)
            {
                return new PaginatedList<CandidateSearchResultDTO>(
                    Array.Empty<CandidateSearchResultDTO>(),
                    totalCount,
                    pageIndex,
                    pageSize);
            }

            var order = profileIds
                .Select((id, index) => new { id, index })
                .ToDictionary(item => item.id, item => item.index);

            var profiles = await _context.CandidateProfiles
                .AsNoTracking()
                .Where(cp => profileIds.Contains(cp.Id))
                .Include(cp => cp.User)
                .Include(cp => cp.Resumes.Where(r =>
                    r.DeletedAtUtc == null &&
                    (r.Visibility == DomainResumeVisibility.Public ||
                     r.Visibility == DomainResumeVisibility.OnlyEmployers)))
                    .ThenInclude(r => r.ResumeSkills)
                    .ThenInclude(rs => rs.Skill)
                .ToListAsync(cancellationToken);

            var items = profiles
                .OrderBy(profile => order[profile.Id])
                .Select(ToDto)
                .ToList();

            return new PaginatedList<CandidateSearchResultDTO>(
                items,
                totalCount,
                pageIndex,
                pageSize);
        }

        private static IQueryable<DomainCandidateProfile> ApplyFilters(
            IQueryable<DomainCandidateProfile> query,
            SearchCandidatesQuery request)
        {
            var searchText = request.SearchText?.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(cp =>
                    cp.User.FirstName.ToLower().Contains(searchText) ||
                    cp.User.LastName.ToLower().Contains(searchText) ||
                    (cp.City != null && cp.City.ToLower().Contains(searchText)) ||
                    (cp.About != null && cp.About.ToLower().Contains(searchText)) ||
                    cp.Resumes.Any(r =>
                        r.DeletedAtUtc == null &&
                        (r.Visibility == DomainResumeVisibility.Public ||
                         r.Visibility == DomainResumeVisibility.OnlyEmployers) &&
                        (r.Title.ToLower().Contains(searchText) ||
                         r.DesiredPosition.ToLower().Contains(searchText) ||
                         (r.Summary != null && r.Summary.ToLower().Contains(searchText)) ||
                         r.ResumeSkills.Any(rs =>
                             rs.Skill.DeletedAtUtc == null &&
                             rs.Skill.Name.ToLower().Contains(searchText)))));
            }

            var city = request.City?.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(cp =>
                    cp.City != null &&
                    cp.City.ToLower().Contains(city));
            }

            if (request.SkillId.HasValue)
            {
                query = query.Where(cp => cp.Resumes.Any(r =>
                    r.DeletedAtUtc == null &&
                    (r.Visibility == DomainResumeVisibility.Public ||
                     r.Visibility == DomainResumeVisibility.OnlyEmployers) &&
                    r.ResumeSkills.Any(rs =>
                        rs.SkillId == request.SkillId.Value &&
                        rs.Skill.DeletedAtUtc == null)));
            }

            var skill = request.Skill?.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(cp => cp.Resumes.Any(r =>
                    r.DeletedAtUtc == null &&
                    (r.Visibility == DomainResumeVisibility.Public ||
                     r.Visibility == DomainResumeVisibility.OnlyEmployers) &&
                    r.ResumeSkills.Any(rs =>
                        rs.Skill.DeletedAtUtc == null &&
                        rs.Skill.Name.ToLower().Contains(skill))));
            }

            if (request.EmploymentType.HasValue)
            {
                var employmentType = (DomainEmploymentType)request.EmploymentType.Value;
                query = query.Where(cp => cp.Resumes.Any(r =>
                    r.DeletedAtUtc == null &&
                    (r.Visibility == DomainResumeVisibility.Public ||
                     r.Visibility == DomainResumeVisibility.OnlyEmployers) &&
                    r.EmploymentType == employmentType));
            }

            if (request.WorkFormat.HasValue)
            {
                var workFormat = (DomainWorkFormat)request.WorkFormat.Value;
                query = query.Where(cp => cp.Resumes.Any(r =>
                    r.DeletedAtUtc == null &&
                    (r.Visibility == DomainResumeVisibility.Public ||
                     r.Visibility == DomainResumeVisibility.OnlyEmployers) &&
                    r.WorkFormat == workFormat));
            }

            if (request.ExperienceLevel.HasValue)
            {
                var experienceLevel = (DomainExperienceLevel)request.ExperienceLevel.Value;
                query = query.Where(cp => cp.Resumes.Any(r =>
                    r.DeletedAtUtc == null &&
                    (r.Visibility == DomainResumeVisibility.Public ||
                     r.Visibility == DomainResumeVisibility.OnlyEmployers) &&
                    r.ExperienceLevel == experienceLevel));
            }

            return query;
        }

        private static IQueryable<DomainCandidateProfile> ApplySorting(
            IQueryable<DomainCandidateProfile> query,
            string? sortBy,
            string? sortDirection)
        {
            var descending = !string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.Trim().ToLowerInvariant() switch
            {
                "name" => descending
                    ? query.OrderByDescending(cp => cp.User.FirstName).ThenByDescending(cp => cp.User.LastName)
                    : query.OrderBy(cp => cp.User.FirstName).ThenBy(cp => cp.User.LastName),
                "city" => descending
                    ? query.OrderByDescending(cp => cp.City).ThenByDescending(cp => cp.UpdatedAtUtc)
                    : query.OrderBy(cp => cp.City).ThenByDescending(cp => cp.UpdatedAtUtc),
                "createdat" or "created" => descending
                    ? query.OrderByDescending(cp => cp.CreatedAtUtc)
                    : query.OrderBy(cp => cp.CreatedAtUtc),
                "experiencelevel" or "experience" => descending
                    ? query.OrderByDescending(cp => cp.Resumes
                            .Where(r =>
                                r.DeletedAtUtc == null &&
                                (r.Visibility == DomainResumeVisibility.Public ||
                                 r.Visibility == DomainResumeVisibility.OnlyEmployers))
                            .Max(r => (int?)r.ExperienceLevel))
                        .ThenByDescending(cp => cp.UpdatedAtUtc)
                    : query.OrderBy(cp => cp.Resumes
                            .Where(r =>
                                r.DeletedAtUtc == null &&
                                (r.Visibility == DomainResumeVisibility.Public ||
                                 r.Visibility == DomainResumeVisibility.OnlyEmployers))
                            .Max(r => (int?)r.ExperienceLevel))
                        .ThenByDescending(cp => cp.UpdatedAtUtc),
                _ => descending
                    ? query.OrderByDescending(cp => cp.Resumes
                            .Where(r =>
                                r.DeletedAtUtc == null &&
                                (r.Visibility == DomainResumeVisibility.Public ||
                                 r.Visibility == DomainResumeVisibility.OnlyEmployers))
                            .Max(r => (DateTime?)r.UpdatedAtUtc))
                        .ThenByDescending(cp => cp.UpdatedAtUtc)
                    : query.OrderBy(cp => cp.Resumes
                            .Where(r =>
                                r.DeletedAtUtc == null &&
                                (r.Visibility == DomainResumeVisibility.Public ||
                                 r.Visibility == DomainResumeVisibility.OnlyEmployers))
                            .Max(r => (DateTime?)r.UpdatedAtUtc))
                        .ThenBy(cp => cp.UpdatedAtUtc)
            };
        }

        private static CandidateSearchResultDTO ToDto(DomainCandidateProfile profile)
        {
            var visibleResumes = profile.Resumes
                .Where(IsSearchableResume)
                .OrderByDescending(r => r.IsPrimary)
                .ThenByDescending(r => r.UpdatedAtUtc)
                .ToList();

            var primaryResume = visibleResumes.FirstOrDefault();
            var updatedAtUtc = visibleResumes
                .Select(r => (DateTime?)r.UpdatedAtUtc)
                .Max() ?? profile.UpdatedAtUtc;

            return new CandidateSearchResultDTO
            {
                CandidateProfileId = profile.Id,
                UserId = profile.UserId,
                FullName = $"{profile.User.FirstName} {profile.User.LastName}".Trim(),
                City = profile.City,
                About = profile.About,
                OpenToWork = profile.OpenToWork,
                PrimaryResumeId = primaryResume?.Id,
                PrimaryResumeTitle = primaryResume?.Title,
                DesiredPosition = primaryResume?.DesiredPosition,
                Summary = primaryResume?.Summary,
                EmploymentType = primaryResume == null ? null : (ContractEmploymentType)primaryResume.EmploymentType,
                WorkFormat = primaryResume == null ? null : (ContractWorkFormat)primaryResume.WorkFormat,
                ExperienceLevel = primaryResume == null ? null : (ContractExperienceLevel)primaryResume.ExperienceLevel,
                UpdatedAtUtc = updatedAtUtc,
                Skills = visibleResumes
                    .SelectMany(r => r.ResumeSkills)
                    .Where(rs => rs.Skill.DeletedAtUtc == null)
                    .GroupBy(rs => new { rs.SkillId, rs.Skill.Name })
                    .Select(group => new CandidateSearchSkillDTO
                    {
                        Id = group.Key.SkillId,
                        Name = group.Key.Name,
                        Level = (ContractSkillLevel)group.Max(rs => (int)rs.Level)
                    })
                    .OrderBy(skill => skill.Name)
                    .ToList()
            };
        }

        private static bool IsSearchableResume(DomainResume resume)
        {
            return resume.DeletedAtUtc == null &&
                   (resume.Visibility == DomainResumeVisibility.Public ||
                    resume.Visibility == DomainResumeVisibility.OnlyEmployers);
        }
    }
}
