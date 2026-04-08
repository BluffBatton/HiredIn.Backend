using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using HiredIn.Backend.Domain.Entities;
using MediatR;

namespace HiredIn.Backend.Application.Services.Application
{
    public class CreateApplicationCommand : IRequest
    {
        public ApplicationCreateDTO Application { get; set; }

        public CreateApplicationCommand(ApplicationCreateDTO application)
        {
            Application = application;
        }
    }

    public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateApplicationCommandHandler(IApplicationDbContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = _mapper.Map<Domain.Entities.Application>(request.Application);
            await _context.Applications.AddAsync(application, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }
    }
}
