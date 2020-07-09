using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditAssetNotesDto : EntityDto<int?>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Notes { get; set; }

        public long? UserId { get; set; } 

        public int AssetId { get; set; }

        public int? TenantId { get; set; }
    }
}
