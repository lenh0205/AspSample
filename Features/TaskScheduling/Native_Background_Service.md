> native background service does not handle scheduling, but can implement task scheduling within a native background service

# Native Background Service
* -> 
* => is perfect for **`long-running background process`**
* => **periodic tasks** (like refreshing cache, processing logs, or sending notifications) but **not scheduled jobs with specific timing** (e.g., "Run this at exactly 2 AM" → use Quartz.NET or Hangfire instead)

# 'IHostedService' or 'BackgroundService'
* -> this is mainly implementation of **`Native Background Service`** in .NET 
* -> is more related to **background jobs** but can use for **task scheduling** as well

```
// Example: A simple background service that runs a task every 10 seconds:

// -> register:
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<MyBackgroundService>();

var app = builder.Build();
app.Run();

// -> usage:
public class MyBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Background task running at: {DateTime.Now}");
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
```
