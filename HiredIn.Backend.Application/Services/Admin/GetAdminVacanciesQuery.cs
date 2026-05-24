using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class GetAdminVacanciesQuery : IRequest<List<VacancyReadDTO>>
    {
    }

    public class GetAdminVacanciesQueryHandler : IRequestHandler<GetAdminVacanciesQuery, List<VacancyReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAdminVacanciesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VacancyReadDTO>> Handle(GetAdminVacanciesQuery request, CancellationToken cancellationToken)
        {
            var vacancies = await _context.Vacancies
                .AsNoTracking()
                .Include(v => v.Company)
                .Where(v => v.DeletedAtUtc == null)
                .OrderByDescending(v => v.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<VacancyReadDTO>>(vacancies);
        }
    }
}
