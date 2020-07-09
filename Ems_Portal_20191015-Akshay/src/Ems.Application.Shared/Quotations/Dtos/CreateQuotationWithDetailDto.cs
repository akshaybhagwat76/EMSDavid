
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Quotations.Dtos
{
    public class CreateQuotationWithDetailDto : EntityDto<int?>
    {

        // Quotation

        [Required]
        public string Reference { get; set; }


        [Required]
        public string Title { get; set; }


        public string QuotationDescription { get; set; }


        public DateTime? StartDate { get; set; }


        public DateTime? EndDate { get; set; }


        public decimal? TotalTax { get; set; }


        public decimal? TotalPrice { get; set; }


        public decimal? TotalDiscount { get; set; }


        public decimal? TotalCharge { get; set; }


        public decimal? Version { get; set; }


        public bool IsFinal { get; set; }


        public string QuotationRemark { get; set; }


        public int? RequoteRefId { get; set; }


        public string QuotationLoc8GUID { get; set; }


        public string QuotationAttachments { get; set; }


        public string AcknowledgedBy { get; set; }


        public DateTime? AcknowledgedAt { get; set; }


        public int SupportContractId { get; set; }

        public int? QuotationStatusId { get; set; }

        public int RfqId { get; set; }

        // Quotation Detail

        [Required]
        public string DetailDescription { get; set; }


        public decimal Quantity { get; set; }


        public decimal UnitPrice { get; set; }


        public decimal? Cost { get; set; }


        public decimal Tax { get; set; }


        public decimal Charge { get; set; }


        public decimal Discount { get; set; }


        public decimal MarkUp { get; set; }


        public bool IsChargeable { get; set; }

        public bool IsTaxable { get; set; }


        public bool IsAdHoc { get; set; }


        public bool IsStandbyReplacementUnit { get; set; }


        public bool IsOptionalItem { get; set; }


        public string DetailRemark { get; set; }


        public string DetailLoc8GUID { get; set; }


        public string DetailAttachments { get; set; }


        public int? AssetId { get; set; }

        public int? AssetClassId { get; set; }

        public int? ItemTypeId { get; set; }

        public int? SupportTypeId { get; set; }

        public int QuotationId { get; set; }

        public int? UomId { get; set; }

        public int? SupportItemId { get; set; }

        public int? WorkOrderId { get; set; }
    }
}