# Implement SignalR in ASP.NET Core 8

## Server

- **NuGet Package:**
    - `Microsoft.AspNetCore.SignalR`

- **Configure services in `Program.cs`:**
    ```csharp
    builder.Services.AddSignalR();
    ```

- **Hub built-in mechanisms:**
    ```csharp
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        // Automatically calls IUserIdProvider.GetUserId(...).
        // If IUserIdProvider is not customized, it defaults to:
        // Context.User.FindFirst(ClaimTypes.NameIdentifier)

        // Custom logic (validation, logging, group joining, etc.) can be added here.

        await base.OnConnectedAsync();
    }

    /* Trigger client events via:
    Clients
        .All	
        .Caller	
        .Others	
        .Client(connectionId)	

        // related to the implementation of IUserIdProvider
        .User(userId)   
        .Users(new[] { userId })      
        
        .Group(groupName)	
        .Groups(new[] { groupName })	
        .OthersInGroup(group)	

            .SendAsync("some event name", arguments)
    */
    ```

- **Map the Hub:**
    ```csharp
    app.MapHub<DemoHub>("/hub");
    ```

- *(For simplified testing)* implement `IUserIdProvider` to control how SignalR associates connections with user IDs.  
  If `IUserIdProvider` is not implemented, it defaults to `Context.User.FindFirst(ClaimTypes.NameIdentifier)`.

    ```csharp
    public class CustomUserIdProvider : IUserIdProvider
    {
        // Returns the userId.
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext()?.Request.Query["userId"];
        }
    }

    // Once registered, this applies to all hubs.
    builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
    ```

---

## Client (JavaScript)

The JS client library can be fetched using **LibMan**, provider: `cdnjs`, search for `"signalr"`.

- **Connect:**
    ```js
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`/hub?userId=${userId.value}`) // The userId is for the custom IUserIdProvider.
        .build();

    connection.start();
    ```

- **Listen for server events:**
    ```js
    connection.on("someEventName", eventHandler);
    ```

- **Invoke hub methods:**
    ```js
    connection.invoke("hubMethodName", arguments);
    ```
