using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs;
using DomainResumeSkill = HiredIn.Backend.Domain.Entities.ResumeSkill;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class ResumeSkillCreateMapping : Profile
    {
        public ResumeSkillCreateMapping()
        {
            CreateMap<ResumeSkillCreateDTO, DomainResumeSkill>()
                .ForMember(dest => dest.Resume, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.Ignore())
                .ForMember(dest => dest.Level,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Domain.Enums.SkillLevel)src.Level));
        }
    }
}