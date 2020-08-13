using System;
using System.Linq;
using System.Linq.Expressions;

namespace Net.Bluewalk.MongoDbEntities.Abstract
{
    public interface IEntityGuidBaseRepository<T> where T : class, IEntityGuidBase, new()
    {
        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetSingle(Guid id);

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        Guid Save(T entity);

        /// <summary>
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
    }
}
