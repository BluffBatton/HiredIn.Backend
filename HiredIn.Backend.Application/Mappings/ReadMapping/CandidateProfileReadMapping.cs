using HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class CandidateProfileReadMapping : AutoMapper.Profile
    {
        CandidateProfileReadMapping()
        {
            CreateMap<CandidateProfile, CandidateProfileReadDTO>();
        }
    }
}
