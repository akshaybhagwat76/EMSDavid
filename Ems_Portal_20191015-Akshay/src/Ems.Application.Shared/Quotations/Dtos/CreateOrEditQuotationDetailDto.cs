
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Quotations.Dtos
{
    public class CreateOrEditQuotationDetailDto : EntityDto<int?>
    {

        [Required]
        public string Description { get; set; }


        public decimal Quantity { get; set; }


        public decimal UnitPrice { get; set; }


        public decimal? Cost { get; set; }


        public decimal Tax { get; set; }


        public decimal Charge { get; set; }


        public decimal Discount { get; set; }


        public decimal MarkUp { get; set; }


        public bool IsTaxable { get; set; }


        public bool IsChargeable { get; set; }


        public bool IsAdHoc { get; set; }


        public bool IsStandbyReplacementUnit { get; set; }


        public bool IsOptionalItem { get; set; }


        public string Remark { get; set; }


        public string Loc8GUID { get; set; }

        public int? ItemTypeId { get; set; }

        public int QuotationId { get; set; }

        public int? UomId { get; set; }
    }
}