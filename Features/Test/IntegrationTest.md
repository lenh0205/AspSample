
# Example: "external APIs" inside "integration test"
* -> this would be a problem if these external API is unavailable or have some lincenses that require us to pay for calls it
* -> especially when we're running a pretty large test or running test in parallel
* -> mocking an HTTP Handle might seem like a good idea, but we can't really simulate pragmatically all the real world scenarios for production like network, delays, timeouts, service unavailable, ....

## WrireMock
* => we **don't necessarily need to call those external API**, we can **call a mocked out instance of the API** that will behave in a similar fashion to the one that we need
* => this is where **`WireMock`** comes in - basically by using this library we can setup **InMemory API** that will behave like the real instance
* -> it can response to URL, paths, headers, cookie, ...

## Step
https://www.youtube.com/watch?v=2FWt_v0YB3U&list=WL&index=6

* -> the firt thing we'll need is an **HttpClient**

```cs
public class PaymentServiceClient
{
    private readonly HttpClient _httpClient;

    public PaymentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentStatusResponse> RetrievePaymentStatusAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/v1/payments/{id}/status");

        if (response.IsSuccessStatusCode)
        {
            var paymentStatus = await response.Content.ReadFromJsonAsync<PaymentStatusReponse>();
            return paymentStatus;
        }

        return default;
    }
}

public class PaymentStatusResponse
{
    public string Status { get; set; }
}

public class PaymentServiceEndpointDefinition : IEndpointsDefinition
{
    public static void Configuration
}
```