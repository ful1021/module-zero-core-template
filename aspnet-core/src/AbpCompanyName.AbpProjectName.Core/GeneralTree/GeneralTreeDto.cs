using System.Collections.Generic;

namespace Abp.GeneralTree
{
    public class GeneralTreeDto<TTree, TPrimaryKey> : IGeneralTreeDto<TTree, TPrimaryKey> where TPrimaryKey : struct
    {
        public TPrimaryKey Id { get; set; }

        public string Code { get; set; }

        public TPrimaryKey? ParentId { get; set; }

        public ICollection<TTree> Children { get; set; }
    }
}