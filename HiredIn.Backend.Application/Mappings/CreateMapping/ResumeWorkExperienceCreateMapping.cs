using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using DomainResumeWorkExperience = HiredIn.Backend.Domain.Entities.ResumeWorkExperience;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class ResumeWorkExperienceCreateMapping : Profile
    {
        public ResumeWorkExperienceCreateMapping()
        {
            CreateMap<ResumeWorkExperienceCreateDTO, DomainResumeWorkExperience>()
                .ForMember(dest => dest.Resume, opt => opt.Ignore());
        }
    }
}