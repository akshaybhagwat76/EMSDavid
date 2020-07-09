using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Ems.DataExporting.Excel.EpPlus;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Ems.Storage;

namespace Ems.Organizations.Exporting
{
    public class ContactsExcelExporter : EpPlusExcelExporterBase, IContactsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ContactsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetContactForViewDto> contacts)
        {
            return CreateExcelPackage(
                "Contacts.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Contacts"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("HeadOfficeContact"),
                        L("ContactName"),
                        L("PhoneOffice"),
                        L("PhoneMobile"),
                        L("Fax"),
                        L("EmailAddress"),
                        L("Position"),
                        L("Department"),
                        L("ContactLoc8GUID"),
                        (L("User")) + L("Name"),
                        (L("Vendor")) + L("Name"),
                        (L("AssetOwner")) + L("Name"),
                        (L("Customer")) + L("Name")
                        );

                    AddObjects(
                        sheet, 2, contacts,
                        _ => _.Contact.HeadOfficeContact,
                        _ => _.Contact.ContactName,
                        _ => _.Contact.PhoneOffice,
                        _ => _.Contact.PhoneMobile,
                        _ => _.Contact.Fax,
                        _ => _.Contact.EmailAddress,
                        _ => _.Contact.Position,
                        _ => _.Contact.Department,
                        _ => _.Contact.ContactLoc8GUID,
                        _ => _.UserName,
                        _ => _.VendorName,
                        _ => _.AssetOwnerName,
                        _ => _.CustomerName
                        );

					

                });
        }
    }
}
