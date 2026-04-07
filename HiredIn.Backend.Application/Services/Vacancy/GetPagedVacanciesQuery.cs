using AutoMapper;
using HiredIn.Backend.Application.Common.Models;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using MediatR;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class GetPagedVacanciesQuery : IRequest<PaginatedList<VacancyReadDTO>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class GetPagedVacanciesQueryHandler : IRequestHandler<GetPagedVacanciesQuery, PaginatedList<VacancyReadDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPagedVacanciesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _context = applicationDbContext;
            _mapper = mapper;
        }
        public async Task<PaginatedList<VacancyReadDTO>> Handle(GetPagedVacanciesQuery request, CancellationToken cancellationToken)
        {
            var vacanciesResponseQuery = _context.Vacancies;
                //.Skip((request.Page - 1) * request.PageSize)
                //.Take(request.PageSize)

            var vacanciesDtoQuery = _mapper.ProjectTo<VacancyReadDTO>(vacanciesResponseQuery);
            var paginatedVacancies = await PaginatedList<VacancyReadDTO>.CreateAsync(vacanciesDtoQuery, request.Page, request.PageSize, cancellationToken);

            return paginatedVacancies;
        }
    }
}
