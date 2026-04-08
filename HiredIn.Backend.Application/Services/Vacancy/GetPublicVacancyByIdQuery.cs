using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
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
            var vacancy = await _context.Vacancies.
                Where(v => v.Id == request.Id && v.Status == Domain.Enums.VacancyStatus.Published).
                FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<VacancyReadDTO>(vacancy);
        }
    }
}
