using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class ApplicationReadMapping : Profile
    {
        public ApplicationReadMapping()
        {
            CreateMap<Domain.Entities.Application, ApplicationReadDTO>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.ApplicationStatus)src.Status))
                .ForMember(dest => dest.VacancyTitle,
                    opt => opt.MapFrom(src => src.Vacancy.Title))
                .ForMember(dest => dest.ResumeTitle,
                    opt => opt.MapFrom(src => src.Resume.Title))
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Vacancy.Company.Name))
                .ForMember(dest => dest.SalaryMin,
                    opt => opt.MapFrom(src => src.Vacancy.SalaryMin))
                .ForMember(dest => dest.SalaryMax,
                    opt => opt.MapFrom(src => src.Vacancy.SalaryMax))
                .ForMember(dest => dest.City,
                    opt => opt.MapFrom(src => src.Vacancy.City));
        }
    }
}
