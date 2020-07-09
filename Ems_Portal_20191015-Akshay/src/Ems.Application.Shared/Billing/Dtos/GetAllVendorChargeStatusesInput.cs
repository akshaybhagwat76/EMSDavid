using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllVendorChargeStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string StatusFilter { get; set; }

		public string DescriptionFilter { get; set; }



    }
}