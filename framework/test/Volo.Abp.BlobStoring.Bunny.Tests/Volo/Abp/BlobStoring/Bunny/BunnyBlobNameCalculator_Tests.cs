﻿using System;
using Shouldly;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Volo.Abp.BlobStoring.Bunny;

public class BunnyBlobNameCalculatorTests : AbpBlobStoringBunnyTestCommonBase
{
    private readonly IBunnyBlobNameCalculator _calculator;
    private readonly ICurrentTenant _currentTenant;

    private const string BunnyContainerName = "/";
    private const string BunnySeparator = "/";

    public BunnyBlobNameCalculatorTests()
    {
        _calculator = GetRequiredService<IBunnyBlobNameCalculator>();
        _currentTenant = GetRequiredService<ICurrentTenant>();
    }

    [Fact]
    public void Default_Settings()
    {
        _calculator.Calculate(
            GetArgs("my-container", "my-blob")
        ).ShouldBe($"host{BunnySeparator}my-blob");
    }

    [Fact]
    public void Default_Settings_With_TenantId()
    {
        var tenantId = Guid.NewGuid();

        using (_currentTenant.Change(tenantId))
        {
            _calculator.Calculate(
                GetArgs("my-container", "my-blob")
            ).ShouldBe($"tenants{BunnySeparator}{tenantId:D}{BunnySeparator}my-blob");
        }
    }

    private static BlobProviderArgs GetArgs(
        string containerName,
        string blobName)
    {
        return new BlobProviderGetArgs(
            containerName,
            new BlobContainerConfiguration().UseBunny(x =>
            {
                x.ContainerName = containerName;
            }),
            blobName
        );
    }
}
