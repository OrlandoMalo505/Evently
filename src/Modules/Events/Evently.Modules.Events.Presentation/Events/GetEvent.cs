using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evently.Modules.Events.Application.Events;
using Evently.Modules.Events.Application.Events.GetEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;
internal static class GetEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var query = new GetEventQuery(id);

            var eventResponse = await sender.Send(query);

            return eventResponse is null ? Results.NotFound() : Results.Ok(eventResponse);

        })
            .WithTags(Tags.Events);
    }
}
