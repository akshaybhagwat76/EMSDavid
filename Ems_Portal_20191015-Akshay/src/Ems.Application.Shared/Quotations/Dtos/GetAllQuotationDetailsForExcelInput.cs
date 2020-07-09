using Abp.Application.Services.Dto;
using System;

namespace Ems.Quotations.Dtos
{
    public class GetAllQuotationDetailsForExcelInput
    {
		public string Filter { get; set; }

		public string DescriptionFilter { get; set; }

		public string Loc8GUIDFilter { get; set; }


		 public string AssetReferenceFilter { get; set; }

		 		 public string AssetClassClassFilter { get; set; }

		 		 public string ItemTypeTypeFilter { get; set; }

		 		 public string SupportTypeTypeFilter { get; set; }

		 		 public string QuotationTitleFilter { get; set; }

		 		 public string UomUnitOfMeasurementFilter { get; set; }

		 		 public string SupportItemDescriptionFilter { get; set; }

		 		 public string WorkOrderSubjectFilter { get; set; }

		 
    }
}