using System;
using System.Threading.Tasks;

namespace Abp.GeneralTree
{
    public interface ITreeManager<TTree, TPrimaryKey> : IGeneralTreeManager<TTree, TPrimaryKey>
        where TPrimaryKey : struct
        where TTree : TreeEntity<TTree, TPrimaryKey>
    {
        Task MoveOrderAsync(TPrimaryKey id, TPrimaryKey? parentId, int order, Action<TTree> childrenAction = null);
    }
}