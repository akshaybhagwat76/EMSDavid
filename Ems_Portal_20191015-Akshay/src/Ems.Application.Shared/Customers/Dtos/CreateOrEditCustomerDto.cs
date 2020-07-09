
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class CreateOrEditCustomerDto : EntityDto<int?>
    {

		[Required]
		public string Reference { get; set; }
		
		
		[Required]
		public string Name { get; set; }
		
		
		[Required]
		public string Identifier { get; set; }
		
		
		public string LogoUrl { get; set; }
		
		
		public string Website { get; set; }
		
		
		public string CustomerLoc8UUID { get; set; }
		
		
		 public int CustomerTypeId { get; set; }
		 
		 		 public int? CurrencyId { get; set; }
		 
		 
    }
}