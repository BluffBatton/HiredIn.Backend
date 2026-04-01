using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs;
using DomainResumeSkill = HiredIn.Backend.Domain.Entities.ResumeSkill;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class ResumeSkillReadMapping : Profile
    {
        public ResumeSkillReadMapping()
        {
            CreateMap<DomainResumeSkill, ResumeSkillReadDTO>()
                .ForMember(dest => dest.SkillName,
                    opt => opt.MapFrom(src => src.Skill.Name))
                .ForMember(dest => dest.Level,
                    opt => opt.MapFrom(src => (HiredIn.Backend.Contracts.DTOs.Enums.SkillLevel)src.Level));
        }
    }
}