using System;
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
            return _collection.Find(q => q.Id == id)
                .FirstOrDefault();
        }


        private Guid Add(T entity)
        {
            entity.Id = Guid.NewGuid();

            try
            {
                _collection.InsertOne(entity);
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

        private Guid Update(T entity)
        {
            try
            {
                _collection.ReplaceOne(q => q.Id == entity.Id, entity);
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
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            _collection.DeleteOne(q => q.Id == entity.Id);
        }
    }
}