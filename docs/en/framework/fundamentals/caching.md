# Distributed Caching

ABP extends the [ASP.NET Core distributed cache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed) to provide a more comfortable and easy-to-use cache service.

> **Default implementation of the `IDistributedCache` interface is` MemoryDistributedCache` which works in-memory.** Memory cache is only useful if you are building a monolith application and you run a single instance of your application. For other cases, consider using a distributed cache server. See the ***[When to Use a Distributed Cache Server](../../kb/when-to-use-a-distributed-cache-server.md)*** document for more details.

## Installation

> This package is already installed by default in [startup templates](../../solution-templates/index.md). So, most of the time, you don't need to install it manually.

[Volo.Abp.Caching](https://www.nuget.org/packages/Volo.Abp.Caching) is the main package of the caching system. You can install it as a project using the add-package command of the [ABP CLI](../../cli):

```bash
abp add-package Volo.Abp.Caching
```

You need to run this command on a command line terminal in a folder containing a `csproj` file (see [other options](https://abp.io/package-detail/Volo.Abp.Caching) to install).

## Usage

### `IDistributedCache` Interface

ASP.NET Core defines the `IDistributedCache` interface to get/set the cache values. But it has some difficulties:

* It works with **byte arrays** rather than .NET objects. So, you need to **serialize/deserialize** the objects you need to cache.
* It provides a **single key pool** for all cache items, so;
  * You need to care about the keys to distinguish **different types of objects**.
  * You need to care about the cache items of **different tenants** in a [multi-tenant](../architecture/multi-tenancy) system.

> `IDistributedCache` is defined in the `Microsoft.Extensions.Caching.Abstractions` package. That means it is not only usable for ASP.NET Core applications but also available to **any type of applications**.

See [ASP.NET Core's distributed caching document](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed) for more information.

### `IDistributedCache<TCacheItem>` Interface

ABP defines the generic `IDistributedCache<TCacheItem>` interface in the [Volo.Abp.Caching](https://www.nuget.org/packages/Volo.Abp.Caching/) package. `TCacheItem` is the type of the object stored in the cache. 

`IDistributedCache<TCacheItem>` solves the difficulties explained above;

* It internally **serializes/deserializes** the cached objects. It uses **JSON** serialization by default but can be overridden by replacing the `IDistributedCacheSerializer` service in the [dependency injection](./dependency-injection.md) system.
* It automatically adds a **cache name** prefix to the cache keys based on the object type stored in the cache. The default cache name is the full name of the cache item class (`CacheItem` postfix is removed if your cache item class ends with it). You can use the **`CacheName` attribute** on the cache item class to set the cache name.
* It automatically adds the **current tenant id** to the cache key to distinguish cache items for different tenants (if your application is [multi-tenant](../architecture/multi-tenancy)). Define `IgnoreMultiTenancy` attribute on the cache item class to disable this if you want to share the cached objects among all tenants in a multi-tenant application.
* Allows defining a **global cache key prefix** per application so different applications can use their isolated key pools in a shared distributed cache server.
* It **can tolerate errors** wherever possible and bypasses the cache. This is useful when you have temporary problems on the cache server.
* It has methods like `GetManyAsync` and `SetManyAsync` which significantly improve the performance of **batch operations**.

**Example: Store Book names and prices in the cache**

````csharp
namespace MyProject
{
    public class BookCacheItem
    {
        public string Name { get; set; }

        public float Price { get; set; }
    }
}
````

You can inject and use the `IDistributedCache<BookCacheItem>` service to get/set `BookCacheItem` objects:

````csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace MyProject
{
    public class BookService : ITransientDependency
    {
        private readonly IDistributedCache<BookCacheItem> _cache;

        public BookService(IDistributedCache<BookCacheItem> cache)
        {
            _cache = cache;
        }

        public async Task<BookCacheItem> GetAsync(Guid bookId)
        {
            return await _cache.GetOrAddAsync(
                bookId.ToString(), //Cache key
                async () => await GetBookFromDatabaseAsync(bookId),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
        }

        private Task<BookCacheItem> GetBookFromDatabaseAsync(Guid bookId)
        {
            //TODO: get from database
        }
    }
}
````

* This sample service uses the `GetOrAddAsync()` method to get a book item from the cache. `GetOrAddAsync` is an additional method that was added by the ABP to the standard ASP.NET Core distributed cache methods.
* If the book was not found in the cache, it calls the factory method (`GetBookFromDatabaseAsync` in this case) to retrieve the book item from the original source.
* `GetOrAddAsync` optionally gets a `DistributedCacheEntryOptions` which can be used to set the lifetime of the cached item.

`IDistributedCache<BookCacheItem>` supports the same methods of the ASP.NET Core's standard `IDistributedCache` interface, so you can refer [it's documentation](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed).

### `IDistributedCache<TCacheItem, TCacheKey>` Interface

`IDistributedCache<TCacheItem>` interface assumes that the type of your **cache key** is `string` (so, you need to manually convert your key to string if you need to use a different kind of cache key). While this is not a big deal, `IDistributedCache<TCacheItem, TCacheKey>` can be used when your cache key type is not `string`.

**Example: Store Book names and prices in the cache**

````csharp
using Volo.Abp.Caching;

namespace MyProject
{
    [CacheName("Books")]
    public class BookCacheItem
    {
        public string Name { get; set; }

        public float Price { get; set; }
    }
}
````

* This example uses the `CacheName` attribute for the `BookCacheItem` class to set the cache name.

You can inject and use the `IDistributedCache<BookCacheItem, Guid>` service to get/set `BookCacheItem` objects:

````csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace MyProject
{
    public class BookService : ITransientDependency
    {
        private readonly IDistributedCache<BookCacheItem, Guid> _cache;

        public BookService(IDistributedCache<BookCacheItem, Guid> cache)
        {
            _cache = cache;
        }

        public async Task<BookCacheItem> GetAsync(Guid bookId)
        {
            return await _cache.GetOrAddAsync(
                bookId, //Guid type used as the cache key
                async () => await GetBookFromDatabaseAsync(bookId),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );
        }
        private Task<BookCacheItem> GetBookFromDatabaseAsync(Guid bookId)
        {
            //TODO: get from database
        }
    }
}
````

* This sample service uses the `GetOrAddAsync()` method to get a book item from the cache.
* Since the cache is explicitly implemented as using  `Guid` as the cache key, the `Guid` value is passed to the `_cache_GetOrAddAsync()` method.

#### Complex Types as the Cache Key

`IDistributedCache<TCacheItem, TCacheKey>`  internally uses `ToString()` method of the key object to convert it to a string. If you need to use a complex object as the cache key, you need to override `ToString` method of your class.

An example class that is used as a cache key:

````csharp
public class UserInOrganizationCacheKey
{
    public Guid UserId { get; set; }
    
    public Guid OrganizationId { get; set; }
    
    //Builds the cache key
    public override string ToString()
    {
        return $"{UserId}_{OrganizationId}";
    }
}
````

Example usage:

````csharp
public class BookService : ITransientDependency
{
    private readonly IDistributedCache<UserCacheItem, UserInOrganizationCacheKey> _cache;

    public BookService(
        IDistributedCache<UserCacheItem, UserInOrganizationCacheKey> cache)
    {
        _cache = cache;
    }
    
    ...
}
````

## Configuration

### AbpDistributedCacheOptions

`AbpDistributedCacheOptions` is the main [options class](./options.md) to configure the caching.

**Example: Set the cache key prefix for the application**

```csharp
Configure<AbpDistributedCacheOptions>(options =>
{
    options.KeyPrefix = "MyApp1";
});
```

> Write that code inside the `ConfigureServices` method of your [module class](../architecture/modularity/basics.md).

#### Available Options

* `HideErrors` (`bool`, default: `true`): Enables/disables hiding the errors on writing/reading values from the cache server.
* `KeyPrefix` (`string`, default: `null`): If your cache server is shared by multiple applications, you can set a prefix for the cache keys for your application. In this case, different applications can not overwrite each other's cache items.
* `GlobalCacheEntryOptions` (`DistributedCacheEntryOptions`): Used to set default distributed cache options (like `AbsoluteExpiration` and `SlidingExpiration`) used when you don't specify the options while saving cache items. The default value uses the `SlidingExpiration` as 20 minutes.

## Error Handling

When you design a cache for your objects, you typically try to get the value from the cache first. If not found in the cache, you query the object from the **original source**. It may be located in a **database** or may require an HTTP call to a remote server to be performed.

In most cases, you want to **tolerate the cache errors**; If you get an error from the cache server, you don't want to cancel the operation. Instead, you silently hide (and log) the error and **query from the original source**. This is what the ABP does by default.

ABP's Distributed Cache [handle](./exception-handling.md), log and hide errors by default. There is an option to change this globally (see the options below).

In addition, all of the `IDistributedCache<TCacheItem>` (and `IDistributedCache<TCacheItem, TCacheKey>`) methods have an optional `hideErrors` parameter, which is `null` by default. The global value is used if this parameter is left as `null`; otherwise, you can decide to hide or throw the exceptions for individual method calls.

## Batch Operations

ABP's distributed cache interfaces provide methods to perform batch operations that improve performance when you want to batch operation multiple cache items in a single method call.

* `SetManyAsync` and `SetMany` methods can be used to set multiple values to the cache.
* `GetManyAsync` and `GetMany` methods can be used to retrieve multiple values from the cache.
* `GetOrAddManyAsync` and `GetOrAddMany` methods can be used to retrieve multiple values and set missing values from the cache
* `RefreshManyAsync` and `RefreshMany` methods can be used to reset the sliding expiration timeout of multiple values from the cache
* `RemoveManyAsync` and `RemoveMany` methods can be used to remove multiple values from the cache

> These are not standard methods of the ASP.NET Core caching. So, some providers may not support them. They are supported by the [ABP Redis Cache integration package](./redis-cache.md). If the provider doesn't support it, it falls back to `SetAsync` and `GetAsync` ... methods (called once for each item).

## Caching Entities

ABP provides a [Distributed Entity Cache System](../infrastructure/entity-cache.md) for caching entities. It is useful if you want to use caching for quicker access to the entity rather than repeatedly querying it from the database.

It's designed as read-only and automatically invalidates a cached entity if the entity is updated or deleted.

> See the [Entity Cache](../infrastructure/entity-cache.md) documentation for more information.

## Advanced Topics

### Unit Of Work Level Cache

Distributed cache service provides an interesting feature. Assume that you've updated the price of a book in the database, then set the new price to the cache, so you can use the cached value later. What if you have an exception after setting the cache and you **rollback the transaction** that updates the price of the book? In this case, cache value will be incorrect.

`IDistributedCache<..>` methods gets an optional parameter, named `considerUow`, which is `false` by default. If you set it to `true`, then the changes you made for the cache are not actually applied to the real cache store, but associated with the current [unit of work](../architecture/domain-driven-design/unit-of-work.md). You get the value you set in the same unit of work, but the changes are applied **only if the current unit of work succeeds**.

### IDistributedCacheSerializer

`IDistributedCacheSerializer` service is used to serialize and deserialize the cache items. The default implementation is the `Utf8JsonDistributedCacheSerializer` class that uses `IJsonSerializer` service to convert objects to [JSON](../../json-serialization.md) and vice verse. Then it uses UTC8 encoding to convert the JSON string to a byte array which is accepted by the distributed cache.

You can [replace](./dependency-injection.md) this service with your own implementation if you want to implement your own serialization logic.

### IDistributedCacheKeyNormalizer

`IDistributedCacheKeyNormalizer` is implemented by the `DistributedCacheKeyNormalizer` class by default. It adds the cache name, application cache prefix and current tenant ID to the cache key. If you need a more advanced key normalization, you can [replace](./dependency-injection.md) this service with your own implementation.

## See Also

* [Entity Cache](../infrastructure/entity-cache.md)
* [Redis Cache](./redis-cache.md)
