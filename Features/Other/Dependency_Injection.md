===============================================================================
## DI without interface
```cs
builder.Services.AddScoped<UserService>();
```

## DI with Generic 

```cs
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```


## DI with Factory

===============================================================================
# Autofac

## 'Autofac' vs 'Microsoft.Extensions.DependencyInjection'
* -> https://stackoverflow.com/questions/63407601/how-is-autofac-better-than-microsoft-extensions-dependencyinjection

* -> Ex: register IHttpClientFactory with **Microsoft.Extensions.DependencyInjection**:https://stackoverflow.com/questions/73608744/how-to-use-ihttpclientfactory-with-autofac-using-net-framework

## Install
* -> **`Autofac`**
* -> **`Autofac.WebApi2`**
* -> **`Autofac.Mvc5`**

* -> trong App_Start -> chọn "Add" -> chọn **`OWIN Startup class`** để tạo class với tên là **Startup.cs**
```cs
[assembly: OwinStartup(typeof(TeduShop.Web.App_Start.Startup))]
namespace TeduShop.Web.App_Start.Startup
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
        private void ConfigAutofac(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
        }
    }
}
```