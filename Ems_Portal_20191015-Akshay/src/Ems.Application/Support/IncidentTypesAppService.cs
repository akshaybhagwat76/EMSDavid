

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_IncidentTypes)]
    public class IncidentTypesAppService : EmsAppServiceBase, IIncidentTypesAppService
    {
		 private readonly IRepository<IncidentType> _incidentTypeRepository;
		 private readonly IIncidentTypesExcelExporter _incidentTypesExcelExporter;
		 

		  public IncidentTypesAppService(IRepository<IncidentType> incidentTypeRepository, IIncidentTypesExcelExporter incidentTypesExcelExporter ) 
		  {
			_incidentTypeRepository = incidentTypeRepository;
			_incidentTypesExcelExporter = incidentTypesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetIncidentTypeForViewDto>> GetAll(GetAllIncidentTypesInput input)
         {
			
			var filteredIncidentTypes = _incidentTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredIncidentTypes = filteredIncidentTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var incidentTypes = from o in pagedAndFilteredIncidentTypes
                         select new GetIncidentTypeForViewDto() {
							IncidentType = new IncidentTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredIncidentTypes.CountAsync();

            return new PagedResultDto<GetIncidentTypeForViewDto>(
                totalCount,
                await incidentTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetIncidentTypeForViewDto> GetIncidentTypeForView(int id)
         {
            var incidentType = await _incidentTypeRepository.GetAsync(id);

            var output = new GetIncidentTypeForViewDto { IncidentType = ObjectMapper.Map<IncidentTypeDto>(incidentType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentTypes_Edit)]
		 public async Task<GetIncidentTypeForEditOutput> GetIncidentTypeForEdit(EntityDto input)
         {
            var incidentType = await _incidentTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetIncidentTypeForEditOutput {IncidentType = ObjectMapper.Map<CreateOrEditIncidentTypeDto>(incidentType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditIncidentTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentTypes_Create)]
		 protected virtual async Task Create(CreateOrEditIncidentTypeDto input)
         {
            var incidentType = ObjectMapper.Map<IncidentType>(input);

			
			if (AbpSession.TenantId != null)
			{
				incidentType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _incidentTypeRepository.InsertAsync(incidentType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditIncidentTypeDto input)
         {
            var incidentType = await _incidentTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, incidentType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_IncidentTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _incidentTypeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetIncidentTypesToExcel(GetAllIncidentTypesForExcelInput input)
         {
			
			var filteredIncidentTypes = _incidentTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var query = (from o in filteredIncidentTypes
                         select new GetIncidentTypeForViewDto() { 
							IncidentType = new IncidentTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var incidentTypeListDtos = await query.ToListAsync();

            return _incidentTypesExcelExporter.ExportToFile(incidentTypeListDtos);
         }


    }
}