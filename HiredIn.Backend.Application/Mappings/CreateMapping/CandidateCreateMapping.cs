using HiredIn.Backend.Contracts.DTOs.AuthDTOs;
using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class CandidateCreateMapping : AutoMapper.Profile
    {
        public CandidateCreateMapping()
        {
            CreateMap<RegisterCandidateRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLower()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => UserRole.Candidate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => UserStatus.Active))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.CandidateProfile, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyMembers, opt => opt.Ignore())
                .ForMember(dest => dest.SentMessages, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.FavouriteVacancies, opt => opt.Ignore())
                .ForMember(dest => dest.ChatParticipants, opt => opt.Ignore());

            CreateMap<RegisterCandidateRequestDto, CandidateProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.OpenToWork, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Resumes, opt => opt.Ignore());
        }
    }
}
