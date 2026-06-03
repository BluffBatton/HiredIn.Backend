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
                    opt => opt.MapFrom(src => src.Vacancy.City))
                .ForMember(dest => dest.CandidateProfileId,
                    opt => opt.MapFrom(src => src.Resume.CandidateProfileId))
                .ForMember(dest => dest.CandidateName,
                    opt => opt.MapFrom(src => (src.Resume.CandidateProfile.User.FirstName + " " + src.Resume.CandidateProfile.User.LastName).Trim()))
                .ForMember(dest => dest.CandidateCity,
                    opt => opt.MapFrom(src => src.Resume.CandidateProfile.City))
                .ForMember(dest => dest.CandidateAbout,
                    opt => opt.MapFrom(src => src.Resume.CandidateProfile.About))
                .ForMember(dest => dest.CandidateOpenToWork,
                    opt => opt.MapFrom(src => src.Resume.CandidateProfile.OpenToWork))
                .ForMember(dest => dest.DesiredPosition,
                    opt => opt.MapFrom(src => src.Resume.DesiredPosition))
                .ForMember(dest => dest.EmploymentType,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.EmploymentType)src.Resume.EmploymentType))
                .ForMember(dest => dest.WorkFormat,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.WorkFormat)src.Resume.WorkFormat))
                .ForMember(dest => dest.ExperienceLevel,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.ExperienceLevel)src.Resume.ExperienceLevel));
        }
    }
}
