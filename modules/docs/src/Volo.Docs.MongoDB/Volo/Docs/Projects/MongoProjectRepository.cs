﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using Volo.Docs.MongoDB;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Volo.Docs.Projects
{
    public class MongoProjectRepository : MongoDbRepository<IDocsMongoDbContext, Project, Guid>, IProjectRepository
    {
        public MongoProjectRepository(IMongoDbContextProvider<IDocsMongoDbContext> dbContextProvider) : base(
            dbContextProvider)
        {
        }

        public virtual async Task<List<Project>> GetListAsync(string sorting, int maxResultCount, int skipCount, CancellationToken cancellationToken = default)
        {
            var projects = await (await GetQueryableAsync(cancellationToken)).OrderBy(sorting.IsNullOrEmpty() ? "Id desc" : sorting).PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            return projects;
        }

        public virtual async Task<List<ProjectWithoutDetails>> GetListWithoutDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync(cancellationToken))
                .Select(x=> new ProjectWithoutDetails {
                    Id = x.Id,
                    Name = x.Name,
                })
                .OrderBy(x=>x.Name)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<Project> GetByShortNameAsync(string shortName, CancellationToken cancellationToken = default)
        {
            var normalizeShortName = NormalizeShortName(shortName);

            var project = await (await GetQueryableAsync(cancellationToken)).FirstOrDefaultAsync(p => p.ShortName == normalizeShortName, GetCancellationToken(cancellationToken));

            if (project == null)
            {
                throw new EntityNotFoundException($"Project with the name {shortName} not found!");
            }

            return project;
        }

        public virtual async Task<bool> ShortNameExistsAsync(string shortName, CancellationToken cancellationToken = default)
        {
            var normalizeShortName = NormalizeShortName(shortName);

            return await (await GetQueryableAsync(cancellationToken)).AnyAsync(x => x.ShortName == normalizeShortName, GetCancellationToken(cancellationToken));
        }

        private string NormalizeShortName(string shortName)
        {
            return shortName.ToLower();
        }
    }
}
