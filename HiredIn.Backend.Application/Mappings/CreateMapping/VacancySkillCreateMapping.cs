using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs;
using DomainVacancySkill = HiredIn.Backend.Domain.Entities.VacancySkill;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class VacancySkillCreateMapping : Profile
    {
        public VacancySkillCreateMapping()
        {
            CreateMap<VacancySkillCreateDTO, DomainVacancySkill>()
                .ForMember(dest => dest.Vacancy, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.Ignore());
        }
    }
}