using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations
{
    public interface IQuotationStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetQuotationStatusForViewDto>> GetAll(GetAllQuotationStatusesInput input);

        Task<GetQuotationStatusForViewDto> GetQuotationStatusForView(int id);

		Task<GetQuotationStatusForEditOutput> GetQuotationStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditQuotationStatusDto input);

		Task Delete(EntityDto input);

		
    }
}