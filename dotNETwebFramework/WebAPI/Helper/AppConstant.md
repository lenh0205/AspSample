
```cs
public static class AppConstants
{
    // compile-time constant
    public const string AppName = "MyMVCApp";
    public const int MaxUsers = 100;

    // runtime constant
    public static readonly string ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
}
```