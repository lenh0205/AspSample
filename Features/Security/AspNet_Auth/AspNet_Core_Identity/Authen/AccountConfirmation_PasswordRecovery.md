==================================================================================
# Create and test a web app with authentication

```bash - create a web app with authentication
dotnet new webapp -au Individual -o WebPWrecover
cd WebPWrecover
dotnet run
```

## Configure an email provider
* -> we will use **`SendGrid`** (_library_) to send email
* -> **a SendGrid account and key is needed to send email**
* _Microsot recommend using `SendGrid` or another email service to send email rather than `SMTP`; `SMTP` is **difficult to secure and set up correctly**_
* _the SendGrid account may require adding a **Sender**

* -> we need to create **`a class to fetch the secure email key`**

```cs - create Services/AuthMessageSenderOptions.cs:
namespace WebPWrecover.Services;

public class AuthMessageSenderOptions
{
    public string? SendGridKey { get; set; }
}
```

### Configure 'SendGrid' user secrets
* -> **`set the SendGridKey with the secret-manager tool`**

```bash - For example:
dotnet user-secrets set SendGridKey <key>
# Successfully saved SendGridKey to the secret store
```

* -> on Windows, **Secret Manager** stores keys/value pairs in a **`secrets.json`** file in the **`%APPDATA%/Microsoft/UserSecrets/<WebAppName-userSecretsId>`** directory  
* -> the contents of the secrets.json file aren't encrypted

```json
{
  "SendGridKey": "<key removed>"
}
```

## Install SendGrid
```bash
Install-Package SendGrid
```

## Implement 'IEmailSender'

```cs - Services/EmailSender.cs 
public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(Options.SendGridKey, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("Joe@contoso.com", "Password Recovery"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode 
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }
}
```

## Configure app to support email
* -> add **EmailSender** as **`a transient service`**
* -> register the **AuthMessageSenderOptions** configuration instance

```cs
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>(); // this one
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration); // this one

var app = builder.Build();

// ....
```

## Disable default account verification when Account.RegisterConfirmation has been scaffolded

```cs - /Areas/Identity/Pages/Account/RegisterConfirmation.cshtml.cs
[AllowAnonymous]
public class RegisterConfirmationModel : PageModel
{
    //...

    public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
    {
        // ... 
        DisplayConfirmAccountLink = false;
        if (DisplayConfirmAccountLink)
        //....
    }
}
```

==================================================================================
# Testing - Register, confirm email, and reset password
* _run the web app, and test the account confirmation and password recovery flow_
* -> run the app and register a new user
* -> check your email for the account confirmation link
* -> Click the link to confirm your email.
* -> Sign in with your email and password.
* -> Sign out.

* _Test password reset_
* -> If you're signed in, select Logout.
* -> Select the Log in link and select the Forgot your password? link.
* -> Enter the email you used to register the account.
* -> An email with a link to reset your password is sent. Check your email and click the link to reset your password. After your password has been successfully reset, you can sign in with your email and new password

==================================================================================
# Resend email confirmation
* -> select the "Resend email confirmation link" on the "Login" page

## Change email and activity timeout
* -> **`the default inactivity timeout is 14 days`**

```cs - For example: sets the inactivity timeout to "5 days"
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.ConfigureApplicationCookie(o => { // this one
    o.ExpireTimeSpan = TimeSpan.FromDays(5);
    o.SlidingExpiration = true;
});

var app = builder.Build();

// ....
```

## Change all data protection token lifespans
* -> he built in Identity user tokens have **`a one day timeout`**

```cs - For example: changes all data protection tokens timeout period to "3 hours"
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.Configure<DataProtectionTokenProviderOptions>(o => // this one
       o.TokenLifespan = TimeSpan.FromHours(3));

var app = builder.Build();

// ....
```

## Change the email token lifespan
* -> the **default token lifespan of the Identity user tokens** is **`one day`**

```cs - add a custom "DataProtectorTokenProvider<TUser>" and "DataProtectionTokenProviderOptions":
public class CustomEmailConfirmationTokenProvider<TUser> :  DataProtectorTokenProvider<TUser> where TUser : class
{
    public CustomEmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger
    ) : base(dataProtectionProvider, options, logger)
    {
    }
}
public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public EmailConfirmationTokenProviderOptions()
    {
        Name = "EmailDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}
```

```cs - add the "custom provider" to the service container:
builder.Services.AddDefaultIdentity<IdentityUser>(config =>
{
    config.SignIn.RequireConfirmedEmail = true;
    config.Tokens.ProviderMap.Add("CustomEmailConfirmation", // this one
        new TokenProviderDescriptor(
            typeof(CustomEmailConfirmationTokenProvider<IdentityUser>)));
    config.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<CustomEmailConfirmationTokenProvider<IdentityUser>>(); // this one

builder.Services.AddRazorPages();
```

## Debug email

## Combine social and local login accounts