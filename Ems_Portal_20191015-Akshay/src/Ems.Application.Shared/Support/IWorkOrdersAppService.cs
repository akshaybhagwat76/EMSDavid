using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Web.Mvc.Models;
using System.Collections.Generic;

namespace Ems.Support
{
    public interface IWorkOrdersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWorkOrderForViewDto>> GetAll(GetAllWorkOrdersInput input);

        List<GetWorkorderItemsForViewDto> GetWorkorderItems(GetWorkorderItemsInput input);

        Task<GetWorkOrderForViewDto> GetWorkOrderForView(int id);

		Task<GetWorkOrderForEditOutput> GetWorkOrderForEdit(EntityDto input);

		Task<int> CreateOrEdit(CreateOrEditWorkOrderDto input);

        Task<ErrorViewModel> SetWorkOrderStatusComplete(CreateOrEditWorkOrderDto input);

        Task Delete(EntityDto input);

		Task<FileDto> GetWorkOrdersToExcel(GetAllWorkOrdersForExcelInput input);

		
		Task<PagedResultDto<WorkOrderWorkOrderPriorityLookupTableDto>> GetAllWorkOrderPriorityForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderWorkOrderTypeLookupTableDto>> GetAllWorkOrderTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderSupportItemLookupTableDto>> GetAllSupportItemForLookupTable(GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderAssetOwnershipLookupTableDto>> GetAllAssetOwnershipForLookupTable(GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<WorkOrderWorkOrderStatusLookupTableDto>> GetAllWorkOrderStatusForLookupTable(GetAllForLookupTableInput input);

        Task<WorkOrderAssetFkListDto> GetAssetFkList(int incidentId, int assetOwnerShipId);

        Task<WorkOrderWorkOrderPriorityLookupTableDto> GetWorkOrderPriorityByType(int workOrderTypeId);

        Task<WorkOrderUserLookupTableDto> GetDefaultCreator();

        Task<PagedResultDto<Organizations.Dtos.LocationLookupTableDto>> GetAllLocationForLookupTable(GetAllForLookupTableInput input);
    }
}