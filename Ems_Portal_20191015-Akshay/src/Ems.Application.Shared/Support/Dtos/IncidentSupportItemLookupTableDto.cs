using Abp.Application.Services.Dto;
using Ems.Customers;
using System.Collections.Generic;

namespace Ems.Support.Dtos
{
    public class IncidentSupportItemLookupTableDto
    {
		public int Id { get; set; }

		public string DisplayName { get; set; }
    }

    public class IncidentSupportItemAndCustomerListDto
    {
        public List<IncidentCustomerLookupTableDto> CustomerList { get; set; }

        public List<IncidentSupportItemLookupTableDto> SupportItemList { get; set; }
    }
}