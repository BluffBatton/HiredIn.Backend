using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class ResumeFileCreateMapping : Profile
    {
        public ResumeFileCreateMapping()
        {
            CreateMap<ResumeFileCreateDTO, ResumeFile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Resume, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());
        }
    }
}