using Ems.Assets;
using Ems.Metrics;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Exporting;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
    public class SupportItemsAppService : EmsAppServiceBase, ISupportItemsAppService
    {
        private readonly string _entityType = "SupportItem";

        private readonly IRepository<SupportItem> _supportItemRepository;
        private readonly ISupportItemsExcelExporter _supportItemsExcelExporter;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<AssetClass, int> _lookup_assetClassRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<SupportContract, int> _lookup_supportContractRepository;
        private readonly IRepository<ConsumableType, int> _lookup_consumableTypeRepository;
        private readonly IRepository<SupportType, int> _lookup_supportTypeRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;

        public SupportItemsAppService
            (
                IRepository<SupportItem> supportItemRepository, 
                ISupportItemsExcelExporter supportItemsExcelExporter, 
                IRepository<Asset, int> lookup_assetRepository, 
                IRepository<AssetClass, int> lookup_assetClassRepository, 
                IRepository<Uom, int> lookup_uomRepository, 
                IRepository<SupportContract, int> lookup_supportContractRepository, 
                IRepository<ConsumableType, int> lookup_consumableTypeRepository,
                IRepository<AssetOwnership> assetOwnershipRepository,
                IRepository<SupportType, int> lookup_supportTypeRepository
            )
        {
            _supportItemRepository = supportItemRepository;
            _supportItemsExcelExporter = supportItemsExcelExporter;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_assetClassRepository = lookup_assetClassRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_supportContractRepository = lookup_supportContractRepository;
            _lookup_consumableTypeRepository = lookup_consumableTypeRepository;
            _lookup_supportTypeRepository = lookup_supportTypeRepository;
            _assetOwnershipRepository = assetOwnershipRepository;
        }

        public async Task<PagedResultDto<GetSupportItemForViewDto>> GetAll(GetAllSupportItemsInput input)
        {

            var filteredSupportItems = _supportItemRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.UomFk)
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.ConsumableTypeFk)
                        .Include(e => e.SupportTypeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinUnitPriceFilter != null, e => e.UnitPrice >= input.MinUnitPriceFilter)
                        .WhereIf(input.MaxUnitPriceFilter != null, e => e.UnitPrice <= input.MaxUnitPriceFilter)
                        .WhereIf(input.MinFrequencyFilter != null, e => e.Frequency >= input.MinFrequencyFilter)
                        .WhereIf(input.MaxFrequencyFilter != null, e => e.Frequency <= input.MaxFrequencyFilter)
                        .WhereIf(input.IsAdHocFilter > -1, e => Convert.ToInt32(e.IsAdHoc) == input.IsAdHocFilter)
                        .WhereIf(input.IsChargeableFilter > -1, e => Convert.ToInt32(e.IsChargeable) == input.IsChargeableFilter)
                        .WhereIf(input.IsStandbyReplacementUnitFilter > -1, e => Convert.ToInt32(e.IsStandbyReplacementUnit) == input.IsStandbyReplacementUnitFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ConsumableTypeTypeFilter), e => e.ConsumableTypeFk != null && e.ConsumableTypeFk.Type.ToLower() == input.ConsumableTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportTypeTypeFilter), e => e.SupportTypeFk != null && e.SupportTypeFk.Type.ToLower() == input.SupportTypeTypeFilter.ToLower().Trim());

            var pagedAndFilteredSupportItems = filteredSupportItems
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var supportItems = from o in pagedAndFilteredSupportItems
                               join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               join o2 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o2.Id into j2
                               from s2 in j2.DefaultIfEmpty()

                               join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                               from s3 in j3.DefaultIfEmpty()

                               join o4 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o4.Id into j4
                               from s4 in j4.DefaultIfEmpty()

                               join o5 in _lookup_consumableTypeRepository.GetAll() on o.ConsumableTypeId equals o5.Id into j5
                               from s5 in j5.DefaultIfEmpty()

                               join o6 in _lookup_supportTypeRepository.GetAll() on o.SupportTypeId equals o6.Id into j6
                               from s6 in j6.DefaultIfEmpty()

                               select new GetSupportItemForViewDto()
                               {
                                   SupportItem = new SupportItemDto
                                   {
                                       Description = o.Description,
                                       UnitPrice = o.UnitPrice,
                                       Frequency = o.Frequency,
                                       IsAdHoc = o.IsAdHoc,
                                       IsChargeable = o.IsChargeable,
                                       IsStandbyReplacementUnit = o.IsStandbyReplacementUnit,

                                       Id = o.Id
                                   },
                                   AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                                   AssetClassClass = s2 == null ? "" : s2.Class.ToString(),
                                   UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement.ToString(),
                                   SupportContractTitle = s4 == null ? "" : s4.Title.ToString(),
                                   ConsumableTypeType = s5 == null ? "" : s5.Type.ToString(),
                                   SupportTypeType = s6 == null ? "" : s6.Type.ToString()
                               };

            var totalCount = await filteredSupportItems.CountAsync();

            return new PagedResultDto<GetSupportItemForViewDto>(
                totalCount,
                await supportItems.ToListAsync()
            );
        }

        public async Task<GetSupportItemForViewDto> GetSupportItemForView(int id)
        {
            var supportItem = await _supportItemRepository.GetAsync(id);

            var output = new GetSupportItemForViewDto { SupportItem = ObjectMapper.Map<SupportItemDto>(supportItem) };

            if (output.SupportItem != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.SupportItem.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            if (output.SupportItem.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.SupportItem.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.SupportItem != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.SupportItem.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.SupportItem.SupportContractId != null)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.SupportItem.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

            if (output.SupportItem.ConsumableTypeId != null)
            {
                var _lookupConsumableType = await _lookup_consumableTypeRepository.FirstOrDefaultAsync((int)output.SupportItem.ConsumableTypeId);
                output.ConsumableTypeType = _lookupConsumableType.Type.ToString();
            }

            if (output.SupportItem.SupportTypeId != null)
            {
                var _lookupSupportType = await _lookup_supportTypeRepository.FirstOrDefaultAsync((int)output.SupportItem.SupportTypeId);
                output.SupportTypeType = _lookupSupportType.Type.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_SupportItemsEdit)]
        public async Task<GetSupportItemForEditOutput> GetSupportItemForEdit(EntityDto input)
        {
            var supportItem = await _supportItemRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSupportItemForEditOutput { SupportItem = ObjectMapper.Map<CreateOrEditSupportItemDto>(supportItem) };

            if (output.SupportItem != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.SupportItem.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            if (output.SupportItem.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.SupportItem.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.SupportItem != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.SupportItem.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.SupportItem.SupportContractId != null)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.SupportItem.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

            if (output.SupportItem.ConsumableTypeId != null)
            {
                var _lookupConsumableType = await _lookup_consumableTypeRepository.FirstOrDefaultAsync((int)output.SupportItem.ConsumableTypeId);
                output.ConsumableTypeType = _lookupConsumableType.Type.ToString();
            }

            if (output.SupportItem.SupportTypeId != null)
            {
                var _lookupSupportType = await _lookup_supportTypeRepository.FirstOrDefaultAsync((int)output.SupportItem.SupportTypeId);
                output.SupportTypeType = _lookupSupportType.Type.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditSupportItemDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_SupportItemsCreate)]
        protected virtual async Task Create(CreateOrEditSupportItemDto input)
        {
            var supportItem = ObjectMapper.Map<SupportItem>(input);


            if (AbpSession.TenantId != null)
            {
                supportItem.TenantId = (int?)AbpSession.TenantId;
            }


            await _supportItemRepository.InsertAsync(supportItem);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_SupportItemsEdit)]
        protected virtual async Task Update(CreateOrEditSupportItemDto input)
        {
            var supportItem = await _supportItemRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, supportItem);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_SupportItemsDelete)]
        public async Task Delete(EntityDto input)
        {
            await _supportItemRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetSupportItemsToExcel(GetAllSupportItemsForExcelInput input)
        {

            var filteredSupportItems = _supportItemRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.UomFk)
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.ConsumableTypeFk)
                        .Include(e => e.SupportTypeFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinUnitPriceFilter != null, e => e.UnitPrice >= input.MinUnitPriceFilter)
                        .WhereIf(input.MaxUnitPriceFilter != null, e => e.UnitPrice <= input.MaxUnitPriceFilter)
                        .WhereIf(input.MinFrequencyFilter != null, e => e.Frequency >= input.MinFrequencyFilter)
                        .WhereIf(input.MaxFrequencyFilter != null, e => e.Frequency <= input.MaxFrequencyFilter)
                        .WhereIf(input.IsAdHocFilter > -1, e => Convert.ToInt32(e.IsAdHoc) == input.IsAdHocFilter)
                        .WhereIf(input.IsChargeableFilter > -1, e => Convert.ToInt32(e.IsChargeable) == input.IsChargeableFilter)
                        .WhereIf(input.IsStandbyReplacementUnitFilter > -1, e => Convert.ToInt32(e.IsStandbyReplacementUnit) == input.IsStandbyReplacementUnitFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ConsumableTypeTypeFilter), e => e.ConsumableTypeFk != null && e.ConsumableTypeFk.Type.ToLower() == input.ConsumableTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportTypeTypeFilter), e => e.SupportTypeFk != null && e.SupportTypeFk.Type.ToLower() == input.SupportTypeTypeFilter.ToLower().Trim());

            var query = (from o in filteredSupportItems
                         join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_consumableTypeRepository.GetAll() on o.ConsumableTypeId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         join o6 in _lookup_supportTypeRepository.GetAll() on o.SupportTypeId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()

                         select new GetSupportItemForViewDto()
                         {
                             SupportItem = new SupportItemDto
                             {
                                 Description = o.Description,
                                 UnitPrice = o.UnitPrice,
                                 Frequency = o.Frequency,
                                 IsAdHoc = o.IsAdHoc,
                                 IsChargeable = o.IsChargeable,
                                 IsStandbyReplacementUnit = o.IsStandbyReplacementUnit,

                                 Id = o.Id
                             },
                             AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                             AssetClassClass = s2 == null ? "" : s2.Class.ToString(),
                             UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement.ToString(),
                             SupportContractTitle = s4 == null ? "" : s4.Title.ToString(),
                             ConsumableTypeType = s5 == null ? "" : s5.Type.ToString(),
                             SupportTypeType = s6 == null ? "" : s6.Type.ToString()
                         });


            var supportItemListDtos = await query.ToListAsync();

            return _supportItemsExcelExporter.ExportToFile(supportItemListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportItemAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            List<int?> myAssetIds = new List<int?>();

            if (tenantInfo.Tenant.TenantType == "A")
            {
                myAssetIds = _assetOwnershipRepository.GetAll()
                        .Where(e => e.AssetOwnerId == tenantInfo.AssetOwner.Id)
                        .Select(e => e.AssetId)
                        .ToList();
            }

            var query = _lookup_assetRepository.GetAll()
                .WhereIf(tenantInfo.Tenant.TenantType == "A", e => myAssetIds.Contains(e.Id)) // Get all my Assets
                .WhereIf(tenantInfo.Tenant.Id != 0 && tenantInfo.Tenant.TenantType != "A" && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                .WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Reference.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportItemAssetLookupTableDto>();
            foreach (var asset in assetList)
            {
                lookupTableDtoList.Add(new SupportItemAssetLookupTableDto
                {
                    Id = asset.Id,
                    DisplayName = asset.Reference?.ToString()
                });
            }

            return new PagedResultDto<SupportItemAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportItemAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetClassRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Class.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetClassList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportItemAssetClassLookupTableDto>();
            foreach (var assetClass in assetClassList)
            {
                lookupTableDtoList.Add(new SupportItemAssetClassLookupTableDto
                {
                    Id = assetClass.Id,
                    DisplayName = assetClass.Class?.ToString()
                });
            }

            return new PagedResultDto<SupportItemAssetClassLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportItemSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_supportContractRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Title.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var supportContractList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportItemSupportContractLookupTableDto>();
            foreach (var supportContract in supportContractList)
            {
                lookupTableDtoList.Add(new SupportItemSupportContractLookupTableDto
                {
                    Id = supportContract.Id,
                    DisplayName = supportContract.Title?.ToString()
                });
            }

            return new PagedResultDto<SupportItemSupportContractLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportItemConsumableTypeLookupTableDto>> GetAllConsumableTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_consumableTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var consumableTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportItemConsumableTypeLookupTableDto>();
            foreach (var consumableType in consumableTypeList)
            {
                lookupTableDtoList.Add(new SupportItemConsumableTypeLookupTableDto
                {
                    Id = consumableType.Id,
                    DisplayName = consumableType.Type?.ToString()
                });
            }

            return new PagedResultDto<SupportItemConsumableTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportItemSupportTypeLookupTableDto>> GetAllSupportTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_supportTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var supportTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportItemSupportTypeLookupTableDto>();
            foreach (var supportType in supportTypeList)
            {
                lookupTableDtoList.Add(new SupportItemSupportTypeLookupTableDto
                {
                    Id = supportType.Id,
                    DisplayName = supportType.Type?.ToString()
                });
            }

            return new PagedResultDto<SupportItemSupportTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<GetSupportItemForViewDto>> GetSome(int supportContractId, PagedAndSortedResultRequestDto input)
        {
            var filteredSupportItems = _supportItemRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.UomFk)
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.ConsumableTypeFk)
                        .Include(e => e.SupportTypeFk)
                        .Where(e => e.SupportContractId == supportContractId);

            var pagedAndFilteredSupportItems = filteredSupportItems
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var supportItems = from o in pagedAndFilteredSupportItems
                               join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               join o2 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o2.Id into j2
                               from s2 in j2.DefaultIfEmpty()

                               join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                               from s3 in j3.DefaultIfEmpty()

                               join o4 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o4.Id into j4
                               from s4 in j4.DefaultIfEmpty()

                               join o5 in _lookup_consumableTypeRepository.GetAll() on o.ConsumableTypeId equals o5.Id into j5
                               from s5 in j5.DefaultIfEmpty()

                               join o6 in _lookup_supportTypeRepository.GetAll() on o.SupportTypeId equals o6.Id into j6
                               from s6 in j6.DefaultIfEmpty()

                               select new GetSupportItemForViewDto()
                               {
                                   SupportItem = new SupportItemDto
                                   {
                                       Description = o.Description,
                                       UnitPrice = o.UnitPrice,
                                       Frequency = o.Frequency,
                                       IsAdHoc = o.IsAdHoc,
                                       IsChargeable = o.IsChargeable,
                                       IsStandbyReplacementUnit = o.IsStandbyReplacementUnit,

                                       Id = o.Id
                                   },
                                   AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                                   AssetClassClass = s2 == null ? "" : s2.Class.ToString(),
                                   UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement.ToString(),
                                   SupportContractTitle = s4 == null ? "" : s4.Title.ToString(),
                                   ConsumableTypeType = s5 == null ? "" : s5.Type.ToString(),
                                   SupportTypeType = s6 == null ? "" : s6.Type.ToString()
                               };

            var totalCount = await filteredSupportItems.CountAsync();

            return new PagedResultDto<GetSupportItemForViewDto>(
                totalCount,
                await supportItems.ToListAsync()
            );
        }

    }
}