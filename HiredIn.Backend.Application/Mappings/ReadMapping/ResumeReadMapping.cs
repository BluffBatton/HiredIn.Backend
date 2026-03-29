using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class ResumeReadMapping : Profile
    {
        public ResumeReadMapping()
        {
            CreateMap<Resume, ResumeReadDTO>()
                .ForMember(dest => dest.EmploymentType,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.EmploymentType)src.EmploymentType))
                .ForMember(dest => dest.WorkFormat,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.WorkFormat)src.WorkFormat))
                .ForMember(dest => dest.ExperienceLevel,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.ExperienceLevel)src.ExperienceLevel))
                .ForMember(dest => dest.Visibility,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.ResumeVisibility)src.Visibility));
        }
    }
}