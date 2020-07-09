
using System;
using Abp.Application.Services.Dto;

namespace Ems.Vendors.Dtos
{
    public class VendorChargeDetailDto : EntityDto
    {
		public string InvoiceDetail { get; set; }

		public decimal Quantity { get; set; }

		public decimal UnitPrice { get; set; }

		public decimal Tax { get; set; }

		public decimal SubTotal { get; set; }



    }
}