using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class ResumeCreateMapping : Profile
    {
        public ResumeCreateMapping()
        {
            CreateMap<ResumeCreateDTO, Resume>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.ResumeFile, opt => opt.Ignore())
                .ForMember(dest => dest.Educations, opt => opt.Ignore())
                .ForMember(dest => dest.WorkExperiences, opt => opt.Ignore())
                .ForMember(dest => dest.ResumeSkills, opt => opt.Ignore())
                .ForMember(dest => dest.Applications, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentType,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.EmploymentType)src.EmploymentType))
                .ForMember(dest => dest.WorkFormat,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.WorkFormat)src.WorkFormat))
                .ForMember(dest => dest.ExperienceLevel,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.ExperienceLevel)src.ExperienceLevel))
                .ForMember(dest => dest.Visibility,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.ResumeVisibility)src.Visibility));
        }
    }
}