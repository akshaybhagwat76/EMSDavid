using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetNotesDto : EntityDto
    {
		public string Title { get; set; }

		public string Notes { get; set; }

        public int? AssetId { get; set; }

		public long? UserId { get; set; }

		public int? TenantId { get; set; }
    }
}
