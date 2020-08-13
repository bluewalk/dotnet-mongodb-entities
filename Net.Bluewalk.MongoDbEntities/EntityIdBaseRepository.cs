using System;
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
            return _collection.Find(q => q.Id == id)
                .FirstOrDefault();
        }
        
        private long Add(T entity)
        {
            entity.Id = _collection.Find(e => true)
                         .Project(e => new {e.Id})
                         .SortByDescending(e => e.Id)
                         .FirstOrDefault()?.Id + 1 ?? 1;

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
                return -1;
            }

            return entity.Id;
        }

        private long Update(T entity)
        {
            try
            {
                _collection.ReplaceOne(q => q.Id == entity.Id, entity);
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
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            _collection.DeleteOne(q => q.Id == entity.Id);
        }
    }
}