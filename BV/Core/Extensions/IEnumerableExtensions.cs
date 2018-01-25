using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace VB.Common.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToDelimitedString<T>(this IEnumerable<T> enumerable, string delimiter)
        {
            var items = enumerable as T[] ?? enumerable.ToArray();
            if (items.Count() == 0)
                return String.Empty;

            var sb = new StringBuilder();

            foreach (var item in items)
            {
                sb.Append(item);
                sb.Append(delimiter);
            }

            // Remove the trailing delimiter.
            if( sb.Length > 0 )
            {
                sb.Length = sb.Length - delimiter.Length;
            }

            return sb.ToString();
        }


        public static IQueryable<T> DynamicOrderBy<T>(this IQueryable<T> collection, string propertyPath, EnumerableSortDirection sortDirection)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return collection;
            }

            Type collectionType = typeof(T);

            ParameterExpression parameterExpression = Expression.Parameter(collectionType, "p");

            Expression seedExpression = parameterExpression;

            Expression aggregateExpression = propertyPath.Split('.').Aggregate(seedExpression, Expression.Property);

            MemberExpression memberExpression = aggregateExpression as MemberExpression;

            if (memberExpression == null)
            {
                throw new NullReferenceException(string.Format("Unable to cast Member Expression for given path: {0}.", propertyPath));
            }

            LambdaExpression orderByExp = Expression.Lambda(memberExpression, parameterExpression);

            const string orderBy = "OrderBy";

            const string orderByDesc = "OrderByDescending";

            Type childPropertyType = ((PropertyInfo)(memberExpression.Member)).PropertyType;

            string methodToInvoke = sortDirection == EnumerableSortDirection.Ascending ? orderBy : orderByDesc;

            var orderByCall = Expression.Call(typeof(Queryable), methodToInvoke, new[] { collectionType, childPropertyType }, collection.Expression, Expression.Quote(orderByExp));

            return collection.Provider.CreateQuery<T>(orderByCall);

        }

        /// <summary>
        /// Returns *pageSize* items from an IEnumerable beginning at *start*.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> items, int start, int pageSize)
        {
            return items.Skip(start).Take(pageSize);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }

    public enum EnumerableSortDirection
    {
        Ascending = 1,
        Descending = 2
    }
}
