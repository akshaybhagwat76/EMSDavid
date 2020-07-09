using Ems.Authorization.Users;
using Ems.Support;


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
	[AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
    public class IncidentUpdatesAppService : EmsAppServiceBase, IIncidentUpdatesAppService
    {
		 private readonly IRepository<IncidentUpdate> _incidentUpdateRepository;
		 private readonly IIncidentUpdatesExcelExporter _incidentUpdatesExcelExporter;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 private readonly IRepository<Incident,int> _lookup_incidentRepository;
		 

		  public IncidentUpdatesAppService(IRepository<IncidentUpdate> incidentUpdateRepository, IIncidentUpdatesExcelExporter incidentUpdatesExcelExporter , IRepository<User, long> lookup_userRepository, IRepository<Incident, int> lookup_incidentRepository) 
		  {
			_incidentUpdateRepository = incidentUpdateRepository;
			_incidentUpdatesExcelExporter = incidentUpdatesExcelExporter;
			_lookup_userRepository = lookup_userRepository;
		_lookup_incidentRepository = lookup_incidentRepository;
		
		  }

		 public async Task<PagedResultDto<GetIncidentUpdateForViewDto>> GetAll(GetAllIncidentUpdatesInput input)
         {
			
			var filteredIncidentUpdates = _incidentUpdateRepository.GetAll()
						.Include( e => e.UserFk)
						.Include( e => e.IncidentFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Update.Contains(input.Filter))
						.WhereIf(input.MinUpdatedFilter != null, e => e.Updated >= input.MinUpdatedFilter)
						.WhereIf(input.MaxUpdatedFilter != null, e => e.Updated <= input.MaxUpdatedFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UpdateFilter),  e => e.Update.ToLower() == input.UpdateFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim());

			var pagedAndFilteredIncidentUpdates = filteredIncidentUpdates
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var incidentUpdates = from o in pagedAndFilteredIncidentUpdates
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetIncidentUpdateForViewDto() {
							IncidentUpdate = new IncidentUpdateDto
							{
                                Updated = o.Updated,
                                Update = o.Update,
                                Id = o.Id
							},
                         	UserName = s1 == null ? "" : s1.Name.ToString(),
                         	IncidentDescription = s2 == null ? "" : s2.Description.ToString()
						};

            var totalCount = await filteredIncidentUpdates.CountAsync();

            return new PagedResultDto<GetIncidentUpdateForViewDto>(
                totalCount,
                await incidentUpdates.ToListAsync()
            );
         }
		 
		 public async Task<GetIncidentUpdateForViewDto> GetIncidentUpdateForView(int id)
         {
            var incidentUpdate = await _incidentUpdateRepository.GetAsync(id);

            var output = new GetIncidentUpdateForViewDto { IncidentUpdate = ObjectMapper.Map<IncidentUpdateDto>(incidentUpdate) };

		    if (output.IncidentUpdate != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.IncidentUpdate.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }

		    if (output.IncidentUpdate != null)
            {
                var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.IncidentUpdate.IncidentId);
                output.IncidentDescription = _lookupIncident.Description.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_Incidents_EditIncidentUpdates)]
		 public async Task<GetIncidentUpdateForEditOutput> GetIncidentUpdateForEdit(EntityDto input)
         {
            var incidentUpdate = await _incidentUpdateRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetIncidentUpdateForEditOutput {IncidentUpdate = ObjectMapper.Map<CreateOrEditIncidentUpdateDto>(incidentUpdate)};

		    if (output.IncidentUpdate != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.IncidentUpdate.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }

		    if (output.IncidentUpdate != null)
            {
                var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.IncidentUpdate.IncidentId);
                output.IncidentDescription = _lookupIncident.Description.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditIncidentUpdateDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Incidents_CreateIncidentUpdates)]
		 protected virtual async Task Create(CreateOrEditIncidentUpdateDto input)
         {
            var incidentUpdate = ObjectMapper.Map<IncidentUpdate>(input);

			
			if (AbpSession.TenantId != null)
			{
				incidentUpdate.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _incidentUpdateRepository.InsertAsync(incidentUpdate);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Incidents_EditIncidentUpdates)]
		 protected virtual async Task Update(CreateOrEditIncidentUpdateDto input)
         {
            var incidentUpdate = await _incidentUpdateRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, incidentUpdate);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Incidents_DeleteIncidentUpdates)]
         public async Task Delete(EntityDto input)
         {
            await _incidentUpdateRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetIncidentUpdatesToExcel(GetAllIncidentUpdatesForExcelInput input)
         {
			
			var filteredIncidentUpdates = _incidentUpdateRepository.GetAll()
						.Include( e => e.UserFk)
						.Include( e => e.IncidentFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Update.Contains(input.Filter))
						.WhereIf(input.MinUpdatedFilter != null, e => e.Updated >= input.MinUpdatedFilter)
						.WhereIf(input.MaxUpdatedFilter != null, e => e.Updated <= input.MaxUpdatedFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UpdateFilter),  e => e.Update.ToLower() == input.UpdateFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim());

			var query = (from o in filteredIncidentUpdates
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetIncidentUpdateForViewDto() { 
							IncidentUpdate = new IncidentUpdateDto
							{
                                Updated = o.Updated,
                                Update = o.Update,
                                Id = o.Id
							},
                         	UserName = s1 == null ? "" : s1.Name.ToString(),
                         	IncidentDescription = s2 == null ? "" : s2.Description.ToString()
						 });


            var incidentUpdateListDtos = await query.ToListAsync();

            return _incidentUpdatesExcelExporter.ExportToFile(incidentUpdateListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
         public async Task<PagedResultDto<IncidentUpdateUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<IncidentUpdateUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new IncidentUpdateUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<IncidentUpdateUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
         public async Task<PagedResultDto<IncidentUpdateIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_incidentRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Description.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var incidentList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<IncidentUpdateIncidentLookupTableDto>();
			foreach(var incident in incidentList){
				lookupTableDtoList.Add(new IncidentUpdateIncidentLookupTableDto
				{
					Id = incident.Id,
					DisplayName = incident.Description?.ToString()
				});
			}

            return new PagedResultDto<IncidentUpdateIncidentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}