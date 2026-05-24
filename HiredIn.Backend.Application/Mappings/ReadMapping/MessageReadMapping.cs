using AutoMapper;
using HiredIn.Backend.Contracts.MessagesDTOs;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Mappings.ReadMapping
{
    public class MessageReadMapping : Profile
    {
        public MessageReadMapping()
        {
            CreateMap<Message, MessageReadDTO>()
                .ForMember(dest => dest.Content,
                    opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.SenderFullName,
                    opt => opt.MapFrom(src => $"{src.SenderUser.FirstName} {src.SenderUser.LastName}"));
        }
    }
}
