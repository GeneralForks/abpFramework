﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Volo.Abp.Identity.EntityFrameworkCore
{
    public class EfCoreIdentityClaimTypeRepository : EfCoreRepository<IIdentityDbContext, IdentityClaimType, Guid>, IIdentityClaimTypeRepository
    {
        public EfCoreIdentityClaimTypeRepository(IDbContextProvider<IIdentityDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<bool> DoesNameExist(string name, Guid? claimTypeId = null)
        {
            return await DbSet.WhereIf(claimTypeId != null, ct => ct.Id != claimTypeId).CountAsync(ct => ct.Name == name) > 0;
        }

        public async Task<List<IdentityClaimType>> GetListAsync(string sorting, int maxResultCount, int skipCount)
        {
            var identityClaimTypes = await DbSet.OrderBy(sorting ?? "name desc")
                .PageBy(skipCount, maxResultCount)
                .ToListAsync();

            return identityClaimTypes;
        }

        public async Task<int> GetTotalCount()
        {
            return await DbSet.CountAsync();
        }
    }
}
