

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Exporting;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_EstimateStatuses)]
    public class EstimateStatusesAppService : EmsAppServiceBase, IEstimateStatusesAppService
    {
		 private readonly IRepository<EstimateStatus> _estimateStatusRepository;
		 private readonly IEstimateStatusesExcelExporter _estimateStatusesExcelExporter;
		 

		  public EstimateStatusesAppService(IRepository<EstimateStatus> estimateStatusRepository, IEstimateStatusesExcelExporter estimateStatusesExcelExporter ) 
		  {
			_estimateStatusRepository = estimateStatusRepository;
			_estimateStatusesExcelExporter = estimateStatusesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetEstimateStatusForViewDto>> GetAll(GetAllEstimateStatusesInput input)
         {
			
			var filteredEstimateStatuses = _estimateStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var pagedAndFilteredEstimateStatuses = filteredEstimateStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var estimateStatuses = from o in pagedAndFilteredEstimateStatuses
                         select new GetEstimateStatusForViewDto() {
							EstimateStatus = new EstimateStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredEstimateStatuses.CountAsync();

            return new PagedResultDto<GetEstimateStatusForViewDto>(
                totalCount,
                await estimateStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetEstimateStatusForViewDto> GetEstimateStatusForView(int id)
         {
            var estimateStatus = await _estimateStatusRepository.GetAsync(id);

            var output = new GetEstimateStatusForViewDto { EstimateStatus = ObjectMapper.Map<EstimateStatusDto>(estimateStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_EstimateStatuses_Edit)]
		 public async Task<GetEstimateStatusForEditOutput> GetEstimateStatusForEdit(EntityDto input)
         {
            var estimateStatus = await _estimateStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetEstimateStatusForEditOutput {EstimateStatus = ObjectMapper.Map<CreateOrEditEstimateStatusDto>(estimateStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditEstimateStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_EstimateStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditEstimateStatusDto input)
         {
            var estimateStatus = ObjectMapper.Map<EstimateStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				estimateStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _estimateStatusRepository.InsertAsync(estimateStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_EstimateStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditEstimateStatusDto input)
         {
            var estimateStatus = await _estimateStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, estimateStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_EstimateStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _estimateStatusRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetEstimateStatusesToExcel(GetAllEstimateStatusesForExcelInput input)
         {
			
			var filteredEstimateStatuses = _estimateStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var query = (from o in filteredEstimateStatuses
                         select new GetEstimateStatusForViewDto() { 
							EstimateStatus = new EstimateStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var estimateStatusListDtos = await query.ToListAsync();

            return _estimateStatusesExcelExporter.ExportToFile(estimateStatusListDtos);
         }


    }
}