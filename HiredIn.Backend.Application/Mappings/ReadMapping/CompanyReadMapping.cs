using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class CompanyReadMapping : AutoMapper.Profile
    {
        public CompanyReadMapping() 
        {
            CreateMap<Company, CompanyReadDTO>();
        }
    }
}
