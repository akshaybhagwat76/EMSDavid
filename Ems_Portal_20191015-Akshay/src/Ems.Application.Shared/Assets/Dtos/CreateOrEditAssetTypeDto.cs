
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditAssetTypeDto : EntityDto<int?>
    {

		[Required]
		public string Code { get; set; }
		
		
		[Required]
		public string Type { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		
		public int Sort { get; set; }
		
		

    }
}