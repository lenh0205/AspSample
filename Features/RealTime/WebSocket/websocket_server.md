> https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-9.0
> https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-9.0&tabs=visual-studio

# WebSocket Server 

## "UseWebSockets" ASP.NET Core middleware
* -> Detect WebSocket Requests: It checks if an incoming HTTP request is a WebSocket handshake request (Ex: wss://127.0.0.1:8987/SignCopy) via headers (like Upgrade: websocket)
* -> Upgrade the Connection: If the request is valid, it upgrades the HTTP connection to a WebSocket connection and passes control to your application logic (e.g., the WebSocket handlers).
* -> Manage Protocol Details: It handles lower-level WebSocket protocol operations, ensuring proper communication between clients and the application

* _does not manage WebSocket connections or act as the server itself. It merely provides the functionality required to establish and use WebSocket connections_
* The ASP.NET Core application (running on Kestrel or another web server) plays the role of the WebSocket server
* individual WebSocket connections (the WebSocket instance) for specific client are created by the WebSocket server by calling **AcceptWebSocketAsync()** when a client connects

## Action
* -> SendAsync: Sends messages to only one specific client (the WebSocket instance passed to the method).
* -> CloseAsync: Closes the connection for only one specific client
* -> The WebSocket server remains unaffected unless the entire ASP.NET Core application is stopped

* => để mà send a message to all connected clients
* => ta sẽ cần to maintain a list of active WebSocket connections and iterate over it to call SendAsync for each client


## Example
```cs - ASP.NET Core
// program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();
// app.UseWebSockets(new WebSocketOptions
// {
//     KeepAliveInterval = TimeSpan.FromSeconds(120) // to specific life time
// });

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Map("/ws/endpoint1", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketCommunication(webSocket, "Endpoint1");
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Map("/ws/endpoint2", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketCommunication(webSocket, "Endpoint2");
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();

// method to handle WebSocket communication for each endpoint
private static async Task HandleWebSocketCommunication(WebSocket webSocket, string endpointName)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result;

    Console.WriteLine($"WebSocket connection established on {endpointName}");

    while (webSocket.State == WebSocketState.Open)
    {
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine($"WebSocket connection closed on {endpointName}");
        }
        else if (result.MessageType == WebSocketMessageType.Text)
        {
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Message from {endpointName}: {receivedMessage}");

            var responseMessage = $"Echo from {endpointName}: {receivedMessage}";
            var responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
```
```cs - hoặc ta cũng có thể làm kiểu này
public class WebSocketController : ControllerBase
{
  [Route("/ws")]
  public async Task Get()
  {
      if (HttpContext.WebSockets.IsWebSocketRequest)
      {
          using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
          await Echo(webSocket);
      }
      else
      {
          HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
      }
  }
}
```

```javascript - to connect from a web client to these endpoints
const ws1 = new WebSocket("ws://localhost:5000/ws/endpoint1");
ws1.onopen = () => {
    console.log("Connected to Endpoint1");
    ws1.send("Hello from client!");
};
ws1.onmessage = (event) => {
    console.log("Message from Endpoint1:", event.data);
};

const ws2 = new WebSocket("ws://localhost:5000/ws/endpoint2");
ws2.onopen = () => {
    console.log("Connected to Endpoint2");
    ws2.send("Hello from client!");
};
ws2.onmessage = (event) => {
    console.log("Message from Endpoint2:", event.data);
};
```

## Using 'SignalR'
* -> built on top of **`WebSockets`** and also fallback support like **`Server-Sent Events`**, **`Long Polling`** to simplify **real-time communication**
* -> ta sẽ không cần phải manually handle all connection management, routing, and messaging logic
* -> provide features like **`broadcasting`**, **`client group management`**, and **`automatic reconnections`**

```cs - SignalR
// program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Add SignalR to the services
builder.Services.AddSignalR();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MyHub>("/myhub"); // Register SignalR hub
});

app.Run();

// Hub
public class MyHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

```

```javascript
// npm install @microsoft/signalr

// -> SignalR approach
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/myhub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
});

connection.start().then(() => {
    console.log("Connected to SignalR hub");
    connection.invoke("SendMessage", "User1", "Hello from the client!");
});

// -> still able to WebSocket approach
const ws = new WebSocket('ws://localhost:5000/ws/SignApproved');
ws.send('message');
```