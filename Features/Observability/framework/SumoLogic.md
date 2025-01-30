> Enrichment là gì ?

=========================================================
# Sumo Logic
* -> a log management and analytics platform to monitor, troubleshoot and secure apps

## Sumo Logic and OpenTelemetry
* -> OpenTelemetry is used to monitor distributed systems and cloud-native applications
* -> **developers and operators use OTel** to **`collect telemetry data from systems into tools like 'Sumo Logic'`**
* => the data is then analyzed to identify performance bottlenecks, troubleshoot errors, and resolve outages; Security teams use telemetry data to understand security posture and investigate breaches

* -> **`Sumo Logic's OpenTelemetry Collector`** built on **`OpenTelemetry`** that provides a **single unified agent to send logs, metrics, traces, and metadata for observability to Sumo Logic**
* -> it can be easily deployed as **a containerized application** on any cloud platform, and it **`supports a wide range of data sources, including AWS CloudWatch, Prometheus, and Jaeger`**
* -> this means that organizations can use the collector to gain deeper observability data across their systems, no matter where they are hosted

* -> once the data is collected, the **Sumo Logic platform** provides **`powerful analytics capabilities, enabling users to gain insights into their applications and systems, troubleshoot issues, and optimize their operations`**
* -> with its user-friendly interface and powerful features, the **Sumo Logic OTel Collector** is an ideal choice for organizations looking to understand their systems with better visibility and improve their overall performance and reliability

=========================================================
# Example: config 'Serilog' and 'SumoLogic' in .NET 6 Web API

## Knowledge
* -> **example purpose**: able to push application logs to SumoLogic as a structed data

* -> **`Serilog`** is a logging library for .NET
* -> support **structured data** like **JSON format**, **`different from conventional .NET log it doesn't come with the structure data`**
* -> structured data allow to **query data easily** and **create more meaningful metrics and help to troubleshoot** 
* -> support logging to **File**, **Console**, other platforms like **SumoLogic**, **AWS CloudWatch**, **AWS S3**, .....

* **`SumoLogic`** provides best-in-class cloud monitoring, log management

## 'SumoLogic' preparation
* -> ta cần truy cập vào website SumoLogic với tài khoản của ta
* -> first, we need to create a **`HTTP collector endpoint`** in SumoLogic
* -> Menu -> Manage Data -> Collection
* -> có 2 cách để tạo "HTTP Source collector endpoint" là:

* cách 1 là **Setup Wizard** -> chọn "Integrate with Sumo Logic" -> chọn "Your Custom App" -> chọn "HTTPS Source" 
* -> đặt tên cho cái endpoint này (Source Category) - nó có thể được dùng sau này để filter the logs to narrow the search and query
* -> rồi "Next"
* => this will give us a **`HTTP Source URL`** - that we will use in our **.NET application**
* -> Copy cái URL này rồi "Next" -> close the window -> refresh lại "Collection" section

* cách 2 là **Add Collector** -> chọn "Hosted Collector" -> cho nó tên -> Save 
* => nó sẽ tạo ra 1 **Collector** mới và 1 collector thì có thể có multiple sources
* -> click **Add Source** -> chọn **HTTP Logs & Metrics** -> cho nó 1 cái tên -> Save
* => nó sẽ cho ta cái **`HTTP Source Address`** -> ta copy lại

* -> giờ ta sẽ chọn vào **Collector** ta muốn -> click "Edit" để cho nó cái tên phù hợp và "Show URL" để lấy Collector endpoint

## .NET application preparation
* -> **dotnet new webapi**

* -> by default, .NET Web API framework come with the **logging provider**
* -> the **ILogger** come from **`Microsoft.Extensions.Logging`**

