using Ems.Assets;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Assets.Exporting;
using Ems.Assets.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using Ems.Metrics;

namespace Ems.Assets
{
    [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
    public class LeaseItemsAppService : EmsAppServiceBase, ILeaseItemsAppService
    {
        private readonly string _entityType = "LeaseItems";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;
        private readonly ILeaseItemsExcelExporter _leaseItemsExcelExporter;
        private readonly IRepository<AssetClass, int> _lookup_assetClassRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<LeaseAgreement, int> _lookup_leaseAgreementRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;


        public LeaseItemsAppService
            (
                IUnitOfWorkManager unitOfWorkManager, 
                IRepository<LeaseItem> leaseItemRepository, 
                IRepository<AssetOwnership> assetOwnershipRepository,
                ILeaseItemsExcelExporter leaseItemsExcelExporter, 
                IRepository<AssetClass, int> lookup_assetClassRepository, 
                IRepository<Asset, int> lookup_assetRepository, 
                IRepository<LeaseAgreement, int> lookup_leaseAgreementRepository, 
                IRepository<Uom, int> lookup_uomRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _assetOwnershipRepository = assetOwnershipRepository;
            _leaseItemRepository = leaseItemRepository;
            _leaseItemsExcelExporter = leaseItemsExcelExporter;
            _lookup_assetClassRepository = lookup_assetClassRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_leaseAgreementRepository = lookup_leaseAgreementRepository;
            _lookup_uomRepository = lookup_uomRepository;

        }

        public async Task<PagedResultDto<GetLeaseItemForViewDto>> GetAll(GetAllLeaseItemsInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to enable global access to "User" entity
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var tenantType = tenantInfo.Tenant.TenantType;

                var filteredLeaseItems = _leaseItemRepository.GetAll()
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.AssetFk)
                        .Include(e => e.LeaseAgreementFk)
                        .WhereIf(tenantType == "A", e => e.LeaseAgreementFk.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any LeaseItems that are not relevant to the AssetOwener
                        .WhereIf(tenantType == "C", e => e.LeaseAgreementFk.CustomerId == tenantInfo.Customer.Id) // Filter out any LeaseItems that are not relevant to the Customer
                        .WhereIf(input.LeaseAgreementId > 0, e => e.LeaseAgreementId == input.LeaseAgreementId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Terms.Contains(input.Filter) || e.Item.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(input.MinDateAllocatedFilter != null, e => e.DateAllocated >= input.MinDateAllocatedFilter)
                        .WhereIf(input.MaxDateAllocatedFilter != null, e => e.DateAllocated <= input.MaxDateAllocatedFilter)
                        .WhereIf(input.MinAllocationPercentageFilter != null, e => e.AllocationPercentage >= input.MinAllocationPercentageFilter)
                        .WhereIf(input.MaxAllocationPercentageFilter != null, e => e.AllocationPercentage <= input.MaxAllocationPercentageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TermsFilter), e => e.Terms.ToLower() == input.TermsFilter.ToLower().Trim())
                        .WhereIf(input.MinUnitRentalRateFilter != null, e => e.UnitRentalRate >= input.MinUnitRentalRateFilter)
                        .WhereIf(input.MaxUnitRentalRateFilter != null, e => e.UnitRentalRate <= input.MaxUnitRentalRateFilter)
                        .WhereIf(input.MinUnitDepositRateFilter != null, e => e.UnitDepositRate >= input.MinUnitDepositRateFilter)
                        .WhereIf(input.MaxUnitDepositRateFilter != null, e => e.UnitDepositRate <= input.MaxUnitDepositRateFilter)
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)

                        .WhereIf(input.MinRentalUomRefIdFilter != null, e => e.RentalUomRefId >= input.MinRentalUomRefIdFilter)
                        .WhereIf(input.MaxRentalUomRefIdFilter != null, e => e.RentalUomRefId <= input.MaxRentalUomRefIdFilter)
                        .WhereIf(input.MinDepositUomRefIdFilter != null, e => e.DepositUomRefId >= input.MinDepositUomRefIdFilter)
                        .WhereIf(input.MaxDepositUomRefIdFilter != null, e => e.DepositUomRefId <= input.MaxDepositUomRefIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemFilter), e => e.Item.ToLower() == input.ItemFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementTitleFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Title.ToLower() == input.LeaseAgreementTitleFilter.ToLower().Trim());

                var pagedAndFilteredLeaseItems = filteredLeaseItems
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var leaseItems = from o in pagedAndFilteredLeaseItems
                                 join o1 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                                 from s2 in j2.DefaultIfEmpty()

                                 join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                                 from s3 in j3.DefaultIfEmpty()

                                 join o4 in _lookup_uomRepository.GetAll() on o.DepositUomRefId equals o4.Id into j4
                                 from s4 in j4.DefaultIfEmpty()

                                 join o5 in _lookup_uomRepository.GetAll() on o.RentalUomRefId equals o5.Id into j5
                                 from s5 in j5.DefaultIfEmpty()

                                 select new GetLeaseItemForViewDto()
                                 {
                                     LeaseItem = new LeaseItemDto
                                     {
                                         DateAllocated = o.DateAllocated,
                                         AllocationPercentage = o.AllocationPercentage,
                                         Terms = o.Terms,
                                         UnitRentalRate = o.UnitRentalRate,
                                         UnitDepositRate = o.UnitDepositRate,
                                         StartDate = o.StartDate,
                                         EndDate = o.EndDate,

                                         RentalUomRefId = o.RentalUomRefId,
                                         DepositUomRefId = o.DepositUomRefId,
                                         Item = o.Item,
                                         Description = o.Description,
                                         Id = o.Id
                                     },
                                     AssetClassClass = s1 == null ? "" : s1.Class.ToString(),
                                     AssetReference = s2 == null ? "" : s2.Reference.ToString(),
                                     LeaseAgreementTitle = s3 == null ? "" : s3.Title.ToString() ,
                                     DepositUom = s4 == null ? "" : s4.UnitOfMeasurement,
                                     RentalUom = s5 == null ? "" : s5.UnitOfMeasurement
                                 };

                var totalCount = await filteredLeaseItems.CountAsync();

                return new PagedResultDto<GetLeaseItemForViewDto>(
                    totalCount,
                    await leaseItems.ToListAsync()
                );
            }
        }

        public async Task<GetLeaseItemForViewDto> GetLeaseItemForView(int id)
        {
            var leaseItem = await _leaseItemRepository.GetAsync(id);

            var output = new GetLeaseItemForViewDto { LeaseItem = ObjectMapper.Map<LeaseItemDto>(leaseItem) };

            if (output.LeaseItem.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.LeaseItem.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.LeaseItem.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.LeaseItem.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }
            
            if (output.LeaseItem.DepositUomRefId != null)
            {
                var _lookupDepositUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.LeaseItem.DepositUomRefId);
                output.DepositUom = _lookupDepositUom.UnitOfMeasurement.ToString();
            }

            if (output.LeaseItem.RentalUomRefId != null)
            {
                var _lookupRentalUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.LeaseItem.RentalUomRefId);
                output.RentalUom = _lookupRentalUom.UnitOfMeasurement.ToString();
            }
            
            if (output.LeaseItem.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.LeaseItem.LeaseAgreementId);
                output.LeaseAgreementTitle = _lookupLeaseAgreement.Title.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsEdit)]
        public async Task<GetLeaseItemForEditOutput> GetLeaseItemForEdit(EntityDto input)
        {
            var leaseItem = await _leaseItemRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetLeaseItemForEditOutput { LeaseItem = ObjectMapper.Map<CreateOrEditLeaseItemDto>(leaseItem) };

            if (output.LeaseItem.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.LeaseItem.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.LeaseItem.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.LeaseItem.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }
            
            if (output.LeaseItem.DepositUomRefId != null)
            {
                var _lookupDepositUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.LeaseItem.DepositUomRefId);
                output.DepositUom = _lookupDepositUom.UnitOfMeasurement.ToString();
            }

            if (output.LeaseItem.RentalUomRefId != null)
            {
                var _lookupRentalUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.LeaseItem.RentalUomRefId);
                output.RentalUom = _lookupRentalUom.UnitOfMeasurement.ToString();
            }
            
            if (output.LeaseItem.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.LeaseItem.LeaseAgreementId);
                output.LeaseAgreementTitle = _lookupLeaseAgreement.Title.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditLeaseItemDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsCreate)]
        protected virtual async Task Create(CreateOrEditLeaseItemDto input)
        {
            var leaseItem = ObjectMapper.Map<LeaseItem>(input);


            if (AbpSession.TenantId != null)
            {
                leaseItem.TenantId = (int?)AbpSession.TenantId;
            }


            await _leaseItemRepository.InsertAsync(leaseItem);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsEdit)]
        protected virtual async Task Update(CreateOrEditLeaseItemDto input)
        {
            var leaseItem = await _leaseItemRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, leaseItem);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_LeaseItemsDelete)]
        public async Task Delete(EntityDto input)
        {
            await _leaseItemRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetLeaseItemsToExcel(GetAllLeaseItemsForExcelInput input)
        {

            var filteredLeaseItems = _leaseItemRepository.GetAll()
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.AssetFk)
                        .Include(e => e.LeaseAgreementFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Terms.Contains(input.Filter) || e.Item.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(input.MinDateAllocatedFilter != null, e => e.DateAllocated >= input.MinDateAllocatedFilter)
                        .WhereIf(input.MaxDateAllocatedFilter != null, e => e.DateAllocated <= input.MaxDateAllocatedFilter)
                        .WhereIf(input.MinAllocationPercentageFilter != null, e => e.AllocationPercentage >= input.MinAllocationPercentageFilter)
                        .WhereIf(input.MaxAllocationPercentageFilter != null, e => e.AllocationPercentage <= input.MaxAllocationPercentageFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TermsFilter), e => e.Terms.ToLower() == input.TermsFilter.ToLower().Trim())
                        .WhereIf(input.MinUnitRentalRateFilter != null, e => e.UnitRentalRate >= input.MinUnitRentalRateFilter)
                        .WhereIf(input.MaxUnitRentalRateFilter != null, e => e.UnitRentalRate <= input.MaxUnitRentalRateFilter)
                        .WhereIf(input.MinUnitDepositRateFilter != null, e => e.UnitDepositRate >= input.MinUnitDepositRateFilter)
                        .WhereIf(input.MaxUnitDepositRateFilter != null, e => e.UnitDepositRate <= input.MaxUnitDepositRateFilter)
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)

                        .WhereIf(input.MinRentalUomRefIdFilter != null, e => e.RentalUomRefId >= input.MinRentalUomRefIdFilter)
                        .WhereIf(input.MaxRentalUomRefIdFilter != null, e => e.RentalUomRefId <= input.MaxRentalUomRefIdFilter)
                        .WhereIf(input.MinDepositUomRefIdFilter != null, e => e.DepositUomRefId >= input.MinDepositUomRefIdFilter)
                        .WhereIf(input.MaxDepositUomRefIdFilter != null, e => e.DepositUomRefId <= input.MaxDepositUomRefIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemFilter), e => e.Item.ToLower() == input.ItemFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementTitleFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Title.ToLower() == input.LeaseAgreementTitleFilter.ToLower().Trim());

            var query = (from o in filteredLeaseItems
                         join o1 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetLeaseItemForViewDto()
                         {
                             LeaseItem = new LeaseItemDto
                             {
                                 DateAllocated = o.DateAllocated,
                                 AllocationPercentage = o.AllocationPercentage,
                                 Terms = o.Terms,
                                 UnitRentalRate = o.UnitRentalRate,
                                 UnitDepositRate = o.UnitDepositRate,
                                 StartDate = o.StartDate,
                                 EndDate = o.EndDate,

                                 RentalUomRefId = o.RentalUomRefId,
                                 DepositUomRefId = o.DepositUomRefId,
                                 Item = o.Item,
                                 Description = o.Description,
                                 Id = o.Id
                             },
                             AssetClassClass = s1 == null ? "" : s1.Class.ToString(),
                             AssetReference = s2 == null ? "" : s2.Reference.ToString(),
                             LeaseAgreementTitle = s3 == null ? "" : s3.Title.ToString()
                         });


            var leaseItemListDtos = await query.ToListAsync();

            return _leaseItemsExcelExporter.ExportToFile(leaseItemListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseItemAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetClassRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Class.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetClassList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<LeaseItemAssetClassLookupTableDto>();
            foreach (var assetClass in assetClassList)
            {
                lookupTableDtoList.Add(new LeaseItemAssetClassLookupTableDto
                {
                    Id = assetClass.Id,
                    DisplayName = assetClass.Class?.ToString()
                });
            }

            return new PagedResultDto<LeaseItemAssetClassLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseItemAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
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

            var lookupTableDtoList = new List<LeaseItemAssetLookupTableDto>();
            foreach (var asset in assetList)
            {
                lookupTableDtoList.Add(new LeaseItemAssetLookupTableDto
                {
                    Id = asset.Id,
                    DisplayName = asset.Reference?.ToString()
                });
            }

            return new PagedResultDto<LeaseItemAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseItemLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_leaseAgreementRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Title.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var leaseAgreementList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<LeaseItemLeaseAgreementLookupTableDto>();
            foreach (var leaseAgreement in leaseAgreementList)
            {
                lookupTableDtoList.Add(new LeaseItemLeaseAgreementLookupTableDto
                {
                    Id = leaseAgreement.Id,
                    DisplayName = leaseAgreement.Title?.ToString()
                });
            }

            return new PagedResultDto<LeaseItemLeaseAgreementLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
        
        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseItemUomLookupTableDto>> GetAllUomForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_uomRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.UnitOfMeasurement.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var uomList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<LeaseItemUomLookupTableDto>();
            foreach (var uom in uomList)
            {
                lookupTableDtoList.Add(new LeaseItemUomLookupTableDto
                {
                    Id = uom.Id,
                    UnitOfMeasurement = uom.UnitOfMeasurement?.ToString()
                });
            }

            return new PagedResultDto<LeaseItemUomLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
        
    }
}