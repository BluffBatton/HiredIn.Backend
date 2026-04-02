using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class GetMyVacanciesQuery : IRequest<List<VacancyReadDTO>>
    {
    }

    public class GetMyVacanciesQueryHandler : IRequestHandler<GetMyVacanciesQuery, List<VacancyReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetMyVacanciesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<VacancyReadDTO>> Handle(GetMyVacanciesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var companyIds = await _context.CompanyMembers
                .Where(cm => cm.UserId == currentUserId.Value)
                .Select(cm => cm.CompanyId)
                .ToListAsync(cancellationToken);

            var vacancies = await _context.Vacancies
                .Where(v => companyIds.Contains(v.CompanyId) && v.DeletedAtUtc == null)
                .Include(v => v.Company)
                .OrderByDescending(v => v.UpdatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VacancyReadDTO>>(vacancies);
        }
    }
}