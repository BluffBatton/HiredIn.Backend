using HiredIn.Backend.Contracts.DTOs.CompanyRatingDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class CompanyRatingCreateMapping : AutoMapper.Profile
    {
        public CompanyRatingCreateMapping()
        {
            CreateMap<CompanyRatingCreateDTO, CompanyRating>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Company, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore());
        }
    }
}