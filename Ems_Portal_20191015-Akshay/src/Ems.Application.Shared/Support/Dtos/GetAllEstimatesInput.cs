using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllEstimatesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ReferenceFilter { get; set; }

        public string TitleFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public DateTime? MaxStartDateFilter { get; set; }
        public DateTime? MinStartDateFilter { get; set; }

        public DateTime? MaxEndDateFilter { get; set; }
        public DateTime? MinEndDateFilter { get; set; }

        public decimal? MaxTotalTaxFilter { get; set; }
        public decimal? MinTotalTaxFilter { get; set; }

        public decimal? MaxTotalPriceFilter { get; set; }
        public decimal? MinTotalPriceFilter { get; set; }

        public decimal? MaxTotalDiscountFilter { get; set; }
        public decimal? MinTotalDiscountFilter { get; set; }

        public decimal? MaxTotalChargeFilter { get; set; }
        public decimal? MinTotalChargeFilter { get; set; }

        public decimal? MaxVersionFilter { get; set; }
        public decimal? MinVersionFilter { get; set; }

        public string RemarkFilter { get; set; }

        public int? MaxRequoteRefIdFilter { get; set; }
        public int? MinRequoteRefIdFilter { get; set; }

        public string QuotationLoc8GUIDFilter { get; set; }

        public int? MaxAcknowledgedByFilter { get; set; }
        public int? MinAcknowledgedByFilter { get; set; }

        public DateTime? MaxAcknowledgedAtFilter { get; set; }
        public DateTime? MinAcknowledgedAtFilter { get; set; }


        public string CustomerNameFilter { get; set; }

        public string WorkOrderSubjectFilter { get; set; }

        public string QuotationTitleFilter { get; set; }

        public string EstimateStatusStatusFilter { get; set; }


    }
}