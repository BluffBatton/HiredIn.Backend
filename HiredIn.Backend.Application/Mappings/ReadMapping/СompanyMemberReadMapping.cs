using HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class СompanyMemberReadMapping : AutoMapper.Profile
    {
        public СompanyMemberReadMapping()
        {
            CreateMap<CompanyMember, CompanyMemberReadDTO>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}")
                )
                .ForMember(
                    dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company.Name)
                )
                .ForMember(
                    dest => dest.Role,
                    opt => opt.MapFrom(src =>
                        Enum.Parse<Contracts.DTOs.Enums.CompanyMemberRole>(src.Role.ToString()))
                );
        }
    }
}
