===================================================================
# HttpClient
* -> although HttpClient implement **IDisposable** interface, but when **we dispose HttpClient object `the socket connection is not disposed of yet`**
* -> so in the scenario where we **creating a new HttpClient object (in "using" statement) on each request (`especially to the same API`)**, this lead to the **`Socket exhaustion exception`**

* _HttpClient sẽ giữ socket open một thời gian sau khi request completed_
* _**Socket exhaustion** - tức là a new HttpClient cannot acquire a socket to make a request, vậy nên những request này sẽ gây lỗi_

## Solution
* -> so the recommended approach is **not to dispose of it after each external api call**; it **should be live `until our application is live` and should be `reused` for other api calls`**
* -> The recommended solution is **`IHttpClientFactory`**

## use a global 'HttpClient' object
* -> create HttpClient object as singleton or static, we'll have **a single instance of HttpClient for a given endpoint**
* -> this maybe a solution but still not preffered, because we will not get the liberty of IHttpClienFatory like **`managing the lifetime of httpClient`** and **`pooling objects for reuse`**

* -> with this approach, it’s better to configure the **connection pooling behavior** by configuring the **`PooledConnectionLifetime`** 
* -> this allows us to define how long a connection remains active when pooled, and once this lifetime expires, the connection will no longer be pooled or issued for future requests
* => it gets or sets **`how long a connection can be in the pool to be considered reusable`**
* -> otherwise if the DNS TTL (time to live — is a setting that tells the DNS resolver how long to cache a query before requesting a new one) is expired and the domain name points to a new IP
* -> our code will never know until it restarts, since there is no default logic in HttpClient to handle that

```cs
public class HttpClientExample : IHttpClientExample
{
    private readonly IConfiguration _configuration;
    private readonly string _baseUri;

    private static HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
    {
        // configured the PooledConnectionLifeTime for 1 minute
        PooledConnectionLifetime = TimeSpan.FromMinutes(1)
    });

    public HttpClientExample(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // get the API Url from the appsettings.json file
        _baseUri = _configuration.GetSection("CoinDeskApi:Url").Value;
    }

    public async Task<BtcContent?> GetBtcContent()
    {
        try
        {
            // make request
            var response = await _httpClient.GetAsync($"{_baseUri}currentprice.json");

            var btcCurrentPrice = JsonSerializer.Deserialize<BtcContent>(await response.Content.ReadAsStringAsync());
            return btcCurrentPrice;
        }
        catch (Exception ex)
        {
            return await Task.FromException<BtcContent>(ex);
        }
    }
}
```

===================================================================
> the best approach for making requests to external API is by using IHttpClientFactory

# IHttpClientFactory (Microsoft.Extensions.Http)
* -> a **factory abstraction** that can create HttpClient instances with custom configurations that take advantage of resilient and transient-fault-handling third-party middleware with ease

* -> it has a method **`CreateClient`** which **returns the HttpClient Object**
* -> HttpClient is actually just a wrapper for **HttpMessageHandler**, **`HttpClientFactory manages the lifetime of HttpMessageHander`** (_which is a HttpClientHandler who does the real work under the hood_)

## Mechanism
* -> IHttpClientFactory creates a **`pool of HttpMessageHanlder objects`** and **`disposing of HttpClientHandlers after a specified period`**
* -> whenever **any client requests a HttpClient Object**, it **first looks into the HttpMessageHandler object pool**
* -> if it finds any object available there, then it **`returns it instead of creating a new one`**; if it does **`not find then it will return a new object`**
* -> when a new HttpClientHandler is created for an endpoint, a DNS lookup is performed; we won’t wear out the sockets, and we will **`get a new IP address for each endpoint`**

## Other advantages
* -> resilient HttpClient using Polly
* -> naming and configuring logical HttpClient instances
* -> build an outgoing request middleware to manage cross-cutting concerns around HTTP requests
* -> integrates with Polly for transient fault handling
* -> avoid common DNS problems by managing HttpClient lifetimes
* -> adds logging for all requests sent through clients created by the factory

## Best Pratice
* -> use **Named Clients** - define named HttpClient instances for specific API endpoints or services to encapsulate configuration settings and promote code readability
* -> **Configure Policies** - implement resilient HTTP request policies such as retry, circuit breaker, and timeout policies to handle transient faults and improve application robustness.
* -> **Dispose Responsibly** - while IHttpClientFactory manages the lifecycle of HttpClient instances, it's essential to dispose of resources explicitly when necessary, especially in long-lived applications

```cs 
// program.cs
services.AddHttpClient("order", config => { // Named Clients as "order"
    config.BaseAddress = new System.Uri("https://localhost:5001/");
})

// Usage
public class OrderDetailProvider
{
    private readonly IHttpClientFactory httpClientFactory;

    public OrderDetailProvider(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<OrderDetail[]> Get()
    {
        // for basic handle, in case of exception that the service we called is fail, we don't fail we may return an empty result so that the user can retry later and application is not broken
        try {
            using var client = httpClientFactory.CreateClient("order");
            var response = await client.GetAsync("/api/order");
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrderDetail[]>(data);
        }
        catch (Exception ex)
        {
            // Log the exception
            return Array.Empty<OrderDetail>();
        }
    }
}
```

===================================================================
# Example: using HttpClient

```cs - Console App
// -> Console App as Cient make multiple requests to same WebAPI (https://localhost:44350)

class Program
{
    static async Task Main(string[] args)
    {
        for (int i = 0; i < 10; i++)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://localhost:44350/weatherforecast");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
        }
        Console.ReadLine();
    }
}
```
```bash
# check if there are socket connections still open in 'Time_Wait' status
netstat -na | find "44350"

# Output:
# TCP       0.0.0.0:44350           0.0.0.0:0               LISTENING
# TCP       127.0.0.1:65443         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65444         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65445         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65446         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65447         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65448         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65449         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65450         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65451         127.0.0.1:44350         TIME_WAIT
# TCP       127.0.0.1:65452         127.0.0.1:44350         TIME_WAIT
# TCP       [::]:44350              [::]:0                  LISTENING
# TCP       [::1]:44350             [::1]:65454             ESTABLISHED
# TCP       [::1]:65454             [::1]:44350             ESTABLISHED
```

# Example: using HttpClientFactory

```cs - Console App
static async Task Main(string[] args)
{
    // calling 'AddHttpClient()' to register 'IHttpClientFactory' as a singleton
    var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
    var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

    for (int i = 0; i < 10; i++)
    {
        // get the HttpClient object from Dependency Injection
        var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync("https://localhost:44350/weatherforecast");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
        Console.WriteLine(Environment.NewLine);
    }
    Console.ReadLine();
}
```
```bash
netstat -na | find "44350"

# Output:
# TCP       0.0.0.0:44350           0.0.0.0:0               LISTENING
# TCP       127.0.0.1:44350         127.0.0.1:60687         ESTABLISHED
# TCP       127.0.0.1:65443         127.0.0.1:44350         ESTABLISHED
```

===================================================================
# Socket
* -> a socket represents one endpoint for communication channel between two systems over a network
* -> generally applies to communication using TCP, UDP or other similar endpoint-based communication models
* -> The socket binds to an IP address and port and establishes a connection with the server's socket on its IP address and port

## Lifecycle:
Creation: A socket is created (e.g., when an HttpClient initiates an HTTP request).
Connection: The socket connects to a server.
Communication: Data is exchanged over the socket.
Closure: The socket is closed when the communication is complete.