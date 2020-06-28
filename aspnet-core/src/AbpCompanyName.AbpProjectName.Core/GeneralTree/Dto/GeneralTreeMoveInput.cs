using System.ComponentModel.DataAnnotations;

namespace Abp.GeneralTree.Dto
{
    public class GeneralTreeMoveInput
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public int? NewParentId { get; set; }

        public int Order { get; set; }
    }
}