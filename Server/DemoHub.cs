using Microsoft.AspNetCore.SignalR;
namespace Server;

public class DemoHub : Hub
{
    private const string DEFAULT_EVENT_NAME = "serverSend";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier; // Automatically calls IUserIdProvider.GetUserId(HubConnectionContext connection)         
        // If IUserIdProvider is not customized, it defaults to Context.User.FindFirst(ClaimTypes.NameIdentifier)

        // Custom logic (validation, logging, group join, etc.) can be added here.

        await Clients.Caller.SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, userId, "Clients.Caller");

        // Finally:
        await base.OnConnectedAsync();
    }

    public Task ProxyToAll(object data) =>
        Clients.All
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, data, ".ProxyToAll, .All");

    public Task ProxyToCaller(object data) =>
        Clients.Caller
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, data, ".ProxyToCaller, .Caller");

    public Task ProxyToOthers(object data) =>
        Clients.Others
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, data, ".ProxyToOthers, .Others");

    public Task ProxyToClient(object data, string connectionId) =>
        //Clients.Client(connectionId)
        //Clients.Clients(new List<string> { connectionId , })
        //Clients.Clients(new[] { connectionId })
        Clients.Clients([connectionId])  // require .NET 8
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, data, ".ProxyToClient, .Client(connectionId)");

    // The implementation of IUserIdProvider determines the relationship between userId and connection.
    public Task ProxyToUser(object data, string userId) =>
        //Clients.User(userId)
        //Clients.Users(userId, "u2")
        Clients.Users([userId])
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, new { data, userId }, ".ProxyToUser, .User(userId)");


    // group:
    public Task JoinGroup(string group) => Groups.AddToGroupAsync(Context.ConnectionId, group);
    public Task LeaveGroup(string group) => Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

    public Task ProxyToGroups(object data, string group) =>
        //Clients.Group(group)
        Clients.Groups([group])
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, new { data, group }, ".ProxyToGroups, .Groups(groupNames)");

    public Task ProxyToOthersInGroup(object data, string group) =>
        Clients.OthersInGroup(group)
        .SendAsync(DEFAULT_EVENT_NAME, Context.ConnectionId, new { data, group }, ".ProxyToOthersInGroup, .OthersInGroup(groupName)");
}

