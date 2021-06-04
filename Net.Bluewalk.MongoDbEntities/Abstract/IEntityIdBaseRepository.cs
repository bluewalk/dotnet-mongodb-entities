using System.Threading.Tasks;

namespace Net.Bluewalk.MongoDbEntities.Abstract
{
    public interface IEntityIdBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityIdBase, new()
    {
        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetSingle(long id);

        /// <summary>
        /// Gets a single entity matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetSingleAsync(long id);

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        long Save(T entity);

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The inserted ID</returns>
        Task<long> SaveAsync(T entity);

        /// <summary>
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Deletes given entity
        /// </summary>
        /// <param name="entity"></param>
        Task DeleteAsync(T entity);
    }
}
