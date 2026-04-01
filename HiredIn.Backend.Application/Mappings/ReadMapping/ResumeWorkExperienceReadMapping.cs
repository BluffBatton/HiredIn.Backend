using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using DomainResumeWorkExperience = HiredIn.Backend.Domain.Entities.ResumeWorkExperience;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class ResumeWorkExperienceReadMapping : Profile
    {
        public ResumeWorkExperienceReadMapping()
        {
            CreateMap<DomainResumeWorkExperience, ResumeWorkExperienceReadDTO>();
        }
    }
}