using AutoMapper;
using HiredIn.Backend.Contracts.DTOs.NotificationDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class NotificationReadMapping : Profile
    {
        public NotificationReadMapping()
        {
            CreateMap<Notification, NotificationReadDTO>();
        }
    }
}