

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Quotations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Quotations
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_RfqTypes)]
    public class RfqTypesAppService : EmsAppServiceBase, IRfqTypesAppService
    {
		 private readonly IRepository<RfqType> _rfqTypeRepository;
		 

		  public RfqTypesAppService(IRepository<RfqType> rfqTypeRepository ) 
		  {
			_rfqTypeRepository = rfqTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetRfqTypeForViewDto>> GetAll(GetAllRfqTypesInput input)
         {
			
			var filteredRfqTypes = _rfqTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredRfqTypes = filteredRfqTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var rfqTypes = from o in pagedAndFilteredRfqTypes
                         select new GetRfqTypeForViewDto() {
							RfqType = new RfqTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredRfqTypes.CountAsync();

            return new PagedResultDto<GetRfqTypeForViewDto>(
                totalCount,
                await rfqTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetRfqTypeForViewDto> GetRfqTypeForView(int id)
         {
            var rfqType = await _rfqTypeRepository.GetAsync(id);

            var output = new GetRfqTypeForViewDto { RfqType = ObjectMapper.Map<RfqTypeDto>(rfqType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_RfqTypes_Edit)]
		 public async Task<GetRfqTypeForEditOutput> GetRfqTypeForEdit(EntityDto input)
         {
            var rfqType = await _rfqTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetRfqTypeForEditOutput {RfqType = ObjectMapper.Map<CreateOrEditRfqTypeDto>(rfqType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditRfqTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_RfqTypes_Create)]
		 protected virtual async Task Create(CreateOrEditRfqTypeDto input)
         {
            var rfqType = ObjectMapper.Map<RfqType>(input);

			
			if (AbpSession.TenantId != null)
			{
				rfqType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _rfqTypeRepository.InsertAsync(rfqType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_RfqTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditRfqTypeDto input)
         {
            var rfqType = await _rfqTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, rfqType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_RfqTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _rfqTypeRepository.DeleteAsync(input.Id);
         } 
    }
}