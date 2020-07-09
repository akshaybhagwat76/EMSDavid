using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Quotations.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Quotations.Exporting
{
    public class QuotationDetailsExcelExporter : EpPlusExcelExporterBase, IQuotationDetailsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public QuotationDetailsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetQuotationDetailForViewDto> quotationDetails)
        {
            return CreateExcelPackage(
                "QuotationDetails.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("QuotationDetails"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Description"),
                        L("Quantity"),
                        L("UnitPrice"),
                        L("Cost"),
                        L("Tax"),
                        L("Charge"),
                        L("Discount"),
                        L("MarkUp"),
                        L("IsChargeable"),
                        L("IsAdHoc"),
                        L("IsStandbyReplacementUnit"),
                        L("IsOptionalItem"),
                        L("Remark"),
                        L("Loc8GUID"),
                        L("Attachments"),
                        //(L("Asset")) + L("Reference"),
                        //(L("AssetClass")) + L("Class"),
                        (L("ItemType")) + L("Type"),
                        //(L("SupportType")) + L("Type"),
                        (L("Quotation")) + L("Title"),
                        (L("Uom")) + L("UnitOfMeasurement")
                        //(L("SupportItem")) + L("Description"),
                        //(L("WorkOrder")) + L("Subject")
                        );

                    AddObjects(
                        sheet, 2, quotationDetails,
                        _ => _.QuotationDetail.Description,
                        _ => _.QuotationDetail.Quantity,
                        _ => _.QuotationDetail.UnitPrice,
                        _ => _.QuotationDetail.Cost,
                        _ => _.QuotationDetail.Tax,
                        _ => _.QuotationDetail.Charge,
                        _ => _.QuotationDetail.Discount,
                        _ => _.QuotationDetail.MarkUp,
                        _ => _.QuotationDetail.IsChargeable,
                        _ => _.QuotationDetail.IsAdHoc,
                        _ => _.QuotationDetail.IsStandbyReplacementUnit,
                        _ => _.QuotationDetail.IsOptionalItem,
                        _ => _.QuotationDetail.Remark,
                        _ => _.QuotationDetail.Loc8GUID,
                        //_ => _.AssetReference,
                        //_ => _.AssetClassClass,
                        _ => _.ItemTypeType,
                        //_ => _.SupportTypeType,
                        _ => _.QuotationTitle,
                        _ => _.UomUnitOfMeasurement
                        //_ => _.SupportItemDescription,
                        //_ => _.WorkOrderSubject
                        );

					

                });
        }
    }
}
