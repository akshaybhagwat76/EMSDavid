using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Telematics.Dtos
{
    public class CreateOrEditUsageMetricDto : EntityDto<int?>
    {
        [Required]
        public string Metric { get; set; }

        public string Description { get; set; }

        public int? LeaseItemId { get; set; }

        public int AssetId { get; set; }

        public int? UomId { get; set; }
    }
}