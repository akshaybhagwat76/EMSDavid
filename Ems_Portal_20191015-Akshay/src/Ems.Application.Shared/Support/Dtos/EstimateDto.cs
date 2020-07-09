using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class EstimateDto : EntityDto
    {
        public string Reference { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? TotalTax { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal? TotalCharge { get; set; }

        public decimal? Version { get; set; }

        public string Remark { get; set; }

        public int? RequoteRefId { get; set; }

        public string QuotationLoc8GUID { get; set; }

        public int? AcknowledgedBy { get; set; }

        public DateTime? AcknowledgedAt { get; set; }

        public int? QuotationId { get; set; }

        public int? WorkOrderId { get; set; }

        public int EstimateStatusId { get; set; }

        public int CustomerId { get; set; }
    }
}