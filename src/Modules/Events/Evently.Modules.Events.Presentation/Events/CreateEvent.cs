﻿using Evently.Modules.Events.Application.Events;
using Evently.Modules.Events.Application.Events.CreateEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;
internal static class CreateEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("events", async (Request req, ISender sender) =>
        {

            var command = new CreateEventCommand(
                req.Title,
                req.Description,
                req.Location,
                req.StartsAtUtc,
                req.EndsAtUtc);

            var eventId = await sender.Send(command);

            return Results.Ok(eventId);

        })
            .WithTags(Tags.Events);
    }

    internal sealed class Request
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
    }
}
