using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllLeaseAgreementsForExcelInput
    {
		public string Filter { get; set; }

		public string ReferenceFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public string TitleFilter { get; set; }

		public string TermsFilter { get; set; }

		public DateTime? MaxStartDateFilter { get; set; }
		public DateTime? MinStartDateFilter { get; set; }

		public DateTime? MaxEndDateFilter { get; set; }
		public DateTime? MinEndDateFilter { get; set; }


		 public string ContactContactNameFilter { get; set; }

		 		 public string AssetOwnerNameFilter { get; set; }

		 		 public string CustomerNameFilter { get; set; }

		 
    }
}