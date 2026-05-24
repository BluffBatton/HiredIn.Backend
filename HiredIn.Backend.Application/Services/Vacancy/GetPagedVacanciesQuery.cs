using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiredIn.Backend.Application.Common.Models;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var pageIndex = request.Page < 0 ? 0 : request.Page;
            var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);

            var vacanciesResponseQuery = _context.Vacancies
                .AsNoTracking()
                .Where(v =>
                    v.DeletedAtUtc == null &&
                    v.Status == VacancyStatus.Published &&
                    v.Company.DeletedAtUtc == null &&
                    v.Company.Status == CompanyStatus.Active);

            var vacanciesDtoQuery = _mapper.ProjectTo<VacancyReadDTO>(vacanciesResponseQuery);
            var paginatedVacancies = await PaginatedList<VacancyReadDTO>.CreateAsync(
                vacanciesDtoQuery,
                pageIndex,
                pageSize,
                cancellationToken);

            return paginatedVacancies;
        }
    }
}
