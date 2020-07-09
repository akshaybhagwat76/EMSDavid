using System.Collections.Generic;

namespace Ems.Support.Dtos
{
    public class WorkOrderAssetFkListDto
    {
        public List<WorkOrderCustomerLookupTableDto> CustomerList { get; set; }

        public List<WorkOrderSupportItemLookupTableDto> SupportItemList { get; set; }

        public List<WorkOrderAssetOwnershipLookupTableDto> AssetOwnerList { get; set; }

        public List<WorkOrderVendorLookupTableDto> VendorList { get; set; }
    }
}
