using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Vendors.Dtos;
using Ems.Dto;

namespace Ems.Vendors
{
    public interface IVendorChargeDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetVendorChargeDetailForViewDto>> GetAll(GetAllVendorChargeDetailsInput input);

        Task<GetVendorChargeDetailForViewDto> GetVendorChargeDetailForView(int id);

		Task<GetVendorChargeDetailForEditOutput> GetVendorChargeDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditVendorChargeDetailDto input);

		Task Delete(EntityDto input);

		
    }
}