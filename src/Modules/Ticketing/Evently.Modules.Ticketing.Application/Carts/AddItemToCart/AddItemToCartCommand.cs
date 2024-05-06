using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Events.PublicApi;
using Evently.Modules.Ticketing.Domain.Customers;
using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Users.PublicApi;
using FluentValidation;
using ICommand = Evently.Common.Application.Messaging.ICommand;

namespace Evently.Modules.Ticketing.Application.Carts.AddItemToCart;
public sealed record AddItemToCartCommand(Guid CustomerId, Guid TicketTypeId, decimal Quantity) : ICommand;

internal sealed class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.TicketTypeId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(decimal.Zero);
    }
}

internal sealed class AddItemToCartCommandHandler(
    CartService cartService,
    IUsersApi usersApi,
    IEventsApi eventsApi) : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var customer = await usersApi.GetAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(CustomerErrors.NotFound(request.CustomerId));
        }

        var ticketType = await eventsApi.GetTicketTypeAsync(request.TicketTypeId, cancellationToken);

        if (ticketType is null)
        {
            return Result.Failure(TicketTypeErrors.NotFound(request.TicketTypeId));
        }

        var cartItem = new CartItem
        {
            TicketTypeId = ticketType.Id,
            Price = ticketType.Price,
            Currency = ticketType.Currency,
            Quantity = request.Quantity
        };

        await cartService.AddItemAsync(customer.Id, cartItem, cancellationToken);

        return Result.Success();
    }
}
