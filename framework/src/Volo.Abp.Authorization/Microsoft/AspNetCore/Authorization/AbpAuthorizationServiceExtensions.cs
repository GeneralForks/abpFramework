﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;

namespace Microsoft.AspNetCore.Authorization;

public static class AbpAuthorizationServiceExtensions
{
    public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, string policyName)
    {
        return await AuthorizeAsync(
            authorizationService,
            null,
            policyName
        );
    }

    public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, object resource, IAuthorizationRequirement requirement)
    {
        return await authorizationService.AuthorizeAsync(
            authorizationService.AsAbpAuthorizationService().CurrentPrincipal,
            resource,
            requirement
        );
    }

    public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, object? resource, AuthorizationPolicy policy)
    {
        return await authorizationService.AuthorizeAsync(
            authorizationService.AsAbpAuthorizationService().CurrentPrincipal,
            resource,
            policy
        );
    }

    public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, AuthorizationPolicy policy)
    {
        return await AuthorizeAsync(
            authorizationService,
            null,
            policy
        );
    }

    public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, object resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        return await authorizationService.AuthorizeAsync(
            authorizationService.AsAbpAuthorizationService().CurrentPrincipal,
            resource,
            requirements
        );
    }

    public static async Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService authorizationService, object? resource, string policyName)
    {
        return await authorizationService.AuthorizeAsync(
            authorizationService.AsAbpAuthorizationService().CurrentPrincipal,
            resource,
            policyName
        );
    }

    public static async Task<bool> IsGrantedAsync(this IAuthorizationService authorizationService, string policyName)
    {
        return (await authorizationService.AuthorizeAsync(policyName)).Succeeded;
    }

    public static async Task<bool> IsGrantedAnyAsync(
        this IAuthorizationService authorizationService,
        params string[] policyNames)
    {
        Check.NotNullOrEmpty(policyNames, nameof(policyNames));

        foreach (var policyName in policyNames)
        {
            if ((await authorizationService.AuthorizeAsync(policyName)).Succeeded)
            {
                return true;
            }
        }

        return false;
    }

    public static async Task<bool> IsGrantedAsync(this IAuthorizationService authorizationService, object resource, IAuthorizationRequirement requirement)
    {
        return (await authorizationService.AuthorizeAsync(resource, requirement)).Succeeded;
    }

    public static async Task<bool> IsGrantedAsync(this IAuthorizationService authorizationService, object resource, AuthorizationPolicy policy)
    {
        return (await authorizationService.AuthorizeAsync(resource, policy)).Succeeded;
    }

    public static async Task<bool> IsGrantedAsync(this IAuthorizationService authorizationService, AuthorizationPolicy policy)
    {
        return (await authorizationService.AuthorizeAsync(policy)).Succeeded;
    }

    public static async Task<bool> IsGrantedAsync(this IAuthorizationService authorizationService, object resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        return (await authorizationService.AuthorizeAsync(resource, requirements)).Succeeded;
    }

    public static async Task<bool> IsGrantedAsync(this IAuthorizationService authorizationService, object resource, string policyName)
    {
        return (await authorizationService.AuthorizeAsync(resource, policyName)).Succeeded;
    }

    /// <summary>
    /// Checks if CurrentPrincipal meets a specific authorization policy, throwing an <see cref="AbpAuthorizationException"/> if not.
    /// </summary>
    /// <param name="authorizationService">The <see cref="IAuthorizationService"/> providing authorization.</param>
    /// <param name="policyName">The name of the policy to evaluate.</param>
    public static async Task CheckAsync(this IAuthorizationService authorizationService, string policyName)
    {
        if (!await authorizationService.IsGrantedAsync(policyName))
        {
            throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenPolicyHasNotGrantedWithPolicyName)
                .WithData("PolicyName", policyName);
        }
    }

    /// <summary>
    /// Checks if CurrentPrincipal meets a specific requirement for the specified resource, throwing an <see cref="AbpAuthorizationException"/> if not.
    /// </summary>
    /// <param name="authorizationService">The <see cref="IAuthorizationService"/> providing authorization.</param>
    /// <param name="resource">The resource to evaluate the policy against.</param>
    /// <param name="requirement">The requirement to evaluate the policy against.</param>
    public static async Task CheckAsync(this IAuthorizationService authorizationService, object resource, IAuthorizationRequirement requirement)
    {
        if (!await authorizationService.IsGrantedAsync(resource, requirement))
        {
            throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenRequirementHasNotGrantedForGivenResource)
                .WithData("ResourceName", resource);
        }
    }

    /// <summary>
    /// Checks if CurrentPrincipal meets a specific authorization policy against the specified resource, throwing an <see cref="AbpAuthorizationException"/> if not.
    /// </summary>
    /// <param name="authorizationService">The <see cref="IAuthorizationService"/> providing authorization.</param>
    /// <param name="resource">The resource to evaluate the policy against.</param>
    /// <param name="policy">The policy to evaluate.</param>
    public static async Task CheckAsync(this IAuthorizationService authorizationService, object resource, AuthorizationPolicy policy)
    {
        if (!await authorizationService.IsGrantedAsync(resource, policy))
        {
            throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenPolicyHasNotGrantedForGivenResource)
                .WithData("ResourceName", resource);
        }
    }

    /// <summary>
    /// Checks if CurrentPrincipal meets a specific authorization policy, throwing an <see cref="AbpAuthorizationException"/> if not.
    /// </summary>
    /// <param name="authorizationService">The <see cref="IAuthorizationService"/> providing authorization.</param>
    /// <param name="policy">The policy to evaluate.</param>
    public static async Task CheckAsync(this IAuthorizationService authorizationService, AuthorizationPolicy policy)
    {
        if (!await authorizationService.IsGrantedAsync(policy))
        {
            throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenPolicyHasNotGranted);
        }
    }

    /// <summary>
    /// Checks if CurrentPrincipal meets a specific authorization policy against the specified resource, throwing an <see cref="AbpAuthorizationException"/> if not.
    /// </summary>
    /// <param name="authorizationService">The <see cref="IAuthorizationService"/> providing authorization.</param>
    /// <param name="resource">The resource to evaluate the policy against.</param>
    /// <param name="requirements">The requirements to evaluate the policy against.</param>
    public static async Task CheckAsync(this IAuthorizationService authorizationService, object resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        if (!await authorizationService.IsGrantedAsync(resource, requirements))
        {
            throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenRequirementsHasNotGrantedForGivenResource)
                .WithData("ResourceName", resource);
        }
    }

    /// <summary>
    /// Checks if CurrentPrincipal meets a specific authorization policy against the specified resource, throwing an <see cref="AbpAuthorizationException"/> if not.
    /// </summary>
    /// <param name="authorizationService">The <see cref="IAuthorizationService"/> providing authorization.</param>
    /// <param name="resource">The resource to evaluate the policy against.</param>
    /// <param name="policyName">The name of the policy to evaluate.</param>
    public static async Task CheckAsync(this IAuthorizationService authorizationService, object resource, string policyName)
    {
        if (!await authorizationService.IsGrantedAsync(resource, policyName))
        {
            throw new AbpAuthorizationException(code: AbpAuthorizationErrorCodes.GivenPolicyHasNotGrantedForGivenResource)
                .WithData("ResourceName", resource);
        }
    }

    private static IAbpAuthorizationService AsAbpAuthorizationService(this IAuthorizationService authorizationService)
    {
        if (!(authorizationService is IAbpAuthorizationService abpAuthorizationService))
        {
            throw new AbpException($"{nameof(authorizationService)} should implement {typeof(IAbpAuthorizationService).FullName}");
        }

        return abpAuthorizationService;
    }
}
