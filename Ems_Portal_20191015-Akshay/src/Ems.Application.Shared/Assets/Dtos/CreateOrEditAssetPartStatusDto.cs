
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditAssetPartStatusDto : EntityDto<int?>
    {

		[Required]
		public string Status { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}