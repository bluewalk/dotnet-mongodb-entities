using System;
using System.Linq;
using System.Linq.Expressions;

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
        /// Gets total count of entities
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Gets a single entity matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T GetSingle(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Deletes entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        void DeleteWhere(Expression<Func<T, bool>> predicate);
    }
}
