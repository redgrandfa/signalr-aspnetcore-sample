using Microsoft.AspNetCore.SignalR;

namespace Server;

public class CustomUserIdProvider : IUserIdProvider
{
    // Returns the userId. If IUserIdProvider is not customized, it defaults to Context.User.FindFirst(ClaimTypes.NameIdentifier).
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.GetHttpContext()?.Request.Query["userId"]; // Get from query string
    }
}