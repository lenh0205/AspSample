
## Access the "appSettings" section of the "Web.config" file in an ASP.NET app .NET framework
* _Cách 1: more general and not tied to the "System.Web" namespace_
* -> make code more portable if decide to move parts of it outside of an ASP.NET context
* -> "System.Configuration.ConfigurationManager"
```cs
System.Configuration.ConfigurationManager.AppSettings["Environment"];
```

* _Cách 2: suitable if working with Web-specific features like URL authorization and membership providers_
* -> "System.Web.Configuration.WebConfigurationManager"
```cs
System.Web.Configuration.WebConfigurationManager.AppSettings["Environment"];
```

## Priority in between 'appsettings.json' file and 'appsettings.Development.json' file
* -> when the application starts, it **loads configuration from appsettings.json first** and then **`overrides any matching keys`** with values from **appsettings.{Environment}.json**

```json - appsettings.json
{
    "IdentityServer": {
    "Clients": {
        "IdentitySPA": {
        "Profile": "IdentityServerSPA"
        }
    }
    }
}
```

```json - appsettings.Development.json
{
    "IdentityServer": {
        "Key": {
            "Type": "Development"
        }
    }
}
```
* -> trong trường hợp này, trong **"IdentityServer" sections** thì **"Client" section** vẫn được giữ nguyên (_vì không có key nào tương tự trong appsettings.Development.json_), đồng thời có thêm **"Key" section**