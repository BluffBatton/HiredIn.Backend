using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.UpdateMapping
{
    public class ResumeFilePatchMapping : Profile
    {
        public ResumeFilePatchMapping()
        {
            CreateMap<ResumeFilePatchDTO, ResumeFile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ResumeId, opt => opt.Ignore())
                .ForMember(dest => dest.Resume, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}