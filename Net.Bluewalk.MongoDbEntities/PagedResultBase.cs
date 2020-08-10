using System;

namespace Net.Bluewalk.MongoDbEntities
{
    /// <summary>
    /// PagedResult Base-class
    /// </summary>
    public abstract class PagedResultBase
    {
        /// <summary>
        /// Current page
        /// </summary>
        public long PageCurrent { get; set; }
        /// <summary>
        /// Total pages
        /// </summary>
        public long PageCount { get; set; }
        /// <summary>
        /// Rows per page
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// Total rows
        /// </summary>
        public long RowCount { get; set; }
        /// <summary>
        /// First row on page
        /// </summary>
        public long FirstRowOnPage => (PageCurrent - 1) * PageSize + 1;
        /// <summary>
        /// Last row on page
        /// </summary>
        public long LastRowOnPage => Math.Min(PageCurrent * PageSize, RowCount);
    }
}