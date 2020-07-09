using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Billing.Exporting
{
    public class BillingRulesExcelExporter : EpPlusExcelExporterBase, IBillingRulesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public BillingRulesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetBillingRuleForViewDto> billingRules)
        {
            return CreateExcelPackage(
                "BillingRules.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("BillingRules"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("IsParent"),
                        L("ParentBillingRuleRefId"),
                        L("ChargePerUnit"),
                        L("DefaultInvoiceDescription"),
                        (L("BillingRuleType")) + L("Type"),
                        (L("UsageMetric")) + L("Metric"),
                        (L("LeaseAgreement")) + L("Title"),
                        (L("Vendor")) + L("Name"),
                        (L("LeaseItem")) + L("Item"),
                        (L("Currency")) + L("Code")
                        );

                    AddObjects(
                        sheet, 2, billingRules,
                        _ => _.BillingRule.Name,
                        _ => _.BillingRule.IsParent,
                        _ => _.BillingRule.ParentBillingRuleRefId,
                        _ => _.BillingRule.ChargePerUnit,
                        _ => _.BillingRule.DefaultInvoiceDescription,
                        _ => _.BillingRuleTypeType,
                        _ => _.UsageMetricMetric,
                        _ => _.LeaseAgreementTitle,
                        _ => _.VendorName,
                        _ => _.LeaseItemItem,
                        _ => _.CurrencyCode
                        );

					

                });
        }
    }
}
