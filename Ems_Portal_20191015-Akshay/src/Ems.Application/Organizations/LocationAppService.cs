using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Ems.Organizations.Dtos;
using System.Threading.Tasks;
using System.Linq;

namespace Ems.Organizations
{
    public class LocationAppService : EmsAppServiceBase, ILocationAppService
    {
        private readonly IRepository<Location> _locationRepository;

        public LocationAppService(IRepository<Location> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task CreateOrEdit(CreateOrEditLocationDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        protected virtual async Task Create(CreateOrEditLocationDto input)
        {
            var location = ObjectMapper.Map<Location>(input);

            if (AbpSession.TenantId != null)
            {
                location.TenantId = (int?)AbpSession.TenantId;
            }

            await _locationRepository.InsertAsync(location);
        }

        protected virtual async Task Update(CreateOrEditLocationDto input)
        {
            var location = await _locationRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, location);
        }

        public async Task Delete(EntityDto input)
        {
            await _locationRepository.DeleteAsync(input.Id);
        }
    }
}
