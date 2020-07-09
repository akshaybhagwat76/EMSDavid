
using Abp.Application.Services.Dto;

namespace Ems.Quotations.Dtos
{
    public class QuotationDetailDto : EntityDto
    {
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? Cost { get; set; }

        public decimal Tax { get; set; }

        public decimal Charge { get; set; }

        public decimal Discount { get; set; }

        public decimal MarkUp { get; set; }

        public bool IsChargeable { get; set; }

        public bool IsAdHoc { get; set; }

        public bool IsStandbyReplacementUnit { get; set; }

        public bool IsOptionalItem { get; set; }

        public string Remark { get; set; }

        public string Loc8GUID { get; set; }

        public int? ItemTypeId { get; set; }

        public int QuotationId { get; set; }

        public int? UomId { get; set; }


        //public int? EstimateId { get; set; }
        //public int? AssetId { get; set; }
        //public int? AssetClassId { get; set; }
        //public int? SupportItemId { get; set; }
        //public int? SupportTypeId { get; set; }
        //public int? WorkOrderId { get; set; }
    }
}