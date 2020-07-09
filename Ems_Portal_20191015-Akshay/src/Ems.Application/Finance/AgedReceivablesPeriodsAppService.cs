

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Finance.Exporting;
using Ems.Finance.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Finance
{
	[AbpAuthorize(AppPermissions.Pages_AgedReceivablesPeriods)]
    public class AgedReceivablesPeriodsAppService : EmsAppServiceBase, IAgedReceivablesPeriodsAppService
    {
		 private readonly IRepository<AgedReceivablesPeriod> _agedReceivablesPeriodRepository;
		 private readonly IAgedReceivablesPeriodsExcelExporter _agedReceivablesPeriodsExcelExporter;
		 

		  public AgedReceivablesPeriodsAppService(IRepository<AgedReceivablesPeriod> agedReceivablesPeriodRepository, IAgedReceivablesPeriodsExcelExporter agedReceivablesPeriodsExcelExporter ) 
		  {
			_agedReceivablesPeriodRepository = agedReceivablesPeriodRepository;
			_agedReceivablesPeriodsExcelExporter = agedReceivablesPeriodsExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetAgedReceivablesPeriodForViewDto>> GetAll(GetAllAgedReceivablesPeriodsInput input)
         {
			
			var filteredAgedReceivablesPeriods = _agedReceivablesPeriodRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinPeriodFilter != null, e => e.Period >= input.MinPeriodFilter)
						.WhereIf(input.MaxPeriodFilter != null, e => e.Period <= input.MaxPeriodFilter)
						.WhereIf(input.MinCurrentFilter != null, e => e.Current >= input.MinCurrentFilter)
						.WhereIf(input.MaxCurrentFilter != null, e => e.Current <= input.MaxCurrentFilter)
						.WhereIf(input.MinOver30Filter != null, e => e.Over30 >= input.MinOver30Filter)
						.WhereIf(input.MaxOver30Filter != null, e => e.Over30 <= input.MaxOver30Filter)
						.WhereIf(input.MinOver60Filter != null, e => e.Over60 >= input.MinOver60Filter)
						.WhereIf(input.MaxOver60Filter != null, e => e.Over60 <= input.MaxOver60Filter)
						.WhereIf(input.MinOver90Filter != null, e => e.Over90 >= input.MinOver90Filter)
						.WhereIf(input.MaxOver90Filter != null, e => e.Over90 <= input.MaxOver90Filter)
						.WhereIf(input.MinOver120Filter != null, e => e.Over120 >= input.MinOver120Filter)
						.WhereIf(input.MaxOver120Filter != null, e => e.Over120 <= input.MaxOver120Filter);

			var pagedAndFilteredAgedReceivablesPeriods = filteredAgedReceivablesPeriods
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var agedReceivablesPeriods = from o in pagedAndFilteredAgedReceivablesPeriods
                         select new GetAgedReceivablesPeriodForViewDto() {
							AgedReceivablesPeriod = new AgedReceivablesPeriodDto
							{
                                Period = o.Period,
                                Current = o.Current,
                                Over30 = o.Over30,
                                Over60 = o.Over60,
                                Over90 = o.Over90,
                                Over120 = o.Over120,
                                Id = o.Id
							}
						};

            var totalCount = await filteredAgedReceivablesPeriods.CountAsync();

            return new PagedResultDto<GetAgedReceivablesPeriodForViewDto>(
                totalCount,
                await agedReceivablesPeriods.ToListAsync()
            );
         }
		 
		 public async Task<GetAgedReceivablesPeriodForViewDto> GetAgedReceivablesPeriodForView(int id)
         {
            var agedReceivablesPeriod = await _agedReceivablesPeriodRepository.GetAsync(id);

            var output = new GetAgedReceivablesPeriodForViewDto { AgedReceivablesPeriod = ObjectMapper.Map<AgedReceivablesPeriodDto>(agedReceivablesPeriod) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_AgedReceivablesPeriods_Edit)]
		 public async Task<GetAgedReceivablesPeriodForEditOutput> GetAgedReceivablesPeriodForEdit(EntityDto input)
         {
            var agedReceivablesPeriod = await _agedReceivablesPeriodRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAgedReceivablesPeriodForEditOutput {AgedReceivablesPeriod = ObjectMapper.Map<CreateOrEditAgedReceivablesPeriodDto>(agedReceivablesPeriod)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAgedReceivablesPeriodDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_AgedReceivablesPeriods_Create)]
		 protected virtual async Task Create(CreateOrEditAgedReceivablesPeriodDto input)
         {
            var agedReceivablesPeriod = ObjectMapper.Map<AgedReceivablesPeriod>(input);

			
			if (AbpSession.TenantId != null)
			{
				agedReceivablesPeriod.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _agedReceivablesPeriodRepository.InsertAsync(agedReceivablesPeriod);
         }

		 [AbpAuthorize(AppPermissions.Pages_AgedReceivablesPeriods_Edit)]
		 protected virtual async Task Update(CreateOrEditAgedReceivablesPeriodDto input)
         {
            var agedReceivablesPeriod = await _agedReceivablesPeriodRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, agedReceivablesPeriod);
         }

		 [AbpAuthorize(AppPermissions.Pages_AgedReceivablesPeriods_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _agedReceivablesPeriodRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAgedReceivablesPeriodsToExcel(GetAllAgedReceivablesPeriodsForExcelInput input)
         {
			
			var filteredAgedReceivablesPeriods = _agedReceivablesPeriodRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinPeriodFilter != null, e => e.Period >= input.MinPeriodFilter)
						.WhereIf(input.MaxPeriodFilter != null, e => e.Period <= input.MaxPeriodFilter)
						.WhereIf(input.MinCurrentFilter != null, e => e.Current >= input.MinCurrentFilter)
						.WhereIf(input.MaxCurrentFilter != null, e => e.Current <= input.MaxCurrentFilter)
						.WhereIf(input.MinOver30Filter != null, e => e.Over30 >= input.MinOver30Filter)
						.WhereIf(input.MaxOver30Filter != null, e => e.Over30 <= input.MaxOver30Filter)
						.WhereIf(input.MinOver60Filter != null, e => e.Over60 >= input.MinOver60Filter)
						.WhereIf(input.MaxOver60Filter != null, e => e.Over60 <= input.MaxOver60Filter)
						.WhereIf(input.MinOver90Filter != null, e => e.Over90 >= input.MinOver90Filter)
						.WhereIf(input.MaxOver90Filter != null, e => e.Over90 <= input.MaxOver90Filter)
						.WhereIf(input.MinOver120Filter != null, e => e.Over120 >= input.MinOver120Filter)
						.WhereIf(input.MaxOver120Filter != null, e => e.Over120 <= input.MaxOver120Filter);

			var query = (from o in filteredAgedReceivablesPeriods
                         select new GetAgedReceivablesPeriodForViewDto() { 
							AgedReceivablesPeriod = new AgedReceivablesPeriodDto
							{
                                Period = o.Period,
                                Current = o.Current,
                                Over30 = o.Over30,
                                Over60 = o.Over60,
                                Over90 = o.Over90,
                                Over120 = o.Over120,
                                Id = o.Id
							}
						 });


            var agedReceivablesPeriodListDtos = await query.ToListAsync();

            return _agedReceivablesPeriodsExcelExporter.ExportToFile(agedReceivablesPeriodListDtos);
         }


    }
}