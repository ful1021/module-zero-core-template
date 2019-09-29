using System;
using System.Linq;
using System.Linq.Expressions;
using Abp.Linq.Extensions;

namespace AbpCompanyName.AbpProjectName.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIfString<T>(this IQueryable<T> query, string input, Func<string, Expression<Func<T, bool>>> sqlLike, Func<string[], Expression<Func<T, bool>>> sqlIn)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return query;
            }
            var arrs = input.ToStringArray();
            if (arrs.Length > 150)
            {
                arrs = arrs.Take(150).ToArray();
            }
            var str = input.Trim();

            return query
                .WhereIf(arrs.Length == 1, sqlLike(str))
                .WhereIf(arrs.Length > 1, sqlIn(arrs));
        }

        public static IQueryable<T> WhereIfString<T>(this IQueryable<T> query, string input, Func<string, Expression<Func<T, int, bool>>> sqlLike, Func<string[], Expression<Func<T, int, bool>>> sqlIn)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return query;
            }
            var arrs = input.ToStringArray();
            if (arrs.Length > 150)
            {
                arrs = arrs.Take(150).ToArray();
            }
            var str = input.Trim();
            return query.WhereIf(arrs.Length == 1, sqlLike(str))
                .WhereIf(arrs.Length > 1, sqlIn(arrs));
        }
    }
}