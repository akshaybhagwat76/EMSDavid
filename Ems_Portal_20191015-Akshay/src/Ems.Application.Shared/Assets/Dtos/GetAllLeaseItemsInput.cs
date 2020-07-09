using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllLeaseItemsInput : PagedAndSortedResultRequestDto
    {
        public int LeaseAgreementId { get; set; }

        public string Filter { get; set; }

		public DateTime? MaxDateAllocatedFilter { get; set; }
		public DateTime? MinDateAllocatedFilter { get; set; }

		public decimal? MaxAllocationPercentageFilter { get; set; }
		public decimal? MinAllocationPercentageFilter { get; set; }

		public string TermsFilter { get; set; }

		public decimal? MaxUnitRentalRateFilter { get; set; }
		public decimal? MinUnitRentalRateFilter { get; set; }

		public decimal? MaxUnitDepositRateFilter { get; set; }
		public decimal? MinUnitDepositRateFilter { get; set; }

		public DateTime? MaxStartDateFilter { get; set; }
		public DateTime? MinStartDateFilter { get; set; }

		public DateTime? MaxEndDateFilter { get; set; }
		public DateTime? MinEndDateFilter { get; set; }

		//public string AttachmentsFilter { get; set; }

		public int? MaxRentalUomRefIdFilter { get; set; }
		public int? MinRentalUomRefIdFilter { get; set; }

		public decimal? MaxDepositUomRefIdFilter { get; set; }
		public decimal? MinDepositUomRefIdFilter { get; set; }

		public string ItemFilter { get; set; }

		public string DescriptionFilter { get; set; }


		 public string AssetClassClassFilter { get; set; }

		 		 public string AssetReferenceFilter { get; set; }

		 		 public string LeaseAgreementTitleFilter { get; set; }

		 
    }
}