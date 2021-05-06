using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Driver;
using Net.Bluewalk.MongoDbEntities.Abstract;
using Net.Bluewalk.MongoDbEntities.Attributes;

namespace Net.Bluewalk.MongoDbEntities
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
        where T: class, new()
    {
        /// <summary>
        /// MongoClient
        /// </summary>
        protected readonly MongoClient _client;

        /// <summary>
        /// Database
        /// </summary>
        protected readonly IMongoDatabase _database;
        
        /// <summary>
        /// MongoCollection
        /// </summary>
        protected readonly IMongoCollection<T> _collection;

        /// <summary>
        /// When an exception occurs this event will be fired
        /// </summary>
        public EventHandler<Exception> OnException;

        public EntityBaseRepository(string connectionString)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(connectionString.Split('/').Last());
            _collection = GetCollection<T>();

            EnsureIndexes();
        }

        /// <summary>
        /// Determine name for table
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <returns></returns>
        protected virtual string GetTableName<TT>()
        {
            var table = typeof(TT).GetCustomAttribute<TableAttribute>()?.Name;

            if (string.IsNullOrEmpty(table))
                table = (typeof(TT).Name + "s")
                    .ReplaceEnd("ys", "ies")
                    .ReplaceEnd("ss", "ses");

            return table;
        }

        /// <summary>
        /// Get collection
        /// </summary>
        /// <typeparam name="TT"></typeparam>
        /// <returns></returns>
        protected IMongoCollection<TT> GetCollection<TT>()
        {
            return _database.GetCollection<TT>(GetTableName<TT>(), new MongoCollectionSettings
            {
                AssignIdOnInsert = true
            });
        }

        /// <summary>
        /// Ensure required indexes are created
        /// </summary>
        protected virtual void EnsureIndexes() { }

        /// <summary>
        /// Gets all entities in a paged format
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll(int limit = 50, int page = 1)
        {
            var query = (IQueryable<T>)_collection.AsQueryable();

            if (limit > 0)
                query = query.Skip(limit * (page - 1)).Take(limit);

            return query;
        }

        /// <summary>
        /// Gets all entities in a paged format
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(int limit = 50, int page = 1)
        {
            if (limit > 0)
                return await _collection.Find(q => true).Skip(limit * (page - 1)).Limit(limit).ToListAsync();

            return await _collection.Find(q => true).ToListAsync();
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
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        public virtual async Task<long> CountAsync()
        {
            return await _collection.CountDocumentsAsync(q => true);
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
        /// Gets a single entity matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            var builder = Builders<T>.Filter;
            var query = builder.Where(predicate);

            return await _collection.Find(query).FirstOrDefaultAsync();
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

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            _collection.DeleteMany(predicate);
        }

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        public virtual async Task DeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            await _collection.DeleteManyAsync(predicate);
        }
    }
}
