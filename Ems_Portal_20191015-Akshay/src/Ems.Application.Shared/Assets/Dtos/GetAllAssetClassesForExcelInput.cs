using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetClassesForExcelInput
    {
		public string Filter { get; set; }

		public string ClassFilter { get; set; }


		 public string AssetTypeTypeFilter { get; set; }

		 
    }
}