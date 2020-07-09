using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Organizations.Dtos;
using System.Threading.Tasks;

namespace Ems.Organizations
{
    public interface ILocationAppService : IApplicationService
    {
        Task CreateOrEdit(CreateOrEditLocationDto input);

        Task Delete(EntityDto input);
    }
}
