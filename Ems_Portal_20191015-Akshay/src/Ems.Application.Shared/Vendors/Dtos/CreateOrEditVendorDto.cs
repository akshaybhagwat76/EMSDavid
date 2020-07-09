
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Vendors.Dtos
{
    public class CreateOrEditVendorDto : EntityDto<int?>
    {

		[Required]
		public string Reference { get; set; }
		
		
		[Required]
		public string Name { get; set; }
		
		
		[Required]
		public string Identifier { get; set; }
		
		
		public string LogoUrl { get; set; }
		
		
		public string Website { get; set; }
		
		
		public string VendorLoc8GUID { get; set; }
		
		
		 public int? SsicCodeId { get; set; }
		 
		 		 public int CurrencyId { get; set; }
		 
		 
    }
}