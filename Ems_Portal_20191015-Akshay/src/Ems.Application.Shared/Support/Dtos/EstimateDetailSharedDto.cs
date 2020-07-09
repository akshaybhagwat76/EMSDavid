using Ems.Assets.Dtos;
using Ems.Customers.Dtos;
using Ems.Organizations.Dtos;
using System.Collections.Generic;

namespace Ems.Support.Dtos
{
    public class EstimatePdfDto
    {
        public AssetOwnerDto AssetOwnerInfo { get; set; }

        public AddressDto AssetOwnerAddress { get; set; }

        public ContactDto AssetOwnerContact { get; set; }

        public EstimateDto EstimateInfo { get; set; }

        public PdfEstimateDetailListDto EstimateDetailList { get; set; }

        public CustomerDto CustomerInfo { get; set; }

        public AddressDto CustomerAddress { get; set; }

        public ContactDto CustomerContact { get; set; }

        public string AuthenticationKey { get; set; }
    }

    public class PdfEstimateDetailDto
    {
        public int Id { get; set; }

        public int EstimateId { get; set; }

        public int? ItemTypeId { get; set; }

        public int? UomId { get; set; }

        public int? WorkOrderActionId { get; set; }

        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal CalculatedUnitPrice { get; set; }

        public decimal CalculatedAmount { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? Cost { get; set; }

        public decimal Tax { get; set; }

        public decimal Charge { get; set; }

        public decimal Discount { get; set; }

        public decimal MarkUp { get; set; }

        public string Remark { get; set; }
    }

    public class PdfEstimateDetailForViewDto
    {
        public PdfEstimateDetailDto EstimateDetail { get; set; }

        public string ItemTypeType { get; set; }

        public string UomUnitOfMeasurement { get; set; }

        public string ActionWorkOrderAction { get; set; }
    }

    public class PdfEstimateDetailListDto
    {
        public List<PdfEstimateDetailForViewDto> DetailList { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
