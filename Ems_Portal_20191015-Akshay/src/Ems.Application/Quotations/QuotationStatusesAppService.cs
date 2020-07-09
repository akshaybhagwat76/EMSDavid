

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_QuotationStatuses)]
    public class QuotationStatusesAppService : EmsAppServiceBase, IQuotationStatusesAppService
    {
		 private readonly IRepository<QuotationStatus> _quotationStatusRepository;
		 

		  public QuotationStatusesAppService(IRepository<QuotationStatus> quotationStatusRepository ) 
		  {
			_quotationStatusRepository = quotationStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetQuotationStatusForViewDto>> GetAll(GetAllQuotationStatusesInput input)
         {
			
			var filteredQuotationStatuses = _quotationStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredQuotationStatuses = filteredQuotationStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var quotationStatuses = from o in pagedAndFilteredQuotationStatuses
                         select new GetQuotationStatusForViewDto() {
							QuotationStatus = new QuotationStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredQuotationStatuses.CountAsync();

            return new PagedResultDto<GetQuotationStatusForViewDto>(
                totalCount,
                await quotationStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetQuotationStatusForViewDto> GetQuotationStatusForView(int id)
         {
            var quotationStatus = await _quotationStatusRepository.GetAsync(id);

            var output = new GetQuotationStatusForViewDto { QuotationStatus = ObjectMapper.Map<QuotationStatusDto>(quotationStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_QuotationStatuses_Edit)]
		 public async Task<GetQuotationStatusForEditOutput> GetQuotationStatusForEdit(EntityDto input)
         {
            var quotationStatus = await _quotationStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetQuotationStatusForEditOutput {QuotationStatus = ObjectMapper.Map<CreateOrEditQuotationStatusDto>(quotationStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditQuotationStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_QuotationStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditQuotationStatusDto input)
         {
            var quotationStatus = ObjectMapper.Map<QuotationStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				quotationStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _quotationStatusRepository.InsertAsync(quotationStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_QuotationStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditQuotationStatusDto input)
         {
            var quotationStatus = await _quotationStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, quotationStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_QuotationStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _quotationStatusRepository.DeleteAsync(input.Id);
         } 
    }
}