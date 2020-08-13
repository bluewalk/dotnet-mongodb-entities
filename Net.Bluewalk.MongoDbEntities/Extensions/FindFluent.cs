using System;
using System.Threading;
using MongoDB.Driver;
using Net.Bluewalk.MongoDbEntities.Results;

namespace Net.Bluewalk.MongoDbEntities.Extensions
{
    public static class FindFluent
    {
        /// <summary>
        /// Get paged result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProjection"></typeparam>
        /// <returns></returns>
        public static PagedResult<TProjection> GetPaged<T, TProjection>(this IFindFluent<T, TProjection> query,
            int page, int pageSize) where TProjection : class
        {
            var result = new PagedResult<TProjection>
            {
                PageCurrent = page,
                PageSize = pageSize,
                RowCount = query.CountDocuments(CancellationToken.None)
            };

            var pageCount = (double) result.RowCount / pageSize;
            result.PageCount = (int) Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;

            result.Results = pageSize > 0 ? query.Skip(skip).Limit(pageSize).ToList() : query.ToList();

            return result;
        }
    }
}