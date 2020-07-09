using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetPartTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string TypeFilter { get; set; }

		public string DescriptionFilter { get; set; }



    }
}