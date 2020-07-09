
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Vendors.Dtos
{
    public class CreateOrEditVendorChargeDto : EntityDto<int?>
    {

		[Required]
		public string Reference { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		
		public DateTime? DateIssued { get; set; }
		
		
		public DateTime? DateDue { get; set; }
		
		
		public decimal? TotalTax { get; set; }
		
		
		public decimal? TotalPrice { get; set; }
		
		
		 public int? VendorId { get; set; }
		 
		 		 public int? SupportContractId { get; set; }
		 
		 		 public int? WorkOrderId { get; set; }
		 
		 		 public int? VendorChargeStatusId { get; set; }
		 
		 
    }
}