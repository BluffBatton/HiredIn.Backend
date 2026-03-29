
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.UpdateMapping
{
    public class CompanyUpdateMapping : AutoMapper.Profile
    {
        public CompanyUpdateMapping()
        {
            CreateMap<CompanyUpdateDTO, Company>();
        }
    }
}
