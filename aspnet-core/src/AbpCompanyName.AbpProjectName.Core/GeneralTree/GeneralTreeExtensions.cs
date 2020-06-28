using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;

namespace Abp.GeneralTree
{
    public static class GeneralTreeExtensions
    {
        public static IEnumerable<TTree> ToTree<TTree, TPrimaryKey>(this IEnumerable<TTree> tree)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTree<TTree, TPrimaryKey>
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

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeOrderBy<TTree, TPrimaryKey, TTreeProperty>(this IEnumerable<TTree> tree,
            Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTree<TTree, TPrimaryKey>
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
                x.Children = x.Children.OrderBy(propertySelector).ToList();
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderBy(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeOrderByDescending<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTree<TTree, TPrimaryKey>
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
                x.Children = x.Children.OrderByDescending(propertySelector).ToList();
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderByDescending(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeWithReferenceType<TTree, TPrimaryKey>(this IEnumerable<TTree> tree)
            where TPrimaryKey : class
            where TTree : class, IGeneralTreeWithReferenceType<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId != null).Select(x => x.Value).ForEach(x =>
            {
                if (!treeDic.ContainsKey(x.ParentId))
                {
                    return;
                }

                var parent = treeDic[x.ParentId];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId));
        }

        public static IEnumerable<TTree> ToTreeWithReferenceTypeOrderBy<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : class
            where TTree : class, IGeneralTreeWithReferenceType<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId != null).Select(x => x.Value).ForEach(x =>
            {
                if (!treeDic.ContainsKey(x.ParentId))
                {
                    return;
                }

                var parent = treeDic[x.ParentId];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value).ForEach(x =>
            {
                x.Children = x.Children.OrderBy(propertySelector).ToList();
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderBy(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId));
        }

        public static IEnumerable<TTree> ToTreeWithReferenceTypeOrderByDescending<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : class
            where TTree : class, IGeneralTreeWithReferenceType<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId != null).Select(x => x.Value).ForEach(x =>
            {
                if (!treeDic.ContainsKey(x.ParentId))
                {
                    return;
                }

                var parent = treeDic[x.ParentId];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value).ForEach(x =>
            {
                x.Children = x.Children.OrderByDescending(propertySelector).ToList();
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderByDescending(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId));
        }


        public static IEnumerable<TTree> ToTreeDto<TTree, TPrimaryKey>(this IEnumerable<TTree> tree)
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

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeDtoOrderBy<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTreeDto<TTree, TPrimaryKey>
        {
            Dictionary<TPrimaryKey, TTree> treeDic = GetTreeDict<TTree, TPrimaryKey, TTreeProperty>(tree, propertySelector);

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderBy(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        private static Dictionary<TPrimaryKey, TTree> GetTreeDict<TTree, TPrimaryKey, TTreeProperty>(IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TTree : class, IGeneralTreeDto<TTree, TPrimaryKey>
            where TPrimaryKey : struct
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
                x.Children = x.Children.OrderBy(propertySelector).ToList();
            });
            return treeDic;
        }

        public static IEnumerable<TTree> ToTreeDtoOrderByWithNoParent<TTree, TPrimaryKey, TTreeProperty>(
       this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
       where TPrimaryKey : struct
       where TTree : class, IGeneralTreeDto<TTree, TPrimaryKey>
        {
            var treeDic = GetTreeDict<TTree, TPrimaryKey, TTreeProperty>(tree, propertySelector);

            List<TTree> result = new List<TTree>();
            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                result.AddRange(treeDic.Values.Where(x => x.ParentId == null).OrderBy(propertySelector));
            }
            result.AddRange(treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value)));
            return result;
        }

        public static IEnumerable<TTree> ToTreeDtoOrderByDescending<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
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
                x.Children = x.Children.OrderByDescending(propertySelector).ToList();
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderByDescending(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeDtoWithReferenceType<TTree, TPrimaryKey>(this IEnumerable<TTree> tree)
            where TPrimaryKey : class
            where TTree : class, IGeneralTreeDtoWithReferenceType<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId != null).Select(x => x.Value).ForEach(x =>
            {
                if (!treeDic.ContainsKey(x.ParentId))
                {
                    return;
                }

                var parent = treeDic[x.ParentId];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId));
        }

        public static IEnumerable<TTree> ToTreeDtoWithReferenceTypeOrderBy<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : class
            where TTree : class, IGeneralTreeDtoWithReferenceType<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId != null).Select(x => x.Value).ForEach(x =>
            {
                if (!treeDic.ContainsKey(x.ParentId))
                {
                    return;
                }

                var parent = treeDic[x.ParentId];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value).ForEach(x =>
            {
                x.Children = x.Children.OrderBy(propertySelector).ToList();
            });


            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderBy(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId));
        }

        public static IEnumerable<TTree> ToTreeDtoWithReferenceTypeOrderByDescending<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : class
            where TTree : class, IGeneralTreeDtoWithReferenceType<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            treeDic.Where(x => x.Value.ParentId != null).Select(x => x.Value).ForEach(x =>
            {
                if (!treeDic.ContainsKey(x.ParentId))
                {
                    return;
                }

                var parent = treeDic[x.ParentId];
                if (parent.Children == null)
                {
                    parent.Children = new List<TTree>();
                }

                parent.Children.Add(x);
            });

            treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value).ForEach(x =>
            {
                x.Children = x.Children.OrderByDescending(propertySelector).ToList();
            });


            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderByDescending(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId));
        }
    }
}