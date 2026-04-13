using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using DomainVacancy = HiredIn.Backend.Domain.Entities.Vacancy;

namespace HiredIn.Backend.Application.Mappings.UpdateMapping
{
    public class VacancyUpdateMapping : Profile
    {
        public VacancyUpdateMapping()
        {
            CreateMap<VacancyUpdateDTO, DomainVacancy>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.VacancySkills, opt => opt.Ignore())
                .ForMember(dest => dest.Applications, opt => opt.Ignore())
                .ForMember(dest => dest.FavouriteVacancies, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentType,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.EmploymentType)src.EmploymentType))
                .ForMember(dest => dest.WorkFormat,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.WorkFormat)src.WorkFormat))
                .ForMember(dest => dest.ExperienceLevel,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.ExperienceLevel)src.ExperienceLevel))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.VacancyStatus)src.Status));
        }
    }
}