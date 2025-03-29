using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using CompanyName.ProjectName.Core.Abstractions.Repositories;
using CompanyName.ProjectName.Core.Models.Domain;
using CompanyName.ProjectName.Repository.Data;
using CompanyName.ProjectName.Repository.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace CompanyName.ProjectName.Repository.Repositories
{
    public class BaseRepository<TDomainModel, TEntity> : IBaseRepository<TDomainModel>
        where TEntity : BaseEntity
        where TDomainModel : BaseModel
    {
        protected CompanyNameProjectNameContext Context;
        protected IMapper Mapper;

        public BaseRepository(CompanyNameProjectNameContext context, IMapper mapper)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> ExistsAsync(int id) => await GetByIdAsync(id) != null;

        public async Task<TDomainModel> GetByIdAsync(int id) =>
            Mapper.Map<TDomainModel>(await FirstOrDefaultAsync(x => x.Id == id));

        public async Task<TDomainModel> GetByGuidAsync(Guid guid) =>
            await FirstOrDefaultAsync(x => x.Guid == guid);

        public async Task<TDomainModel> FirstOrDefaultAsync(Expression<Func<TDomainModel, bool>> domainPredicate)
        {
            if (domainPredicate == null)
            {
                throw new ArgumentNullException(nameof(domainPredicate));
            }

            var entityPredicate = Mapper.Map<Expression<Func<TEntity, bool>>>(domainPredicate);
            var entity = await Context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(entityPredicate);
            return Mapper.Map<TDomainModel>(entity);
        }

        public async Task<IEnumerable<TDomainModel>> GetAllAsync() =>
            Mapper.Map<IEnumerable<TDomainModel>>(await Context.Set<TEntity>().AsNoTracking().ToListAsync());

        public async Task<IEnumerable<TDomainModel>> GetWhereAsync(Expression<Func<TDomainModel, bool>> domainPredicate)
        {
            if (domainPredicate == null)
            {
                throw new ArgumentNullException(nameof(domainPredicate));
            }

            var entityPredicate = Mapper.Map<Expression<Func<TEntity, bool>>>(domainPredicate);
            var entities = await Context.Set<TEntity>().AsNoTracking().Where(entityPredicate).ToListAsync();
            return Mapper.Map<IEnumerable<TDomainModel>>(entities);
        }

        public async Task<IEnumerable<TDomainModel>> GetByIdsAsync(IEnumerable<int> ids)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var entities = await Context.Set<TEntity>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();
            return Mapper.Map<IEnumerable<TDomainModel>>(entities);
        }

        public async Task<IEnumerable<TDomainModel>> GetByGuidsAsync(IEnumerable<Guid> guids)
        {
            if (guids is null)
            {
                throw new ArgumentNullException(nameof(guids));
            }

            var entities = await Context.Set<TEntity>().AsNoTracking().Where(x => guids.Contains(x.Guid)).ToListAsync();
            return Mapper.Map<IEnumerable<TDomainModel>>(entities);
        }

        public async Task<int> CountAllAsync() =>
            await Context.Set<TEntity>().AsNoTracking().CountAsync();

        public async Task<int> CountWhereAsync(Expression<Func<TDomainModel, bool>> domainPredicate)
        {
            if (domainPredicate == null)
            {
                throw new ArgumentNullException(nameof(domainPredicate));
            }

            var entityPredicate = Mapper.Map<Expression<Func<TEntity, bool>>>(domainPredicate);
            return await Context.Set<TEntity>().AsNoTracking().CountAsync(entityPredicate);
        }

        public async Task CreateAsync(TDomainModel domainModel)
        {
            if (domainModel == null)
            {
                throw new ArgumentNullException(nameof(domainModel));
            }

            var entity = Mapper.Map<TEntity>(domainModel);
            SetCreateMetadata(entity);
            await Context.Set<TEntity>().AddAsync(entity);
            await SaveChangesAsync();
            Mapper.Map(entity, domainModel);
        }

        public async Task BulkCreateAsync(IEnumerable<TDomainModel> domainModels)
        {
            if (domainModels is null || !domainModels.Any())
            {
                return;
            }

            var entities = Mapper.Map<List<TEntity>>(domainModels.ToList());
            foreach (var entity in entities)
            {
                SetCreateMetadata(entity);
            }

            await Context.BulkInsertAsync(entities);
        }

        public async Task UpdateAsync(TDomainModel domainModel)
        {
            if (domainModel == null)
            {
                throw new ArgumentNullException(nameof(domainModel));
            }

            var entity = Mapper.Map<TEntity>(domainModel);
            SetUpdateMetadata(entity);
            Context.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public async Task BulkUpdateAsync(IEnumerable<TDomainModel> domainModels)
        {
            if (domainModels is null || !domainModels.Any())
            {
                return;
            }

            var entities = Mapper.Map<List<TEntity>>(domainModels.ToList());
            foreach (var entity in entities)
            {
                SetUpdateMetadata(entity);
            }

            await Context.BulkUpdateAsync(entities);
        }

        public async Task DeleteAsync(TDomainModel domainModel)
        {
            if (domainModel == null)
            {
                throw new ArgumentNullException(nameof(domainModel));
            }

            Context.Set<TEntity>().Remove(Mapper.Map<TEntity>(domainModel));
            await SaveChangesAsync();
        }

        public async Task BulkDeleteAsync(IEnumerable<TDomainModel> domainModels)
        {
            if (domainModels is null || !domainModels.Any())
            {
                return;
            }

            var entities = Mapper.Map<List<TEntity>>(domainModels.ToList());
            await Context.BulkDeleteAsync(entities);
        }

        public async Task SaveChangesAsync() => await Context.SaveChangesAsync();

        private void SetCreateMetadata(TEntity entity)
        {
            entity.CreatedBy = 0;
            entity.CreatedOn = DateTimeOffset.UtcNow;
            entity.Guid = Guid.NewGuid();
            SetUpdateMetadata(entity, entity.CreatedOn);
        }

        private void SetUpdateMetadata(TEntity entity, DateTimeOffset? modified = null)
        {
            entity.ModifiedBy = 0;
            entity.ModifiedOn = modified ?? DateTimeOffset.UtcNow;
        }
    }
}
