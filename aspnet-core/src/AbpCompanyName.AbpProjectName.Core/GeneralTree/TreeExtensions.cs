using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;

//https://github.com/maliming/Abp.GeneralTree/blob/master/README.CN.md

namespace Abp.GeneralTree
{
    public static class TreeExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var element in source)
            {
                action(element);
            }
        }

        public static IEnumerable<TTree> ToTreeDtoOrder<TTree, TPrimaryKey>(
     this IEnumerable<TTree> tree, Func<IEnumerable<TTree>, IEnumerable<TTree>> order)
     where TPrimaryKey : struct
     where TTree : class, IGeneralTreeDto<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId.HasValue).Select(x => x.Value).ForEach(x =>
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (!treeDic.ContainsKey(x.ParentId.Value))
                {
                    return;
                }

                var parent = treeDic[x.ParentId.Value];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value).ForEach(x =>
            {
                x.Children = order(x.Children).ToList();
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return order(treeDic.Values.Where(x => x.ParentId == null));
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }
    }
}