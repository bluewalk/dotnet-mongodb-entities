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
        protected readonly IMongoClient Client;

        /// <summary>
        /// Database
        /// </summary>
        protected readonly IMongoDatabase Database;

        /// <summary>
        /// MongoCollection
        /// </summary>
        protected readonly IMongoCollection<T> Collection;

        /// <summary>
        /// When an exception occurs this event will be fired
        /// </summary>
        public EventHandler<Exception> OnException;

        public EntityBaseRepository(string connectionString)
        {
            var mongoUrl = MongoUrl.Create(connectionString);

            Client = MongoClientSingleton.GetClient(mongoUrl);
            Database = Client.GetDatabase(mongoUrl.DatabaseName);
            Collection = GetCollection<T>();

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
            return Database.GetCollection<TT>(GetTableName<TT>(), new MongoCollectionSettings
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
        /// <param name="limit">0 for all records</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual IFindFluent<T, T> GetAll(int limit = 0, int page = 1)
        {
            var query = Collection.Find(q => true);

            if (limit > 0)
                query = query.Skip(limit * (page - 1)).Limit(limit);

            return query;
        }

        /// <summary>
        /// Gets all entities in a paged format
        /// </summary>
        /// <param name="limit">0 for all records</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync(int limit = 0, int page = 1)
        {
            return await GetAll(limit, page).ToListAsync();
        }

        /// <summary>
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return Collection.AsQueryable().Count();
        }

        /// <summary>
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        public virtual async Task<long> CountAsync()
        {
            return await Collection.CountDocumentsAsync(q => true);
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

            return Collection.Find(query).FirstOrDefault();
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

            return await Collection.Find(query).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="limit">0 for all records</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public virtual IFindFluent<T, T> FindBy(Expression<Func<T, bool>> predicate, int limit = 0, int page = 1)
        {
            var query = Collection.Find(predicate);

            if (limit > 0)
                query = query.Skip(limit * (page - 1)).Limit(limit);

            return query;
        }

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            Collection.DeleteMany(predicate);
        }

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        public virtual async Task DeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            await Collection.DeleteManyAsync(predicate);
        }
    }
}