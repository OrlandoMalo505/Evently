using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evently.Modules.Events.Application.TicketTypes.GetTicketType;
using Evently.Modules.Events.PublicApi;
using MediatR;
using TicketTypeResponse = Evently.Modules.Events.PublicApi.TicketTypeResponse;

namespace Evently.Modules.Events.Presentation.PublicApi;
internal sealed class EventsApi(
    ISender _sender) : IEventsApi
{
    public async Task<TicketTypeResponse?> GetTicketTypeAsync(Guid ticketTypeId, CancellationToken cancellationToken = default)
    {
        var response = await _sender.Send(new GetTicketTypeQuery(ticketTypeId), cancellationToken);

        if (response.IsFailure)
        {
            return null;
        }

        return new TicketTypeResponse(
            response.Value.Id,
            response.Value.EventId,
            response.Value.Name,
            response.Value.Price,
            response.Value.Currency,
            response.Value.Quantity);
    }
}
