using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evently.Modules.Events.Api.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Api.Events;
public static class CreateEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("events", async (
            Request req,
            EventsDbContext context) =>
        {
            var @event = new Event
            {
                Id = Guid.NewGuid(),
                Title = req.Title,
                Description = req.Description,
                Location = req.Location,
                StartsAtUtc = req.StartsAtUtc,
                EndsAtUtc = req.EndsAtUtc,
                Status = EventStatus.Draft
            };

            context.Events.Add(@event);

            await context.SaveChangesAsync();

            return Results.Ok(@event.Id);

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
