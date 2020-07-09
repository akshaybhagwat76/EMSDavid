using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Organizations.Dtos;
using Ems.Dto;

namespace Ems.Organizations
{
    public interface ISsicCodesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSsicCodeForViewDto>> GetAll(GetAllSsicCodesInput input);

        Task<GetSsicCodeForViewDto> GetSsicCodeForView(int id);

		Task<GetSsicCodeForEditOutput> GetSsicCodeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSsicCodeDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetSsicCodesToExcel(GetAllSsicCodesForExcelInput input);

		
    }
}