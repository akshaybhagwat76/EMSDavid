

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Billing.Exporting;
using Ems.Billing.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Billing
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_VendorChargeStatuses)]
    public class VendorChargeStatusesAppService : EmsAppServiceBase, IVendorChargeStatusesAppService
    {
		 private readonly IRepository<VendorChargeStatus> _vendorChargeStatusRepository;
		 private readonly IVendorChargeStatusesExcelExporter _vendorChargeStatusesExcelExporter;
		 

		  public VendorChargeStatusesAppService(IRepository<VendorChargeStatus> vendorChargeStatusRepository, IVendorChargeStatusesExcelExporter vendorChargeStatusesExcelExporter ) 
		  {
			_vendorChargeStatusRepository = vendorChargeStatusRepository;
			_vendorChargeStatusesExcelExporter = vendorChargeStatusesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetVendorChargeStatusForViewDto>> GetAll(GetAllVendorChargeStatusesInput input)
         {
			
			var filteredVendorChargeStatuses = _vendorChargeStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var pagedAndFilteredVendorChargeStatuses = filteredVendorChargeStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var vendorChargeStatuses = from o in pagedAndFilteredVendorChargeStatuses
                         select new GetVendorChargeStatusForViewDto() {
							VendorChargeStatus = new VendorChargeStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredVendorChargeStatuses.CountAsync();

            return new PagedResultDto<GetVendorChargeStatusForViewDto>(
                totalCount,
                await vendorChargeStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetVendorChargeStatusForViewDto> GetVendorChargeStatusForView(int id)
         {
            var vendorChargeStatus = await _vendorChargeStatusRepository.GetAsync(id);

            var output = new GetVendorChargeStatusForViewDto { VendorChargeStatus = ObjectMapper.Map<VendorChargeStatusDto>(vendorChargeStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_VendorChargeStatuses_Edit)]
		 public async Task<GetVendorChargeStatusForEditOutput> GetVendorChargeStatusForEdit(EntityDto input)
         {
            var vendorChargeStatus = await _vendorChargeStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetVendorChargeStatusForEditOutput {VendorChargeStatus = ObjectMapper.Map<CreateOrEditVendorChargeStatusDto>(vendorChargeStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditVendorChargeStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_VendorChargeStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditVendorChargeStatusDto input)
         {
            var vendorChargeStatus = ObjectMapper.Map<VendorChargeStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				vendorChargeStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _vendorChargeStatusRepository.InsertAsync(vendorChargeStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_VendorChargeStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditVendorChargeStatusDto input)
         {
            var vendorChargeStatus = await _vendorChargeStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, vendorChargeStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_VendorChargeStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _vendorChargeStatusRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetVendorChargeStatusesToExcel(GetAllVendorChargeStatusesForExcelInput input)
         {
			
			var filteredVendorChargeStatuses = _vendorChargeStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var query = (from o in filteredVendorChargeStatuses
                         select new GetVendorChargeStatusForViewDto() { 
							VendorChargeStatus = new VendorChargeStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var vendorChargeStatusListDtos = await query.ToListAsync();

            return _vendorChargeStatusesExcelExporter.ExportToFile(vendorChargeStatusListDtos);
         }


    }
}