using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using DomainSkill = HiredIn.Backend.Domain.Entities.Skill;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class SkillCreateMapping : Profile
    {
        public SkillCreateMapping()
        {
            CreateMap<SkillCreateDTO, DomainSkill>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ResumeSkills, opt => opt.Ignore())
                .ForMember(dest => dest.VacancySkills, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());
        }
    }
}