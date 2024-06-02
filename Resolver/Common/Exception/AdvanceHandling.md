
# The Circuit Breaker Pattern
* -> _the `Circuit Breaker pattern`_ is **pivotal in preventing a system from repeatedly trying to execute an operation that’s likely to fail**
* -> it’s especially useful when dealing with **`external resources or services`** that might be **temporarily unavailable**

* -> **Common use case:  Preventing System Overload** - when an application that **`depends on a third-party service`**
* -> if this **`service goes down`**, **`continually attempting to connect`** could **overload both the client and the service** when it comes back online

```cs
public class CircuitBreaker
{
    private enum State { Open, HalfOpen, Closed }
    private State state = State.Closed;
    private int failureThreshold = 5;
    private int failureCount = 0;
    private DateTime lastAttempt = DateTime.MinValue;
    private TimeSpan timeout = TimeSpan.FromMinutes(1);

    public bool CanAttemptOperation()
    {
        switch (state)
        {
            case State.Closed:
                return true;
            case State.Open:
                if ((DateTime.UtcNow - lastAttempt) > timeout)
                {
                    state = State.HalfOpen;
                    return true;
                }
                return false;
            case State.HalfOpen:
                return false;
            default:
                throw new InvalidOperationException("Invalid state");
        }
    }

    public void RecordFailure()
    {
        lastAttempt = DateTime.UtcNow;
        failureCount++;
        if (failureCount >= failureThreshold)
        {
            state = State.Open;
        }
    }

    public void Reset()
    {
        state = State.Closed;
        failureCount = 0;
    }
}
```

# Error Monitoring and Alerting Systems
* -> implementing _`a comprehensive error monitoring and alerting system`_ **allows teams to react promptly** to **`unforeseen issues, minimizing downtime and improving user satisfaction`**
* -> **Microsoft’s Application Insights** provides **`extensive monitoring capabilities`**, including automatic exception tracking, custom error logging, and alerting based on predefined criteria

```cs
// Configure Application Insights in Startup.cs or Program.cs
services.AddApplicationInsightsTelemetry();

// Use TelemetryClient to log custom exceptions
public void LogError(Exception exception)
{
    var telemetryClient = new TelemetryClient();
    telemetryClient.TrackException(exception);
}
```

# Proactive Error Recovery
* -> _Proactive error recovery_ involves **anticipating potential failure points** and **implementing strategies to recover from these errors automatically**
* -> **common use case: Automatic Retry Mechanisms** - for operations that may **`intermittently fail due to transient conditions`**, implementing an automatic retry mechanism can improve reliability

```cs
public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxAttempts = 3)
{
    int attempt = 0;
    while (true)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            LogWarning($"Operation failed, attempt {attempt}. Retrying...");
            attempt++;
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
```

# Leveraging 'Polly' for Advanced Resilience Policies
* -> _Polly_ is **`a .NET library`** that provides **resilience and transient-fault-handling capabilities**
* -> it offers **`policies`** like **Retry**, **Circuit Breaker**, **Timeout**, **Bulkhead Isolation**, and more

```cs - Polly Integration Example: Retry and Circuit Breaker
// Define a policy that retries three times with a two-second delay between retries
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

// Define a circuit breaker policy that breaks the circuit after 5 consecutive failures for one minute
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));

var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

await policyWrap.ExecuteAsync(async () => 
{
    // Code to execute that can potentially throw HttpRequestException
});
```

# Observability and Exception Handling - OpenTelemetry
* -> **enhancing observability** through **`detailed logging, metrics, and distributed tracing`**
* -> allows for more effective monitoring and debugging of errors, **especially in complex or distributed systems (microservices)** 

```cs
// Configure OpenTelemetry in your application
services.AddOpenTelemetryTracing(builder =>
{
    builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter();
});

// Use OpenTelemetry API for custom logging and tracing
var span = tracer.StartSpan("MyOperation");
try
{
    // Perform operation
}
catch (Exception ex)
{
    span.RecordException(ex);
    throw;
}
finally
{
    span.End();
}
```

# Exception Handling in Asynchronous Event-Driven Architectures
* -> in **`event-driven architectures`**, especially those **employing asynchronous messaging or event sourcing**
* -> managing exceptions requires **`careful consideration`** of **message retry policies**, **idempotency**, and **dead-letter handling** to ensure system resilience and consistency
* => **Strategy: Dead-Letter Queues for Unprocessable Messages**
```cs
// Configure a dead-letter queue for handling failed message processing
var options = new ServiceBusProcessorOptions
{
    MaxConcurrentCalls = 1,
    AutoCompleteMessages = false,
    DeadLetterErrorDescription = "Failed to process message."
};

serviceBusProcessor.ProcessMessageAsync += async args =>
{
    try
    {
        // Process message
        await args.CompleteMessageAsync(args.Message);
    }
    catch (Exception ex)
    {
        await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
    }
};
```