
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetOwnershipDto : EntityDto
    {
		public DateTime? StartDate { get; set; }

		public DateTime? FinishDate { get; set; }

		public decimal? PercentageOwnership { get; set; }


		 public int? AssetId { get; set; }

		 		 public int? AssetOwnerId { get; set; }

		 
    }
}