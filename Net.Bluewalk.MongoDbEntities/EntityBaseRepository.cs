using System;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using Net.Bluewalk.MongoDbEntities.Abstract;

namespace Net.Bluewalk.MongoDbEntities
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
        where T : class, IEntityBase, new()
    {
        /// <summary>
        /// MongoClient
        /// </summary>
        protected readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// MongoCollection
        /// </summary>
        protected readonly IMongoCollection<T> _collection;

        /// <summary>
        /// When an exception occurs this event will be fired
        /// </summary>
        public EventHandler<Exception> OnException;

        /// <summary>
        /// Entity base repository constructor
        /// </summary>
        /// <param name="connectionString">Format: mongodb://username:password@host:27017/database</param>
        protected EntityBaseRepository(string connectionString)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(connectionString.Split('/').Last());
            _collection = GetCollection<T>();

            EnsureIndexes();
        }

        /// <summary>
        /// Get collection
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <returns></returns>
        protected IMongoCollection<TT> GetCollection<TT>()
        {
            var table = (typeof(TT).Name + "s")
                .ReplaceEnd("ys", "ies")
                .ReplaceEnd("ss", "ses");

            return _database.GetCollection<TT>(table, new MongoCollectionSettings
            {
                AssignIdOnInsert = true
            });
        }

        /// <summary>
        /// Ensure required indexes are created
        /// </summary>
        protected virtual void EnsureIndexes()
        {

        }

        /// <summary>
        /// Gets all entities in a paged format
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll(int limit = 50, int page = 1)
        {
            var query = (IQueryable<T>) _collection.AsQueryable();

            if (limit > 0)
                query = query.Skip(limit * (page - 1)).Take(limit);

            return query;
        }

        /// <summary>
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return _collection.AsQueryable().Count();
        }

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

        /// <summary>
        /// Gets a single entity matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T GetSingle(Expression<Func<T, bool>> predicate)
        {
            var builder = Builders<T>.Filter;
            var query = builder.Where(predicate);

            return _collection.Find(query).FirstOrDefault();
        }

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate);
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

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            _collection.DeleteMany(predicate);
        }
    }
}