using System.Collections.Generic;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing.Exporting
{
    public interface IVendorChargeStatusesExcelExporter
    {
        FileDto ExportToFile(List<GetVendorChargeStatusForViewDto> vendorChargeStatuses);
    }
}