﻿using Evently.Common.Domain;
using Evently.Common.Domain.Abstractions;

namespace Evently.Modules.Users.Domain.Users;

public sealed class UserProfileUpdatedDomainEvent(Guid userId, string firstName, string lastName) : DomainEvent
{
    public Guid UserId { get; init; } = userId;

    public string FirstName { get; init; } = firstName;

    public string LastName { get; init; } = lastName;
}
