using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.UpdateMapping
{
    public class CandidateProfileUpdateMapping : AutoMapper.Profile
    {
        public CandidateProfileUpdateMapping()
        {
            CreateMap<CandidateProfileUpdateDTO, CandidateProfile>();
        }
    }
}
