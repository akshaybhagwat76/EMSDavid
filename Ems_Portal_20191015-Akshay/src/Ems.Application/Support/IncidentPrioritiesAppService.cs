

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_IncidentPriorities)]
    public class IncidentPrioritiesAppService : EmsAppServiceBase, IIncidentPrioritiesAppService
    {
		 private readonly IRepository<IncidentPriority> _incidentPriorityRepository;
		 private readonly IIncidentPrioritiesExcelExporter _incidentPrioritiesExcelExporter;
		 

		  public IncidentPrioritiesAppService(IRepository<IncidentPriority> incidentPriorityRepository, IIncidentPrioritiesExcelExporter incidentPrioritiesExcelExporter ) 
		  {
			_incidentPriorityRepository = incidentPriorityRepository;
			_incidentPrioritiesExcelExporter = incidentPrioritiesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetIncidentPriorityForViewDto>> GetAll(GetAllIncidentPrioritiesInput input)
         {
			
			var filteredIncidentPriorities = _incidentPriorityRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Priority.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredIncidentPriorities = filteredIncidentPriorities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var incidentPriorities = from o in pagedAndFilteredIncidentPriorities
                         select new GetIncidentPriorityForViewDto() {
							IncidentPriority = new IncidentPriorityDto
							{
                                Priority = o.Priority,
                                Description = o.Description,
                                PriorityLevel = o.PriorityLevel,
                                Id = o.Id
							}
						};

            var totalCount = await filteredIncidentPriorities.CountAsync();

            return new PagedResultDto<GetIncidentPriorityForViewDto>(
                totalCount,
                await incidentPriorities.ToListAsync()
            );
         }
		 
		 public async Task<GetIncidentPriorityForViewDto> GetIncidentPriorityForView(int id)
         {
            var incidentPriority = await _incidentPriorityRepository.GetAsync(id);

            var output = new GetIncidentPriorityForViewDto { IncidentPriority = ObjectMapper.Map<IncidentPriorityDto>(incidentPriority) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentPriorities_Edit)]
		 public async Task<GetIncidentPriorityForEditOutput> GetIncidentPriorityForEdit(EntityDto input)
         {
            var incidentPriority = await _incidentPriorityRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetIncidentPriorityForEditOutput {IncidentPriority = ObjectMapper.Map<CreateOrEditIncidentPriorityDto>(incidentPriority)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditIncidentPriorityDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentPriorities_Create)]
		 protected virtual async Task Create(CreateOrEditIncidentPriorityDto input)
         {
            var incidentPriority = ObjectMapper.Map<IncidentPriority>(input);

			
			if (AbpSession.TenantId != null)
			{
				incidentPriority.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _incidentPriorityRepository.InsertAsync(incidentPriority);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentPriorities_Edit)]
		 protected virtual async Task Update(CreateOrEditIncidentPriorityDto input)
         {
            var incidentPriority = await _incidentPriorityRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, incidentPriority);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentPriorities_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _incidentPriorityRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetIncidentPrioritiesToExcel(GetAllIncidentPrioritiesForExcelInput input)
         {
			
			var filteredIncidentPriorities = _incidentPriorityRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Priority.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var query = (from o in filteredIncidentPriorities
                         select new GetIncidentPriorityForViewDto() { 
							IncidentPriority = new IncidentPriorityDto
							{
                                Priority = o.Priority,
                                Description = o.Description,
                                PriorityLevel = o.PriorityLevel,
                                Id = o.Id
							}
						 });


            var incidentPriorityListDtos = await query.ToListAsync();

            return _incidentPrioritiesExcelExporter.ExportToFile(incidentPriorityListDtos);
         }


    }
}