using Abp.Application.Services.Dto;
using System;

namespace Ems.Customers.Dtos
{
    public class GetAllCustomersForExcelInput
    {
		public string Filter { get; set; }

		public string ReferenceFilter { get; set; }

		public string NameFilter { get; set; }

		public string IdentifierFilter { get; set; }

		public string CustomerLoc8UUIDFilter { get; set; }


		 public string CustomerTypeTypeFilter { get; set; }

		 		 public string CurrencyCodeFilter { get; set; }

		 
    }
}