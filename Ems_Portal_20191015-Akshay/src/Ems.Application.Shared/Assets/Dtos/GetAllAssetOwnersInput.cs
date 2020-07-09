using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetOwnersInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string ReferenceFilter { get; set; }

		public string NameFilter { get; set; }

		public string IdentifierFilter { get; set; }

		public string WebsiteFilter { get; set; }


		 public string CurrencyNameFilter { get; set; }

		 		 public string SsicCodeCodeFilter { get; set; }

		 
    }
}