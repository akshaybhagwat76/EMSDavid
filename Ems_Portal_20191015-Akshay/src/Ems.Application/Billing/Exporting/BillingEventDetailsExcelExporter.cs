using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class BillingEventDetailsExcelExporter : EpPlusExcelExporterBase, IBillingEventDetailsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public BillingEventDetailsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetBillingEventDetailForViewDto> billingEventDetails)
        {
            return CreateExcelPackage(
                "BillingEventDetails.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("BillingEventDetails"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("RuleExecutedSuccessfully"),
                        L("Exception"),
                        (L("BillingRule")) + L("Name"),
                        (L("LeaseItem")) + L("Item"),
                        (L("BillingEvent")) + L("Purpose")
                        );

                    AddObjects(
                        sheet, 2, billingEventDetails,
                        _ => _.BillingEventDetail.RuleExecutedSuccessfully,
                        _ => _.BillingEventDetail.Exception,
                        _ => _.BillingRuleName,
                        _ => _.LeaseItemItem,
                        _ => _.BillingEventPurpose
                        );

					

                });
        }
    }
}
