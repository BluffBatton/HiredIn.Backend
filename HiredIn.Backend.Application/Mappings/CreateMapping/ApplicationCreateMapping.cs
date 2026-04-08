namespace HiredIn.Backend.Application.Mappings.CreateMapping
{
    public class ApplicationCreateMapping : AutoMapper.Profile
    {
        public ApplicationCreateMapping()
        {
            CreateMap<Contracts.DTOs.ApplicationDTOs.ApplicationCreateDTO, Domain.Entities.Application>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
