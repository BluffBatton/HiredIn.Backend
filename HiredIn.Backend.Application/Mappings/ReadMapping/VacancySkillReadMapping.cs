using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs;
using DomainVacancySkill = HiredIn.Backend.Domain.Entities.VacancySkill;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class VacancySkillReadMapping : Profile
    {
        public VacancySkillReadMapping()
        {
            CreateMap<DomainVacancySkill, VacancySkillReadDTO>()
                .ForMember(dest => dest.SkillName,
                    opt => opt.MapFrom(src => src.Skill.Name));
        }
    }
}