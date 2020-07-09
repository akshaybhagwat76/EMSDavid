
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditAssetClassDto : EntityDto<int?>
    {

		[Required]
		public string Manufacturer { get; set; }
		
		
		public string Model { get; set; }
		
		
		public string Specification { get; set; }
		
		
		[Required]
		public string Class { get; set; }
		
		
		 public int AssetTypeId { get; set; }
		 
		 
    }
}