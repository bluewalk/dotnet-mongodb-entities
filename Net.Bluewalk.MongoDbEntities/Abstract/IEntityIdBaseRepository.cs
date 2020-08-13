using System;
using System.Linq;
using System.Linq.Expressions;

namespace Net.Bluewalk.MongoDbEntities.Abstract
{
    public interface IEntityIdBaseRepository<T> where T : class, IEntityIdBase, new()
    {
        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetSingle(long id);

        /// <summary>
        /// Finds entities matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        long Save(T entity);

        /// <summary>
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
    }
}
