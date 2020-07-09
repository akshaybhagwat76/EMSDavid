

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_IncidentStatuses)]
    public class IncidentStatusesAppService : EmsAppServiceBase, IIncidentStatusesAppService
    {
		 private readonly IRepository<IncidentStatus> _incidentStatusRepository;
		 private readonly IIncidentStatusesExcelExporter _incidentStatusesExcelExporter;
		 

		  public IncidentStatusesAppService(IRepository<IncidentStatus> incidentStatusRepository, IIncidentStatusesExcelExporter incidentStatusesExcelExporter ) 
		  {
			_incidentStatusRepository = incidentStatusRepository;
			_incidentStatusesExcelExporter = incidentStatusesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetIncidentStatusForViewDto>> GetAll(GetAllIncidentStatusesInput input)
         {
			
			var filteredIncidentStatuses = _incidentStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredIncidentStatuses = filteredIncidentStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var incidentStatuses = from o in pagedAndFilteredIncidentStatuses
                         select new GetIncidentStatusForViewDto() {
							IncidentStatus = new IncidentStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredIncidentStatuses.CountAsync();

            return new PagedResultDto<GetIncidentStatusForViewDto>(
                totalCount,
                await incidentStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetIncidentStatusForViewDto> GetIncidentStatusForView(int id)
         {
            var incidentStatus = await _incidentStatusRepository.GetAsync(id);

            var output = new GetIncidentStatusForViewDto { IncidentStatus = ObjectMapper.Map<IncidentStatusDto>(incidentStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentStatuses_Edit)]
		 public async Task<GetIncidentStatusForEditOutput> GetIncidentStatusForEdit(EntityDto input)
         {
            var incidentStatus = await _incidentStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetIncidentStatusForEditOutput {IncidentStatus = ObjectMapper.Map<CreateOrEditIncidentStatusDto>(incidentStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditIncidentStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditIncidentStatusDto input)
         {
            var incidentStatus = ObjectMapper.Map<IncidentStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				incidentStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _incidentStatusRepository.InsertAsync(incidentStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditIncidentStatusDto input)
         {
            var incidentStatus = await _incidentStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, incidentStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _incidentStatusRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetIncidentStatusesToExcel(GetAllIncidentStatusesForExcelInput input)
         {
			
			var filteredIncidentStatuses = _incidentStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var query = (from o in filteredIncidentStatuses
                         select new GetIncidentStatusForViewDto() { 
							IncidentStatus = new IncidentStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var incidentStatusListDtos = await query.ToListAsync();

            return _incidentStatusesExcelExporter.ExportToFile(incidentStatusListDtos);
         }


    }
}