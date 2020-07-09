using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface IVendorChargeStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetVendorChargeStatusForViewDto>> GetAll(GetAllVendorChargeStatusesInput input);

        Task<GetVendorChargeStatusForViewDto> GetVendorChargeStatusForView(int id);

		Task<GetVendorChargeStatusForEditOutput> GetVendorChargeStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditVendorChargeStatusDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetVendorChargeStatusesToExcel(GetAllVendorChargeStatusesForExcelInput input);

		
    }
}