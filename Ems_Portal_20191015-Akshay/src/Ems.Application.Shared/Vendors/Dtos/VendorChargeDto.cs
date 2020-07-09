
using System;
using Abp.Application.Services.Dto;

namespace Ems.Vendors.Dtos
{
    public class VendorChargeDto : EntityDto
    {
		public string Reference { get; set; }

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