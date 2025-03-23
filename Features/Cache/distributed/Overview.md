==================================================================================
# Distributed caching
* -> is a **cache** **`shared by multiple app servers`** 
* -> typically maintained as **`an external service`** - the cached data is **`stored on individual app servers`** (_vậy nên rất thích hợp với app is hosted by a `cloud service` hoặc a `server farm`_)

==================================================================================
# All concepts
* https://roadmap.sh/redis

==================================================================================
# Implementation

# Prerequisites
* _add a package reference for the distributed cache provider used:_
* -> for a **Redis** distributed cache, **`Microsoft.Extensions.Caching.StackExchangeRedis`**
* -> for **SQL Server**, **`Microsoft.Extensions.Caching.SqlServer`**
* -> for the **NCache** distributed cache, **`NCache.Microsoft.Extensions.Caching.OpenSource`**

# 'IDistributedCache' interface 
* -> to **establish distributed caching services**, we'll register an implementation of **`IDistributedCache`** in **`Program.cs`**
* -> the implementation will include: **Distributed Redis cache**, **Distributed Memory Cache**, **Distributed SQL Server cache**, **Distributed NCache cache**, **Distributed Azure CosmosDB cache**
* _nhưng nói chung `Redis Cache` vẫn là lựa chọn được recommend thì nó có **`most performant`**_

* _the interface provides the `methods` to manipulate items in the distributed cache implementation_
* -> **`Get`**, **`GetAsync`**: accepts **a string key** and retrieves **a cached item as a byte[] array** if found in the cache
* -> **`Set`**, **`SetAsync`**: adds **an item (as byte[] array)** to the cache using **a string key**
* -> **`Refresh`**, **`RefreshAsync`**: refreshes an item in the cache based on its key, **resetting its sliding expiration timeout (if any)**
* -> **`Remove`**, **`RemoveAsync`**: **removes a cache item** based on its string key