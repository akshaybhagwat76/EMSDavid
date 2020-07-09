
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Vendors.Dtos
{
    public class CreateOrEditVendorChargeDetailDto : EntityDto<int?>
    {

		[Required]
		public string InvoiceDetail { get; set; }
		
		
		public decimal Quantity { get; set; }
		
		
		public decimal UnitPrice { get; set; }
		
		
		public decimal Tax { get; set; }
		
		
		public decimal SubTotal { get; set; }
		
		

    }
}