using System;
using System.Linq;
using Net.Bluewalk.MongoDbEntities.Results;

namespace Net.Bluewalk.MongoDbEntities.Extensions
{
    public static class Queryable
    {
        /// <summary>
        /// Get paged result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
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

            result.Results = pageSize > 0 ? query.Skip(skip).Take(pageSize).ToList() : query.ToList();

            return result;
        }
    }
}