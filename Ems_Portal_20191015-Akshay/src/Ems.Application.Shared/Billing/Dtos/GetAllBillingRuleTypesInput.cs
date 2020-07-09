using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllBillingRuleTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}