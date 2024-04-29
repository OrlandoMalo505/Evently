﻿using Evently.Modules.Events.Application.Abstractions.Data;
using Evently.Modules.Events.Domain.Events;
using MediatR;

namespace Evently.Modules.Events.Application.Events.CreateEvent;

internal sealed class CreateEventCommandHandler(
    IEventRepository _eventRepository,
    IUnitOfWork _unitOfWork) : IRequestHandler<CreateEventCommand, Guid>
{
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = Event.Create(
           request.Title,
           request.Description,
           request.Location,
           request.StartsAtUtc,
           request.EndsAtUtc);

        _eventRepository.Insert(@event);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}
