﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Docs.Admin.Navigation;
using Volo.Docs.Localization;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Docs.Admin.Pages.Docs.Admin.SignalR;

namespace Volo.Docs.Admin
{
    [DependsOn(
        typeof(DocsAdminHttpApiModule),
        typeof(AbpAspNetCoreMvcUiBootstrapModule),
        typeof(AbpAspNetCoreSignalRModule)
        )]
    public class DocsAdminWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(DocsResource), typeof(DocsAdminWebModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(DocsAdminWebModule).Assembly);
            });

            context.Services.AddSignalR();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new DocsMenuContributor());
            });

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<DocsAdminWebModule>();
            });

            context.Services.AddAutoMapperObjectMapper<DocsAdminWebModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<DocsAdminWebAutoMapperProfile>(validate: true);
            });

            Configure<AbpBundlingOptions>(options =>
            {
                options
                    .ScriptBundles
                    .Get(StandardBundles.Scripts.Global)
                    .AddFiles("/libs/signalr/signalr.js")
                    .AddFiles("/Pages/Docs/Admin/docs-notification-hub.js");
            });

            Configure<AbpSignalROptions>(options =>
            {
                options.Hubs.AddOrUpdate(
                    typeof(UiNotificationHub), //Hub type
                    config => //Additional configuration
                    {
                        config.RoutePattern = "/document-notification-hub"; //override the default route
                        //config.ConfigureActions.Add(hubOptions =>
                        //{
                        //    //Additional options
                        //    hubOptions.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
                        //});
                    }
                );
            });
        }
    }
}
