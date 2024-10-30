using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Net.Bluewalk.MongoDbEntities.Abstract;

namespace Net.Bluewalk.MongoDbEntities
{
    public class EntityGuidBaseRepository<T> : EntityBaseRepository<T>, IEntityGuidBaseRepository<T>
        where T : class, IEntityGuidBase, new()
    {
        /// <summary>
        /// Entity base repository constructor
        /// </summary>
        /// <param name="connectionString">Format: mongodb://username:password@host:27017/database</param>
        protected EntityGuidBaseRepository(string connectionString) : base(connectionString) { }
        
        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetSingle(Guid id)
        {
            return Collection.Find(q => q.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(Guid id)
        {
            return await Collection.Find(q => q.Id == id)
                .FirstOrDefaultAsync();
        }

        private Guid Add(T entity)
        {
            entity.Id = Guid.NewGuid();

            try
            {
                Collection.InsertOne(entity);
            }
            catch (MongoWriteException we)
            {
                if (we.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return Add(entity);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                return default;
            }

            return entity.Id;
        }

        private async Task<Guid> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();

            try
            {
                await Collection.InsertOneAsync(entity);
            }
            catch (MongoWriteException we)
            {
                if (we.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    return await AddAsync(entity);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                return default;
            }

            return entity.Id;
        }

        private Guid Update(T entity)
        {
            try
            {
                Collection.ReplaceOne(q => q.Id == entity.Id, entity);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                return default;
            }

            return entity.Id;
        }

        private async Task<Guid> UpdateAsync(T entity)
        {
            try
            {
                await Collection.ReplaceOneAsync(q => q.Id == entity.Id, entity);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                return default;
            }

            return entity.Id;
        }

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        public virtual Guid Save(T entity)
        {
            return entity.Id != default ? Update(entity) : Add(entity);
        }

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        public virtual async Task<Guid> SaveAsync(T entity)
        {
            return await (entity.Id != default ? UpdateAsync(entity) : AddAsync(entity));
        }

        /// <summary>
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            Collection.DeleteOne(q => q.Id == entity.Id);
        }

        /// <summary>
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual async Task DeleteAsync(T entity)
        {
            await Collection.DeleteOneAsync(q => q.Id == entity.Id);
        }
    }
}