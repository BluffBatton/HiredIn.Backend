using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class ResumeFileReadMapping : Profile
    {
        public ResumeFileReadMapping()
        {
            CreateMap<ResumeFile, ResumeFileReadDTO>();
        }
    }
}