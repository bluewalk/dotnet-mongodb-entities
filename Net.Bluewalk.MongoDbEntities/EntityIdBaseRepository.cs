using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Net.Bluewalk.MongoDbEntities.Abstract;

namespace Net.Bluewalk.MongoDbEntities
{
    public class EntityIdBaseRepository<T> : EntityBaseRepository<T>, IEntityIdBaseRepository<T>
        where T : class, IEntityIdBase, new()
    {
        /// <summary>
        /// Entity base repository constructor
        /// </summary>
        /// <param name="connectionString">Format: mongodb://username:password@host:27017/database</param>
        protected EntityIdBaseRepository(string connectionString) : base(connectionString) { }
        
        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetSingle(long id)
        {
            return Collection.Find(q => q.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(long id)
        {
            return await Collection.Find(q => q.Id == id)
                .FirstOrDefaultAsync();
        }

        private long Add(T entity)
        {
            entity.Id = Collection.Find(e => true)
                         .Project(e => new {e.Id})
                         .SortByDescending(e => e.Id)
                         .FirstOrDefault()?.Id + 1 ?? 1;

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
                return -1;
            }

            return entity.Id;
        }
        
        private async Task<long> AddAsync(T entity)
        {
            entity.Id = (await Collection.Find(e => true)
                .Project(e => new { e.Id })
                .SortByDescending(e => e.Id)
                .FirstOrDefaultAsync())?.Id + 1 ?? 1;

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
                return -1;
            }

            return entity.Id;
        }

        private long Update(T entity)
        {
            try
            {
                Collection.ReplaceOne(q => q.Id == entity.Id, entity);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                return -1;
            }

            return entity.Id;
        }

        private async Task<long> UpdateAsync(T entity)
        {
            try
            {
                await Collection.ReplaceOneAsync(q => q.Id == entity.Id, entity);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, e);
                return -1;
            }

            return entity.Id;
        }

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        public virtual long Save(T entity)
        {
            return entity.Id > 0 ? Update(entity) : Add(entity);
        }


        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        public virtual async Task<long> SaveAsync(T entity)
        {
            return await (entity.Id > 0 ? UpdateAsync(entity) : AddAsync(entity));
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