```cs
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecaseController> _logger;

    public WeatherForecastController(ILogger<WeatherForecaseController> logger)
    {
        _logger = logger;
    }

    // khi truy cập "https://localhost:7072/WeatherForecast" trên browser 
    // nó sẽ trả cho ta some random weather data
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable
            .Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    // truy cập qua: https://localhost:7072/WeatherForecase/parse-as-json
    [HttpGet]
    [Route("parse-as-json")]
    public IActionResult ParseAsJson()
    {
        _logger.LogInformation("calling route 'parse-as-json'");

        var randomBoolean = new Random().Next(1, 10) / 2 == 1 ? true : false;
        var randomString = randomBoolean ? "C#" : "Javascript";

        var message = new Message
        {
            Id = Guid.NewGuid();
            Type = randomBoolean ? "HTTP" : "HTTPS",
            Data = new SupportedType[] {
                new SupportedType { VerbType = "POST", Message = "Test message: " + randomString },
                new SupportedType { VerbType = "GET", Message = "Test message: " + randomString },
                new SupportedType { VerbType = "DELETE", Message = "Test message: " + randomString }
            }
        };

        _logger.LogInformation("{@message}", message);
        // ta sẽ xem phần log khi access endpoint này trong Console
        // -> ta sẽ thấy nó log ra: MyProject.Controllers.Message
        // -> nó sẽ log ra actual type of "Message" object thay vì actual data nó chứa
        // => thông tin này không thực sự useful or meaningful, ta có thể sẽ cần JSON convert
        // => ta có thể dùng 'Serilog' để có log tốt hơn  

        return Ok(message);
    }
}

public class Message
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public SupportedType[] Data { get; set; }
}

public class SupportedType
{
    public string VerbType { get; set; }
    public string Message { get; set; }
}
```

## Serilog
* -> ta sẽ dùng cli **`dotnet add package <package_name> --version=<version>`** để cài dotnet package
* -> **Serilog**, **Serilog.AspNetCore**, **Serilog.Formatting.Compact**, **Serilog.Settings.Configuration**, **Serilog.Sinks.SumoLogic**, **SumoLogic.Logging.Serilog**

### Log into 'Console'
```cs - Program.cs
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console() // write into Console
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();


// -> giờ ta thử truy cập: https://localhost:7072/WeatherForecase/parse-as-json và xem log trong Console
// -> ta sẽ thấy nhiều thông tin hơn bao gồm: cả data mà "Message" object đang giữ, một số thông tin về request đến endpoint này
```

### Log into a seperate file

```json - appsettings.json
{
    "Serilog": {
        "Using": ["Serilog.Sinks.File"], // the adapter we're using
        "MinimumLevel": {
            "Default": "Information"
        },
        "WriteTo": [
            {
                "Name": "File", // write log to a file
                "Args": {
                    // inject "Day" into log file name
                    "path": "./logs/webapi-.log", 
                    "rollingInterval": "Day",

                    // message format   
                    "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3} {Username} {Message:lj}{Exception}{NewLine}"
                }
            }
        ]
    }
}
```

```cs - program.cs
var logger = new LoggerConfiguration()
    .WriteTo.Console() 
    .ReadFrom.Configuration(builder.Configuration) // read from configuration
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.AddSerilog(logger);


// -> giờ ta thử truy cập: https://localhost:7072/WeatherForecase/parse-as-json
// -> nó sẽ tạo ra thư mục "logs" ở project root và sẽ chứa file log kiểu như "webapi-20240101.log"
```

### Push log to 'SumoLogic'

```cs
using Serilog;
using Serilog.Formatting.Compact;
using SumoLogic.Logging.Serilog.Extensions;

var logger = new LoggerConfiguration()
    .WriteTo.Console() 
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.BufferedSumoLogic(
        new Uri(
            "url_to_httpsourcecollector_ofSumoLogic",
            sourceName: "TEST_COLLECTOR", // appear on 'SumoLogic' so that we can use some query
            formatter: new CompactJsonFormatter()
        )
    )
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.AddSerilog(logger);
```

* -> giờ ta sẽ vào **Search** section của trang **SumoLogic** và nhập:
```bash
_sourceCategory="demo"
```
```bash
_sourceCategory="demo"
    | json field=_raw "message"
    | json field=_raw "message.Data[0].VerbType"
```