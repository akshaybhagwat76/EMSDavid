using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Metrics.Dtos;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Metrics
{
    [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms)]
    public class UomsAppService : EmsAppServiceBase, IUomsAppService
    {
        //private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Uom> _uomRepository;


        public UomsAppService(IRepository<Uom> uomRepository)
        {
            //_unitOfWorkManager = unitOfWorkManager;
            _uomRepository = uomRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms)]
        public async Task<PagedResultDto<GetUomForViewDto>> GetAll(GetAllUomsInput input)
        {

            var filteredUoms = _uomRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.UnitOfMeasurement.Contains(input.Filter));

            var pagedAndFilteredUoms = filteredUoms
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var uoms = from o in pagedAndFilteredUoms
                       select new GetUomForViewDto()
                       {
                           Uom = new UomDto
                           {
                               UnitOfMeasurement = o.UnitOfMeasurement,
                               Id = o.Id
                           }
                       };

            var totalCount = await filteredUoms.CountAsync();

            return new PagedResultDto<GetUomForViewDto>(
                totalCount,
                await uoms.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms)]
        public async Task<GetUomForViewDto> GetUomForView(int id)
        {
            var uom = await _uomRepository.GetAsync(id);

            var output = new GetUomForViewDto { Uom = ObjectMapper.Map<UomDto>(uom) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms_Edit)]
        public async Task<GetUomForEditOutput> GetUomForEdit(EntityDto input)
        {
            var uom = await _uomRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUomForEditOutput { Uom = ObjectMapper.Map<CreateOrEditUomDto>(uom) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUomDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms_Create)]
        protected virtual async Task Create(CreateOrEditUomDto input)
        {
            var uom = ObjectMapper.Map<Uom>(input);


            if (AbpSession.TenantId != null)
            {
                uom.TenantId = (int?)AbpSession.TenantId;
            }


            await _uomRepository.InsertAsync(uom);
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms_Edit)]
        protected virtual async Task Update(CreateOrEditUomDto input)
        {
            var uom = await _uomRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, uom);
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_Uoms_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _uomRepository.DeleteAsync(input.Id);
        }



        public async Task<PagedResultDto<UomLookupTableDto>> GetAllUomForLookupTable(GetAllForLookupTableInput input)
        {
            //using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            //{
            //var tenantInfo = await TenantManager.GetTenantInfo();
            //var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Uom");

            var query = _uomRepository
                .GetAll()
                //.WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.UnitOfMeasurement.Contains(input.Filter)
            );

            var totalCount = await query.CountAsync();

            var uomList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<UomLookupTableDto>();
            foreach (var uom in uomList)
            {
                lookupTableDtoList.Add(new UomLookupTableDto
                {
                    Id = uom.Id,
                    DisplayName = uom.UnitOfMeasurement?.ToString()
                });
            }

            return new PagedResultDto<UomLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
            //}
        }

    }
}