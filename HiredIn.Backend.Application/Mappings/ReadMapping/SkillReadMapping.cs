using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.SkillDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class SkillReadMapping : Profile
    {
        public SkillReadMapping()
        {
            CreateMap<Skill, SkillReadDTO>();
        }
    }
}