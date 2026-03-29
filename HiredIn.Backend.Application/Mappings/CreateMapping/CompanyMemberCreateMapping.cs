using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class CompanyMemberCreateMapping : Profile
    {
        public CompanyMemberCreateMapping()
        {
            CreateMap<CompanyMemberCreateDTO, CompanyMember>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(
                    dest => dest.Role,
                    opt => opt.MapFrom(src =>
                        Enum.Parse<HiredIn.Backend.Domain.Enums.CompanyMemberRole>(src.Role.ToString()))
                );
        }
    }
}