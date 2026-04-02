using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using DomainVacancy = HiredIn.Backend.Domain.Entities.Vacancy;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class VacancyReadMapping : Profile
    {
        public VacancyReadMapping()
        {
            CreateMap<DomainVacancy, VacancyReadDTO>()
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.EmploymentType,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.EmploymentType)src.EmploymentType))
                .ForMember(dest => dest.WorkFormat,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.WorkFormat)src.WorkFormat))
                .ForMember(dest => dest.ExperienceLevel,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.ExperienceLevel)src.ExperienceLevel))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.VacancyStatus)src.Status));
        }
    }
}