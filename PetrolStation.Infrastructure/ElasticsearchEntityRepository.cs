using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public class ElasticsearchEntityRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected string IndexName { get; }

        protected IElasticClient ElasticClient { get; }

        public ElasticsearchEntityRepository(IElasticClient elasticClient, IndexNameProvider<TEntity> indexNameProvider)
        {
            this.ElasticClient = elasticClient;
            this.IndexName = indexNameProvider.IndexName;
        }

        public async Task<TEntity> FindAsync(Guid id)
        {
            var nestId = (id as Guid?).Value;
            var response = await ElasticClient.GetAsync(DocumentPath<TEntity>.Id(nestId).Index(IndexName));
            return response.Source;
        }

        public async Task<TEntity> GetAsync(Guid id)
        {
            var response = await FindAsync(id);

            if (response is null)
                throw new KeyNotFoundException($"Entity with id: {id} does not exist.");

            return response;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var nestId = ((entity).Id as Guid?).Value;

            await ElasticClient.DeleteAsync(DocumentPath<TEntity>.Id(((IEntity) entity).Id)
                .Index(IndexName));
            await ElasticClient.RefreshAsync(IndexName);
        }

        public async Task AddAsync(TEntity entity)
        {
            await ElasticClient.IndexAsync(entity, z => z.Index(this.IndexName));
            await ElasticClient.RefreshAsync(IndexName);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await ElasticClient.UpdateAsync<TEntity>(entity,
                descriptor => descriptor
                    .Doc(entity)
                    .Index(IndexName));

            await ElasticClient.RefreshAsync(IndexName);
        }

        public async Task UpdatePartAsync<TPart>(TPart upsertData) where TPart : class, IEntity
        {
            await ElasticClient.UpdateAsync<TEntity, TPart>(DocumentPath<TEntity>.Id(upsertData.Id),
                descriptor => descriptor
                    .Doc(upsertData)
                    .Index(IndexName));

            await ElasticClient.RefreshAsync(IndexName);
        }
    }
}
