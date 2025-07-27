using Microsoft.AspNetCore.SignalR;
using Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(); // Depend on Nuget package: Microsoft.AspNetCore.SignalR 
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // Once an IUserIdProvider implementation is registered, it supports all hubs.


var app = builder.Build();

app.UseDefaultFiles();  // for wwwroot/index.html
app.UseStaticFiles();

app.MapHub<DemoHub>("/hub"); // Register Hub URL

app.Run();
