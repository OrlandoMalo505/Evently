using Evently.Common.Domain.Abstractions;
using Evently.Modules.Events.Application.TicketTypes.UpdateTicketTypePrice;
using Evently.Common.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Evently.Common.Presentation.Endpoints;

namespace Evently.Modules.Events.Presentation.TicketTypes;

internal sealed class ChangeTicketTypePrice : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("ticket-types/{id}/price", async (Guid id, ChangeTicketTypePriceRequest request, ISender sender) =>
            {
                Result result = await sender.Send(new UpdateTicketTypePriceCommand(id, request.Price));

                return result.Match(Results.NoContent, Common.Presentation.ApiResults.ApiResults.Problem);
            })
            .WithTags(Tags.TicketTypes);
    }

    internal sealed class ChangeTicketTypePriceRequest
    {
        public decimal Price { get; init; }
    }
}
