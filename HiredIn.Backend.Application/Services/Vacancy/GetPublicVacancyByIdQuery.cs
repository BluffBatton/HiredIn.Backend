using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class GetPublicVacancyByIdQuery : IRequest<VacancyReadDTO>
    {
        public Guid Id { get; set; }

        public GetPublicVacancyByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetPublicVacancyByIdQueryHandler : IRequestHandler<GetPublicVacancyByIdQuery, VacancyReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPublicVacancyByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<VacancyReadDTO> Handle(GetPublicVacancyByIdQuery request, CancellationToken cancellationToken)
        {
            var vacancy = await _context.Vacancies
                .AsNoTracking()
                .Include(v => v.Company)
                .Where(v =>
                    v.Id == request.Id &&
                    v.DeletedAtUtc == null &&
                    v.Status == VacancyStatus.Published &&
                    v.Company.DeletedAtUtc == null &&
                    v.Company.Status == CompanyStatus.Active)
                .FirstOrDefaultAsync(cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            return _mapper.Map<VacancyReadDTO>(vacancy);
        }
    }
}
