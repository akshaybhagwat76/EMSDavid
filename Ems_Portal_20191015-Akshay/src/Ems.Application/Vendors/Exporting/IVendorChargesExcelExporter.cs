using System.Collections.Generic;
using Ems.Vendors.Dtos;
using Ems.Dto;

namespace Ems.Vendors.Exporting
{
    public interface IVendorChargesExcelExporter
    {
        FileDto ExportToFile(List<GetVendorChargeForViewDto> vendorCharges);
    }
}