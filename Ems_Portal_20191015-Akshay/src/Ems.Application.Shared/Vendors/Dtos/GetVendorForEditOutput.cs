using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Vendors.Dtos
{
    public class GetVendorForEditOutput
    {
		public CreateOrEditVendorDto Vendor { get; set; }

		public string SsicCodeCode { get; set;}

		public string CurrencyCode { get; set;}


    }
}