# Layered Solution: Overview

````json
//[doc-nav]
{
  "Previous": {
    "Name": "Index",
    "Path": "solution-templates/layered-web-application/index"
  },
  "Next": {
    "Name": "Solution Structure",
    "Path": "solution-templates/layered-web-application/solution-structure"
  }
}
````

> Some of the features mentioned in this document may not be available in the free version. We're using the **\*** symbol to indicate that a feature is available in the **[Team](https://abp.io/pricing)** and **[Higher](https://abp.io/pricing)** licenses.


In this document, you will learn what the Layered solution template offers to you.

## Pre-Installed Libraries & Services

The following **libraries and services** come **pre-installed** and **configured** for both **development** and **production** environments. After creating your solution, you can **modify** or **remove** most of them as needed.

* **[Autofac](https://autofac.org/)** for [Dependency Injection](../../framework/fundamentals/dependency-injection.md).  
* **[Serilog](https://serilog.net/)** with File and Console [logging](../../framework/fundamentals/logging.md) providers.  
* **[Redis](https://redis.io/)** for [distributed caching](../../framework/fundamentals/caching.md). Redis is used for distributed caching if you select the *Public Website* **\*** or *Tiered* **\*** option.  
* **[Swagger](https://swagger.io/)** for exploring and testing HTTP APIs.  
* **[OpenIddict](https://github.com/openiddict/openiddict-core)** as the built-in authentication server.  

## Pre-Configured Features

The following features are built and pre-configured for you in the solution.

* **Authentication** is fully configured based on best practices.
* **[Permission](../../framework/fundamentals/authorization.md)** (authorization), **[setting](../../framework/infrastructure/settings.md)**, **[feature](../../framework/infrastructure/features.md)** and the **[localization](../../framework/fundamentals/localization.md)** management systems are pre-configured and ready to use.
* **[Background job system](../../framework/infrastructure/background-jobs/index.md)**.
* **[BLOB storge](../../framework/infrastructure/blob-storing/index.md)** system is installed with the [database provider](../../framework/infrastructure/blob-storing/database.md).
* **On-the-fly database migration** system (services automatically migrated their database schema when you deploy a new version). **\***
* **[Helm](https://helm.sh/)** charts are included to deploy the solution to **[Kubernetes](https://kubernetes.io/)**. **\***
* **[Swagger](https://swagger.io/)** authentication is configured to test the authorized HTTP APIs.

## Fundamental Modules

The following modules are pre-installed and configured for the solution:

* **[Account](../../modules/account.md)** to authenticate users (login, register, two factor auth **\***, etc)
* **[Identity](../../modules/identity.md)** to manage roles and users
* **[OpenIddict](../../modules/openiddict.md)** (the core part) to implement the OAuth authentication flows

In addition these, [Feature Management](../../modules/feature-management.md), [Permission Management](../../modules/permission-management.md) and [Setting Management](../../modules/setting-management.md) modules are pre-installed as they are the fundamental feature modules of the ABP.

## Optional Modules

The following modules are optionally included in the solution, so you can select the ones you need:

* **[Audit Logging](../../modules/audit-logging.md)**
* **[Chat](../../modules/chat.md)** **\***
* **[File Management](../../modules/file-management.md)** **\***
* **[GDPR](../../modules/gdpr.md)** **\***
* **[Language Management](../../modules/language-management.md)** **\***
* **[OpenIddict (Management UI)](../../modules/openiddict.md)** **\***
* **[Tenant Management](../../modules/tenant-management.md) (Multi-Tenancy) or [SaaS](../../modules/saas.md)** **\*** 
* **[Text Template Management](../../modules/text-template-management.md)** **\***

## UI Theme

The **[LeptonX Lite](../../ui-themes/lepton-x-lite/index.md) or [LeptonX theme](https://leptontheme.com/)** **\*** is pre-configured for the solution. You can select one of the color palettes (System, Light or Dark) as default, while the end-user dynamically change it on the fly.

## Other Options

Layered startup template asks for some preferences while creating your solution.

### Database Providers

There are two database provider options are provided on a new solution creation:

* **[Entity Framework Core](../../framework/data/entity-framework-core/index.md)** with SQL Server, MySQL and PostgreSQL DBMS options. You can [switch to anther DBMS](../../framework/data/entity-framework-core/other-dbms.md) manually after creating your solution.
* **[MongoDB](../../framework/data/mongodb/index.md)**

### UI Frameworks

The solution comes with a main web application with the following UI Framework options:

* **None** (doesn't include a web application to the solution)
* **Angular**
* **MVC / Razor Pages UI**
* **Blazor WebAssembly**
* **Blazor Server**
* **Blazor WebApp**
* **MAUI with Blazor (Hybrid)** **\***

### The Mobile Application

If you prefer, the solution includes a mobile application. The mobile application is fully integrated to the system, implements authentication (login) and other ABP features, and includes a few screens that you can use and take as example. The following options are available:

* **None** (doesn't include a mobile application to the solution)
* **MAUI** **\***
* **React Native** **\***

### Multi-Tenancy & SaaS Module **\***

The **[SaaS module](../../modules/saas.md)** is included as an option. When you select it, the **[multi-tenancy](../../framework/architecture/multi-tenancy/index.md)** system is automatically configured. Otherwise, the system will not include any multi-tenancy overhead.

## See Also

* [Quick Start: Creating a Layered Web Application with ABP Studio](../../get-started/layered-web-application.md)