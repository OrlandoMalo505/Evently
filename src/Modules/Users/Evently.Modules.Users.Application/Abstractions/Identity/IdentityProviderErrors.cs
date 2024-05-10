﻿using Evently.Common.Domain;
using Evently.Common.Domain.Abstractions;

namespace Evently.Modules.Users.Application.Abstractions.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error EmailIsNotUnique = Error.Conflict(
        "Identity.EmailIsNotUnique",
        "The specified email is not unique.");
}
