using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class ResumeEducationReadMapping : Profile
    {
        public ResumeEducationReadMapping()
        {
            CreateMap<ResumeEducation, ResumeEducationReadDTO>();
        }
    }
}