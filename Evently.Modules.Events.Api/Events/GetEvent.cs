using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evently.Modules.Events.Api.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using static Evently.Modules.Events.Api.Events.CreateEvent;

namespace Evently.Modules.Events.Api.Events;
public static class GetEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events/{id}", async (
            Guid id,
            EventsDbContext context) =>
        {
            var eventResponse = await context.Events
            .Where(x => x.Id == id)
            .Select(x => new EventResponse(x.Id, x.Title, x.Description, x.Location, x.StartsAtUtc, x.EndsAtUtc))
            .SingleOrDefaultAsync();

            return eventResponse is null ? Results.NotFound() : Results.Ok(eventResponse);

        })
            .WithTags(Tags.Events);
    }
}
