using System.Text.RegularExpressions;

namespace Net.Bluewalk.MongoDbEntities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Replace the end of a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceEnd(this string source, string value, string replacement)
        {
            return Regex.Replace(source, $"{value}$", replacement);
        }
    }
}
