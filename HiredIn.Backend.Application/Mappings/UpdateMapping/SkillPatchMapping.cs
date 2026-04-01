using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using DomainSkill = HiredIn.Backend.Domain.Entities.Skill;

namespace HiredIn.Backend.Application.Mappings.UpdateMapping
{
    public class SkillPatchMapping : Profile
    {
        public SkillPatchMapping()
        {
            CreateMap<SkillPatchDTO, DomainSkill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ResumeSkills, opt => opt.Ignore())
                .ForMember(dest => dest.VacancySkills, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}