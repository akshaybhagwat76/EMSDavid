

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Organizations.Exporting;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Organizations
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_SsicCodes)]
    public class SsicCodesAppService : EmsAppServiceBase, ISsicCodesAppService
    {
		 private readonly IRepository<SsicCode> _ssicCodeRepository;
		 private readonly ISsicCodesExcelExporter _ssicCodesExcelExporter;
		 

		  public SsicCodesAppService(IRepository<SsicCode> ssicCodeRepository, ISsicCodesExcelExporter ssicCodesExcelExporter ) 
		  {
			_ssicCodeRepository = ssicCodeRepository;
			_ssicCodesExcelExporter = ssicCodesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetSsicCodeForViewDto>> GetAll(GetAllSsicCodesInput input)
         {
			
			var filteredSsicCodes = _ssicCodeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Code.Contains(input.Filter) || e.SSIC.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter),  e => e.Code.ToLower() == input.CodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SSICFilter),  e => e.SSIC.ToLower() == input.SSICFilter.ToLower().Trim());

			var pagedAndFilteredSsicCodes = filteredSsicCodes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var ssicCodes = from o in pagedAndFilteredSsicCodes
                         select new GetSsicCodeForViewDto() {
							SsicCode = new SsicCodeDto
							{
                                Code = o.Code,
                                SSIC = o.SSIC,
                                Id = o.Id
							}
						};

            var totalCount = await filteredSsicCodes.CountAsync();

            return new PagedResultDto<GetSsicCodeForViewDto>(
                totalCount,
                await ssicCodes.ToListAsync()
            );
         }
		 
		 public async Task<GetSsicCodeForViewDto> GetSsicCodeForView(int id)
         {
            var ssicCode = await _ssicCodeRepository.GetAsync(id);

            var output = new GetSsicCodeForViewDto { SsicCode = ObjectMapper.Map<SsicCodeDto>(ssicCode) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_SsicCodes_Edit)]
		 public async Task<GetSsicCodeForEditOutput> GetSsicCodeForEdit(EntityDto input)
         {
            var ssicCode = await _ssicCodeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSsicCodeForEditOutput {SsicCode = ObjectMapper.Map<CreateOrEditSsicCodeDto>(ssicCode)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSsicCodeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_SsicCodes_Create)]
		 protected virtual async Task Create(CreateOrEditSsicCodeDto input)
         {
            var ssicCode = ObjectMapper.Map<SsicCode>(input);

			
			if (AbpSession.TenantId != null)
			{
				ssicCode.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _ssicCodeRepository.InsertAsync(ssicCode);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_SsicCodes_Edit)]
		 protected virtual async Task Update(CreateOrEditSsicCodeDto input)
         {
            var ssicCode = await _ssicCodeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, ssicCode);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_SsicCodes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _ssicCodeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetSsicCodesToExcel(GetAllSsicCodesForExcelInput input)
         {
			
			var filteredSsicCodes = _ssicCodeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Code.Contains(input.Filter) || e.SSIC.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter),  e => e.Code.ToLower() == input.CodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SSICFilter),  e => e.SSIC.ToLower() == input.SSICFilter.ToLower().Trim());

			var query = (from o in filteredSsicCodes
                         select new GetSsicCodeForViewDto() { 
							SsicCode = new SsicCodeDto
							{
                                Code = o.Code,
                                SSIC = o.SSIC,
                                Id = o.Id
							}
						 });


            var ssicCodeListDtos = await query.ToListAsync();

            return _ssicCodesExcelExporter.ExportToFile(ssicCodeListDtos);
         }


    }
}