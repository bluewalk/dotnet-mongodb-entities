using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Net.Bluewalk.MongoDbEntities.Results;

namespace Net.Bluewalk.MongoDbEntities.Extensions
{
    public static class MongoQueryable
    {
        /// <summary>
        /// Get paged result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PagedResult<T> GetPaged<T>(this IMongoQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>
            {
                PageCurrent = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double) result.RowCount / pageSize;
            result.PageCount = (int) Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;

            result.Results = pageSize > 0 
                ? query.Skip(skip).Take(pageSize).ToList()
                : query.ToList();

            return result;
        }

        /// <summary>
        /// Get paged result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IMongoQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>
            {
                PageCurrent = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;

            result.Results = pageSize > 0 
                ? await query.Skip(skip).Take(pageSize).ToListAsync() 
                : await query.ToListAsync();

            return result;
        }
    }
}