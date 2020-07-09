
using System;
using Abp.Application.Services.Dto;

namespace Ems.Quotations.Dtos
{
    public class QuotationDto : EntityDto
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

        public bool IsFinal { get; set; }

        public string Remark { get; set; }

        public int? RequoteRefId { get; set; }

        public string QuotationLoc8GUID { get; set; }

        public string AcknowledgedBy { get; set; }

        public DateTime? AcknowledgedAt { get; set; }


        public int SupportContractId { get; set; }

        public int? QuotationStatusId { get; set; }

        public int? WorkOrderId { get; set; }

        public int? AssetId { get; set; }

        public int? AssetClassId { get; set; }

        public int? SupportTypeId { get; set; }

        public int? SupportItemId { get; set; }
    }
}