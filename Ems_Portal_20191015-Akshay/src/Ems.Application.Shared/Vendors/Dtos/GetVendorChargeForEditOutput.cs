using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Vendors.Dtos
{
    public class GetVendorChargeForEditOutput
    {
		public CreateOrEditVendorChargeDto VendorCharge { get; set; }

		public string VendorName { get; set;}

		public string SupportContractTitle { get; set;}

		public string WorkOrderSubject { get; set;}

		public string VendorChargeStatusStatus { get; set;}


    }
}