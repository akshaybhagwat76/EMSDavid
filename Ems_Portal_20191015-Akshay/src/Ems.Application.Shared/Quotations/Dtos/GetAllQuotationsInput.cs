using Abp.Application.Services.Dto;
using System;

namespace Ems.Quotations.Dtos
{
    public class GetAllQuotationsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ReferenceFilter { get; set; }

        public string TitleFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public DateTime? MaxStartDateFilter { get; set; }
        public DateTime? MinStartDateFilter { get; set; }

        public DateTime? MaxEndDateFilter { get; set; }
        public DateTime? MinEndDateFilter { get; set; }

        public int IsFinalFilter { get; set; }

        public string RemarkFilter { get; set; }

        public int? MaxRequoteRefIdFilter { get; set; }
        public int? MinRequoteRefIdFilter { get; set; }

        public string QuotationLoc8GUIDFilter { get; set; }

        public string AcknowledgedByFilter { get; set; }

        public DateTime? MaxAcknowledgedAtFilter { get; set; }
        public DateTime? MinAcknowledgedAtFilter { get; set; }


        public string SupportContractTitleFilter { get; set; }

        public string QuotationStatusStatusFilter { get; set; }

        public string WorkOrderSubjectFilter { get; set; }

        public string AssetReferenceFilter { get; set; }

        public string AssetClassClassFilter { get; set; }

        public string SupportTypeTypeFilter { get; set; }

        public string SupportItemDescriptionFilter { get; set; }
    }
}