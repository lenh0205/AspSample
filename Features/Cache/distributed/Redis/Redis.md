
# Redis
* -> is an **`open source in-memory data store`** (_an in-memory database_), which is often used as **a distributed cache**

## Setting
* -> first, create **`an Azure Cache for Redis`** (_dù là ta dùng **Azure-hosted ASP.NET Core app** hoặc **local development**_)
* -> then, copy the **`Primary connection string (StackExchange.Redis)`** to **Configuration** (_bỏ vào **Secret Manager** nếu là `Local developement`, **App Service Configuration** nếu là `Azure-hosted app`_)

* _hoặc ta cũng có alternative approaches to a `local Redis cache`: https://github.com/dotnet/AspNetCore.Docs/issues/19542_

## Azure Cache for Redis
* -> https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/cache-overview