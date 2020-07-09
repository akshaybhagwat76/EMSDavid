using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations
{
    public interface IItemTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetItemTypeForViewDto>> GetAll(GetAllItemTypesInput input);

        Task<GetItemTypeForViewDto> GetItemTypeForView(int id);

		Task<GetItemTypeForEditOutput> GetItemTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditItemTypeDto input);

		Task Delete(EntityDto input);

		
    }
}