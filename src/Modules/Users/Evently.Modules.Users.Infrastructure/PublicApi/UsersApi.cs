using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evently.Modules.Users.Application.Users.GetUser;
using Evently.Modules.Users.PublicApi;
using MediatR;
using UserResponse = Evently.Modules.Users.PublicApi.UserResponse;

namespace Evently.Modules.Users.Infrastructure.PublicApi;
internal sealed class UsersApi(
    ISender _sender) : IUsersApi
{
    public async Task<UserResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userResponse = await _sender.Send(new GetUserQuery(userId), cancellationToken);

        if (userResponse.IsFailure)
        {
            return null;
        }

        return new UserResponse(
            userResponse.Value.Id,
            userResponse.Value.Email,
            userResponse.Value.FirstName,
            userResponse.Value.LastName);
    }
}
