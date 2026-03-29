using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class CompanyCreateMapping : AutoMapper.Profile
    {
        public CompanyCreateMapping() 
        {
            CreateMap<CompanyCreateDTO, Company>();
        }
    }
}