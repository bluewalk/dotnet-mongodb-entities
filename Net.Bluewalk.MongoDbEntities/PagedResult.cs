using System.Collections.Generic;

namespace Net.Bluewalk.MongoDbEntities
{
    /// <summary>
    /// Paged Result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T> : PagedResultBase where T : class
    {
        /// <summary>
        /// List of result objects
        /// </summary>
        public IList<T> Results { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}