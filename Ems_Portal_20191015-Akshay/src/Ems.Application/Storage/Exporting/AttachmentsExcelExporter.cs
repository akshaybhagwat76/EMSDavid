using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Storage.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Storage.Exporting
{
    public class AttachmentsExcelExporter : EpPlusExcelExporterBase, IAttachmentsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public AttachmentsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetAttachmentForViewDto> attachments)
        {
            return CreateExcelPackage(
                "Attachments.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Attachments"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Filename"),
                        L("Description"),
                        L("UploadedAt"),
                        L("UploadedBy"),
                        L("BlobFolder"),
                        L("BlobId"),
                        (L("Asset")) + L("Reference"),
                        (L("Incident")) + L("Description"),
                        (L("LeaseAgreement")) + L("Reference"),
                        (L("Quotation")) + L("Title"),
                        (L("SupportContract")) + L("Title"),
                        (L("WorkOrder")) + L("Subject"),
                        (L("CustomerInvoice")) + L("Description")
                        );

                    AddObjects(
                        sheet, 2, attachments,
                        _ => _.Attachment.Filename,
                        _ => _.Attachment.Description,
                        _ => _timeZoneConverter.Convert(_.Attachment.UploadedAt, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.Attachment.UploadedBy,
                        _ => _.Attachment.BlobFolder,
                        _ => _.Attachment.BlobId,
                        _ => _.AssetReference,
                        _ => _.IncidentDescription,
                        _ => _.LeaseAgreementReference,
                        _ => _.QuotationTitle,
                        _ => _.SupportContractTitle,
                        _ => _.WorkOrderSubject,
                        _ => _.CustomerInvoiceDescription
                        );

					var uploadedAtColumn = sheet.Column(3);
                    uploadedAtColumn.Style.Numberformat.Format = "yyyy-mm-dd";
					uploadedAtColumn.AutoFit();
					

                });
        }
    }
}
