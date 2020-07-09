using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Quotations.Dtos;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Quotations
{
    public class ItemTypesAppService : EmsAppServiceBase, IItemTypesAppService
    {
        private readonly IRepository<ItemType> _itemTypeRepository;
        //private readonly IUnitOfWorkManager _unitOfWorkManager;


        public ItemTypesAppService(IRepository<ItemType> itemTypeRepository)
        {
            //_unitOfWorkManager = unitOfWorkManager;
            _itemTypeRepository = itemTypeRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_ItemTypes)]
        public async Task<PagedResultDto<GetItemTypeForViewDto>> GetAll(GetAllItemTypesInput input)
        {

            var filteredItemTypes = _itemTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

            var pagedAndFilteredItemTypes = filteredItemTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var itemTypes = from o in pagedAndFilteredItemTypes
                            select new GetItemTypeForViewDto()
                            {
                                ItemType = new ItemTypeDto
                                {
                                    Type = o.Type,
                                    Description = o.Description,
                                    Id = o.Id
                                }
                            };

            var totalCount = await filteredItemTypes.CountAsync();

            return new PagedResultDto<GetItemTypeForViewDto>(
                totalCount,
                await itemTypes.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_ItemTypes)]
        public async Task<GetItemTypeForViewDto> GetItemTypeForView(int id)
        {
            var itemType = await _itemTypeRepository.GetAsync(id);

            var output = new GetItemTypeForViewDto { ItemType = ObjectMapper.Map<ItemTypeDto>(itemType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_ItemTypes_Edit)]
        public async Task<GetItemTypeForEditOutput> GetItemTypeForEdit(EntityDto input)
        {
            var itemType = await _itemTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetItemTypeForEditOutput { ItemType = ObjectMapper.Map<CreateOrEditItemTypeDto>(itemType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditItemTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Configuration_ItemTypes_Create)]
        protected virtual async Task Create(CreateOrEditItemTypeDto input)
        {
            var itemType = ObjectMapper.Map<ItemType>(input);


            if (AbpSession.TenantId != null)
            {
                itemType.TenantId = (int?)AbpSession.TenantId;
            }


            await _itemTypeRepository.InsertAsync(itemType);
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_ItemTypes_Edit)]
        protected virtual async Task Update(CreateOrEditItemTypeDto input)
        {
            var itemType = await _itemTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, itemType);
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_ItemTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _itemTypeRepository.DeleteAsync(input.Id);
        }


        public async Task<PagedResultDto<ItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input)
        {
            //using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            //{
            //var tenantInfo = await TenantManager.GetTenantInfo();
            //var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "ItemType");

            var query = _itemTypeRepository
                .GetAll()
                //.WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Type.Contains(input.Filter)
            );

            var totalCount = await query.CountAsync();

            var itemTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ItemTypeLookupTableDto>();
            foreach (var itemType in itemTypeList)
            {
                lookupTableDtoList.Add(new ItemTypeLookupTableDto
                {
                    Id = itemType.Id,
                    DisplayName = itemType.Type?.ToString()
                });
            }

            return new PagedResultDto<ItemTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
            //}
        }

    }
}