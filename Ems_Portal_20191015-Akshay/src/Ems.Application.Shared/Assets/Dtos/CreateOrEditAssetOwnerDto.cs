
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class CreateOrEditAssetOwnerDto : EntityDto<int?>
    {

		[Required]
		public string Reference { get; set; }
		
		
		[Required]
		public string Name { get; set; }
		
		
		public string Identifier { get; set; }
		
		
		public string LogoUrl { get; set; }
		
		
		public string Website { get; set; }
		
		
		 public int? CurrencyId { get; set; }
		 
		 		 public int? SsicCodeId { get; set; }
		 
		 
    }
}