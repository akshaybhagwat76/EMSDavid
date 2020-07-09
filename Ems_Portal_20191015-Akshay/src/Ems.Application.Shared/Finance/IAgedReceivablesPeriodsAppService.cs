using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Finance.Dtos;
using Ems.Dto;


namespace Ems.Finance
{
    public interface IAgedReceivablesPeriodsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAgedReceivablesPeriodForViewDto>> GetAll(GetAllAgedReceivablesPeriodsInput input);

        Task<GetAgedReceivablesPeriodForViewDto> GetAgedReceivablesPeriodForView(int id);

		Task<GetAgedReceivablesPeriodForEditOutput> GetAgedReceivablesPeriodForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAgedReceivablesPeriodDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAgedReceivablesPeriodsToExcel(GetAllAgedReceivablesPeriodsForExcelInput input);

		
    }
}