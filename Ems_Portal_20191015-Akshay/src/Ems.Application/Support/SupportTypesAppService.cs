

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_SupportTypes)]
    public class SupportTypesAppService : EmsAppServiceBase, ISupportTypesAppService
    {
		 private readonly IRepository<SupportType> _supportTypeRepository;
		 

		  public SupportTypesAppService(IRepository<SupportType> supportTypeRepository ) 
		  {
			_supportTypeRepository = supportTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetSupportTypeForViewDto>> GetAll(GetAllSupportTypesInput input)
         {
			
			var filteredSupportTypes = _supportTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredSupportTypes = filteredSupportTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var supportTypes = from o in pagedAndFilteredSupportTypes
                         select new GetSupportTypeForViewDto() {
							SupportType = new SupportTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredSupportTypes.CountAsync();

            return new PagedResultDto<GetSupportTypeForViewDto>(
                totalCount,
                await supportTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetSupportTypeForViewDto> GetSupportTypeForView(int id)
         {
            var supportType = await _supportTypeRepository.GetAsync(id);

            var output = new GetSupportTypeForViewDto { SupportType = ObjectMapper.Map<SupportTypeDto>(supportType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_SupportTypes_Edit)]
		 public async Task<GetSupportTypeForEditOutput> GetSupportTypeForEdit(EntityDto input)
         {
            var supportType = await _supportTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetSupportTypeForEditOutput {SupportType = ObjectMapper.Map<CreateOrEditSupportTypeDto>(supportType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditSupportTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_SupportTypes_Create)]
		 protected virtual async Task Create(CreateOrEditSupportTypeDto input)
         {
            var supportType = ObjectMapper.Map<SupportType>(input);

			
			if (AbpSession.TenantId != null)
			{
				supportType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _supportTypeRepository.InsertAsync(supportType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_SupportTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditSupportTypeDto input)
         {
            var supportType = await _supportTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, supportType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_SupportTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _supportTypeRepository.DeleteAsync(input.Id);
         } 
    }
}