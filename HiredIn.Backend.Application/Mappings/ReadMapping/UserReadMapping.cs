using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.UserDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping;

public class UserReadMapping : Profile
{
    public UserReadMapping()
    {
        CreateMap<User, UserReadDTO>()
            .ForMember(dest => dest.UserRole,
                opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.UserStatus,
                opt => opt.MapFrom(src => src.Status));
    }
}