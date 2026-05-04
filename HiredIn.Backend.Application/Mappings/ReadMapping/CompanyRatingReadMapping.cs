using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class CompanyRatingReadMapping : AutoMapper.Profile
    {
        public CompanyRatingReadMapping()
        {
            CreateMap<CompanyRating, CompanyRatingReadDTO>()
                .ForMember(dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Company.Name))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
        }
    }
}