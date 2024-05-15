===============================================================

# Handle null value in "Deserialize"
```cs
var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

var jsonModel = JsonConvert.DeserializeObject<Customer>(jsonString, settings);
```

```cs
public class Response
{
    public string Status;

    public string ErrorCode;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ErrorMessage;
}


var response = JsonConvert.DeserializeObject<Response>(data);
```
