using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllSupportItemsForExcelInput
    {
		public string Filter { get; set; }

		public string DescriptionFilter { get; set; }

		public decimal? MaxUnitPriceFilter { get; set; }
		public decimal? MinUnitPriceFilter { get; set; }

		public decimal? MaxFrequencyFilter { get; set; }
		public decimal? MinFrequencyFilter { get; set; }

		public int IsAdHocFilter { get; set; }

		public int IsChargeableFilter { get; set; }

		public int IsStandbyReplacementUnitFilter { get; set; }


		 public string AssetReferenceFilter { get; set; }

		 		 public string AssetClassClassFilter { get; set; }

		 		 public string UomUnitOfMeasurementFilter { get; set; }

		 		 public string SupportContractTitleFilter { get; set; }

		 		 public string ConsumableTypeTypeFilter { get; set; }

		 		 public string SupportTypeTypeFilter { get; set; }

		 
    }
}