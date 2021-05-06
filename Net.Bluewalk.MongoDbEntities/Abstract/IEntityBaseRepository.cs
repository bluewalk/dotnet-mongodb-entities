using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Net.Bluewalk.MongoDbEntities.Abstract
{
    public interface IEntityBaseRepository<T> where T : class, new()
    {
        /// <summary>
        /// Gets all entities in a paged format
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IQueryable<T> GetAll(int limit = 50, int page = 1);

        /// <summary>
        /// Gets all entities in a paged format
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(int limit = 50, int page = 1);

        /// <summary>
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        Task<long> CountAsync();

        /// <summary>
        /// Gets a single entity matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T GetSingle(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets a single entity matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        void DeleteWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        Task DeleteWhereAsync(Expression<Func<T, bool>> predicate);
    }
}
