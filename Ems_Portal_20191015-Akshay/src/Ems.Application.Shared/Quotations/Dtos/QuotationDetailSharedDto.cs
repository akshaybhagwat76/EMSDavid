using Ems.Assets.Dtos;
using Ems.Organizations.Dtos;
using Ems.Vendors.Dtos;
using System.Collections.Generic;

namespace Ems.Quotations.Dtos
{
    public class QuotationAssetAndSupportItemListDto
    {
        public List<QuotationAssetLookupTableDto> AssetList { get; set; }

        public List<QuotationAssetClassLookupTableDto> AssetClassList { get; set; }

        public List<QuotationSupportContractLookupTableDto> SupportContractList { get; set; }

        public List<QuotationSupportItemLookupTableDto> SupportItemList { get; set; }

        public List<QuotationSupportTypeLookupTableDto> SupportTypeList { get; set; }

        public AddressDto AO_Address { get; set; }
    }

    public class QuotationPdfDto
    {
        public AssetOwnerDto AssetOwnerInfo { get; set; }

        public AddressDto AssetOwnerAddress { get; set; }

        public ContactDto AssetOwnerContact { get; set; }

        public QuotationDto QuotationInfo { get; set; }

        public PdfQuotationDetailListDto QuotationDetailList { get; set; }

        public VendorDto VendorInfo { get; set; }

        public AddressDto VendorAddress { get; set; }

        public ContactDto VendorContact { get; set; }

        public string AuthenticationKey { get; set; }
    }

    public class PdfQuotationDetailDto
    {
        public int Id { get; set; }

        public int? QuotationId { get; set; }

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

    public class PdfQuotationDetailForViewDto
    {
        public PdfQuotationDetailDto QuotationDetail { get; set; }

        public string ItemTypeType { get; set; }

        public string UomUnitOfMeasurement { get; set; }
    }

    public class PdfQuotationDetailListDto
    {
        public List<PdfQuotationDetailForViewDto> DetailList { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
