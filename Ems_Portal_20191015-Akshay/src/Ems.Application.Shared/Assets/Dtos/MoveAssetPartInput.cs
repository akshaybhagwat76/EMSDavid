using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dto
{
    public class MoveAssetPartInput
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        public int? NewParentId { get; set; }
    }
}