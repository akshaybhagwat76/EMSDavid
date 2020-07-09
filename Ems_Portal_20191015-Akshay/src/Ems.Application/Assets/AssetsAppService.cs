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
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Organizations;
using Abp.Domain.Uow;
using Ems.Metrics;
using Ems.Support;
using Ems.Telematics;
using Ems.Vendors;
using Ems.Authorization.Users;
using Ems.Customers;

namespace Ems.Assets
{
    [AbpAuthorize(AppPermissions.Pages_Main_Assets)]
    public class AssetsAppService : EmsAppServiceBase, IAssetsAppService
    {
        private readonly string _entityType = "Asset";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IRepository<Asset> _assetRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;
        private readonly IRepository<AssetOwner> _assetOwnerRepository;
        private readonly IAssetsExcelExporter _assetsExcelExporter;
        private readonly IRepository<AssetClass, int> _lookup_assetClassRepository;
        private readonly IRepository<AssetStatus, int> _lookup_assetStatusRepository;
        private readonly IRepository<Location, int> _lookup_locationRepository;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<LeaseAgreement, int> _lookup_leaseAgreementRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<SupportItem> _supportItemRepository;
        private readonly IRepository<SupportContract, int> _lookup_supportContractRepository;
        private readonly IRepository<ConsumableType, int> _lookup_consumableTypeRepository;
        private readonly IRepository<SupportType, int> _lookup_supportTypeRepository;
        private readonly IRepository<UsageMetric, int> _lookup_usageMetricRepository;
        private readonly IRepository<WorkOrderPriority, int> _lookup_workOrderPriorityRepository;
        private readonly IRepository<WorkOrderType, int> _lookup_workOrderTypeRepository;
        private readonly IRepository<Vendor, int> _lookup_vendorRepository;
        private readonly IRepository<Incident, int> _lookup_incidentRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;
        private readonly IRepository<WorkOrderStatus, int> _lookup_workOrderStatusRepository;

        public AssetsAppService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<WorkOrder> workOrderRepository,
            IRepository<Asset> assetRepository,
            IRepository<AssetOwner> assetOwnerRepository,
            IRepository<AssetOwnership> assetOwnershipRepository,
            IAssetsExcelExporter assetsExcelExporter,
            IRepository<LeaseItem> leaseItemRepository,
            IRepository<AssetClass, int> lookup_assetClassRepository,
            IRepository<AssetStatus, int> lookup_assetStatusRepository,
            IRepository<Location, int> lookup_locationRepository,
            IRepository<LeaseAgreement, int> lookup_leaseAgreementRepository,
            IRepository<Uom, int> lookup_uomRepository,
            IRepository<SupportItem> supportItemRepository,
            IRepository<SupportContract, int> lookup_supportContractRepository,
            IRepository<ConsumableType, int> lookup_consumableTypeRepository,
            IRepository<SupportType, int> lookup_supportTypeRepository,
            IRepository<UsageMetric, int> lookup_usageMetricRepository,
            IRepository<WorkOrderPriority, int> lookup_workOrderPriorityRepository,
            IRepository<WorkOrderType, int> lookup_workOrderTypeRepository,
            IRepository<Vendor, int> lookup_vendorRepository,
            IRepository<Incident, int> lookup_incidentRepository,
            IRepository<User, long> lookup_userRepository,
            IRepository<Customer, int> lookup_customerRepository,
            IRepository<WorkOrderStatus, int> lookup_workOrderStatusRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _workOrderRepository = workOrderRepository;
            _assetRepository = assetRepository;
            _assetOwnerRepository = assetOwnerRepository;
            _assetOwnershipRepository = assetOwnershipRepository;
            _assetsExcelExporter = assetsExcelExporter;
            _leaseItemRepository = leaseItemRepository;
            _lookup_assetClassRepository = lookup_assetClassRepository;
            _lookup_assetStatusRepository = lookup_assetStatusRepository;
            _lookup_locationRepository = lookup_locationRepository;
            _lookup_leaseAgreementRepository = lookup_leaseAgreementRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _supportItemRepository = supportItemRepository;
            _lookup_supportContractRepository = lookup_supportContractRepository;
            _lookup_consumableTypeRepository = lookup_consumableTypeRepository;
            _lookup_supportTypeRepository = lookup_supportTypeRepository;
            _lookup_usageMetricRepository = lookup_usageMetricRepository;
            _lookup_workOrderPriorityRepository = lookup_workOrderPriorityRepository;
            _lookup_workOrderTypeRepository = lookup_workOrderTypeRepository;
            _lookup_vendorRepository = lookup_vendorRepository;
            _lookup_incidentRepository = lookup_incidentRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_customerRepository = lookup_customerRepository;
            _lookup_workOrderStatusRepository = lookup_workOrderStatusRepository;
        }

        public async Task<PagedResultDto<GetAssetForViewDto>> GetAll(GetAllAssetsInput input)
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

            var filteredAssets = _assetRepository.GetAll()
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.AssetStatusFk)
                        .WhereIf(tenantInfo.Tenant.TenantType == "A", e => myAssetIds.Contains(e.Id)) // Get all my Assets
                        .WhereIf(tenantInfo.Tenant.Id != 0 && tenantInfo.Tenant.TenantType != "A" && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.VehicleRegistrationNo.Contains(input.Filter) || e.Location.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.EngineNo.Contains(input.Filter) || e.ChassisNo.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.PurchaseOrderNo.Contains(input.Filter) || e.AssetLoc8GUID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VehicleRegistrationNoFilter), e => e.VehicleRegistrationNo.ToLower() == input.VehicleRegistrationNoFilter.ToLower().Trim())
                        .WhereIf(input.IsExternalAssetFilter > -1, e => Convert.ToInt32(e.IsExternalAsset) == input.IsExternalAssetFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LocationFilter), e => e.Location.ToLower() == input.LocationFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter), e => e.SerialNumber.ToLower() == input.SerialNumberFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EngineNoFilter), e => e.EngineNo.ToLower() == input.EngineNoFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ChassisNoFilter), e => e.ChassisNo.ToLower() == input.ChassisNoFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PurchaseOrderNoFilter), e => e.PurchaseOrderNo.ToLower() == input.PurchaseOrderNoFilter.ToLower().Trim())
                        .WhereIf(input.MinPurchaseDateFilter != null, e => e.PurchaseDate >= input.MinPurchaseDateFilter)
                        .WhereIf(input.MaxPurchaseDateFilter != null, e => e.PurchaseDate <= input.MaxPurchaseDateFilter)
                        .WhereIf(input.MinPurchaseCostFilter != null, e => e.PurchaseCost >= input.MinPurchaseCostFilter)
                        .WhereIf(input.MaxPurchaseCostFilter != null, e => e.PurchaseCost <= input.MaxPurchaseCostFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetLoc8GUIDFilter), e => e.AssetLoc8GUID.ToLower() == input.AssetLoc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetStatusStatusFilter), e => e.AssetStatusFk != null && e.AssetStatusFk.Status.ToLower() == input.AssetStatusStatusFilter.ToLower().Trim());

            var pagedAndFilteredAssets = filteredAssets
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var assets = from o in pagedAndFilteredAssets
                         join o1 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetStatusRepository.GetAll() on o.AssetStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetAssetForViewDto()
                         {
                             Asset = new AssetDto
                             {
                                 Reference = o.Reference,
                                 VehicleRegistrationNo = o.VehicleRegistrationNo,
                                 IsExternalAsset = o.IsExternalAsset,
                                 Location = o.Location,
                                 SerialNumber = o.SerialNumber,
                                 EngineNo = o.EngineNo,
                                 ChassisNo = o.ChassisNo,
                                 Description = o.Description,
                                 PurchaseOrderNo = o.PurchaseOrderNo,
                                 PurchaseDate = o.PurchaseDate,
                                 PurchaseCost = o.PurchaseCost,
                                 AssetLoc8GUID = o.AssetLoc8GUID,

                                 Id = o.Id
                             },
                             AssetClassClass = s1 == null ? "" : s1.Class.ToString(),
                             AssetStatusStatus = s2 == null ? "" : s2.Status.ToString()
                         };

            var totalCount = await filteredAssets.CountAsync();

            return new PagedResultDto<GetAssetForViewDto>(
                totalCount,
                await assets.ToListAsync()
            );
        }

        public async Task<List<GetAssetsWithWorkordersForViewDto>> GetAssetsWithWorkorders(GetAssetsWithWorkordersInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

            var workOrders = _workOrderRepository.GetAll()
                                .Where(w => w.AssetOwnershipFk != null)
                                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                .Include(w => w.AssetOwnershipFk.AssetFk)
                                .Where(w => !w.IsDeleted)
                                .Where(w => w.StartDate >= DateTime.Now.AddDays(-input.Days))
                                .ToList();

            var assetsWithWorkOrders =
                    from w in workOrders
                    group w by w.AssetOwnershipFk.AssetFk into a
                    select new GetAssetsWithWorkordersForViewDto()
                    {
                        Asset = new AssetDto
                        {
                            Reference = a.Key.Reference,
                            Description = a.Key.Description
                        },
                        Workorders = a.Count()
                    };

            var result = assetsWithWorkOrders.OrderByDescending(a => a.Workorders).Take(10).ToList();

            return result;
        }

        public async Task<GetAssetForViewDto> GetAssetForView(int id)
        {
            var asset = await _assetRepository.GetAsync(id);

            var output = new GetAssetForViewDto { Asset = ObjectMapper.Map<AssetDto>(asset) };

            if (output.Asset.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.Asset.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.Asset.AssetStatusId != null)
            {
                var _lookupAssetStatus = await _lookup_assetStatusRepository.FirstOrDefaultAsync((int)output.Asset.AssetStatusId);
                output.AssetStatusStatus = _lookupAssetStatus.Status.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_Edit)]
        public async Task<GetAssetForEditOutput> GetAssetForEdit(EntityDto input)
        {
            var asset = await _assetRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetAssetForEditOutput { Asset = ObjectMapper.Map<CreateOrEditAssetDto>(asset) };

            if (output.Asset.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.Asset.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.Asset.AssetStatusId != null)
            {
                var _lookupAssetStatus = await _lookup_assetStatusRepository.FirstOrDefaultAsync((int)output.Asset.AssetStatusId);
                output.AssetStatusStatus = _lookupAssetStatus.Status.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditAssetDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_Create)]
        protected virtual async Task Create(CreateOrEditAssetDto input)
        {
            var asset = ObjectMapper.Map<Asset>(input);

            if (AbpSession.TenantId != null)
            {
                asset.TenantId = (int?)AbpSession.TenantId;
            }

            await _assetRepository.InsertAsync(asset);
            await CurrentUnitOfWork.SaveChangesAsync();

            CreateLocation(input.Location);

            // Create an AssetOwnership

            var tenantInfo = TenantManager.GetTenantInfo().Result;
            int assetOwnerId = 0;
            int tenantId = 0;
            AssetOwner assetOwner = new AssetOwner();

            switch (tenantInfo.Tenant.TenantType)
            {
                case "H":
                    // Host is creating this Asset, so just assign it to the first (probably only) AssetOwner
                    assetOwner = _assetOwnerRepository.GetAllListAsync().Result.FirstOrDefault();
                    assetOwnerId = assetOwner.Id;
                    tenantId = (int)assetOwner.TenantId;
                    break;
                case "A":
                    assetOwnerId = tenantInfo.AssetOwner.Id;
                    tenantId = tenantInfo.Tenant.Id;
                    break;
                default:
                    throw new Exception($"This Tenant ({tenantInfo.Tenant.Name}) is not entitled to create an Asset!");
            }

            if (assetOwnerId != 0)
            {
                AssetOwnership assetOwnership = new AssetOwnership()
                {
                    AssetId = asset.Id,
                    AssetOwnerId = assetOwnerId,
                    FinishDate = DateTime.Now.AddYears(50),
                    StartDate = DateTime.Now,
                    PercentageOwnership = 100,
                    TenantId = tenantId
                };

                await _assetOwnershipRepository.InsertAsync(assetOwnership);
            }
            else
            {
                throw new Exception($"Couldn't figure out the assetOwnerId");
            }

        }

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_Edit)]
        protected virtual async Task Update(CreateOrEditAssetDto input)
        {
            var asset = await _assetRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, asset);

            CreateLocation(input.Location);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _assetRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetAssetsToExcel(GetAllAssetsForExcelInput input)
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

            var filteredAssets = _assetRepository.GetAll()
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.AssetStatusFk)
                        .WhereIf(tenantInfo.Tenant.TenantType == "A", e => myAssetIds.Contains(e.Id)) // Get all my Assets
                        .WhereIf(tenantInfo.Tenant.Id != 0 && tenantInfo.Tenant.TenantType != "A" && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.VehicleRegistrationNo.Contains(input.Filter) || e.Location.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.EngineNo.Contains(input.Filter) || e.ChassisNo.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.PurchaseOrderNo.Contains(input.Filter) || e.AssetLoc8GUID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VehicleRegistrationNoFilter), e => e.VehicleRegistrationNo.ToLower() == input.VehicleRegistrationNoFilter.ToLower().Trim())
                        .WhereIf(input.IsExternalAssetFilter > -1, e => Convert.ToInt32(e.IsExternalAsset) == input.IsExternalAssetFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LocationFilter), e => e.Location.ToLower() == input.LocationFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter), e => e.SerialNumber.ToLower() == input.SerialNumberFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EngineNoFilter), e => e.EngineNo.ToLower() == input.EngineNoFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ChassisNoFilter), e => e.ChassisNo.ToLower() == input.ChassisNoFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PurchaseOrderNoFilter), e => e.PurchaseOrderNo.ToLower() == input.PurchaseOrderNoFilter.ToLower().Trim())
                        .WhereIf(input.MinPurchaseDateFilter != null, e => e.PurchaseDate >= input.MinPurchaseDateFilter)
                        .WhereIf(input.MaxPurchaseDateFilter != null, e => e.PurchaseDate <= input.MaxPurchaseDateFilter)
                        .WhereIf(input.MinPurchaseCostFilter != null, e => e.PurchaseCost >= input.MinPurchaseCostFilter)
                        .WhereIf(input.MaxPurchaseCostFilter != null, e => e.PurchaseCost <= input.MaxPurchaseCostFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetLoc8GUIDFilter), e => e.AssetLoc8GUID.ToLower() == input.AssetLoc8GUIDFilter.ToLower().Trim())

                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetStatusStatusFilter), e => e.AssetStatusFk != null && e.AssetStatusFk.Status.ToLower() == input.AssetStatusStatusFilter.ToLower().Trim());

            var query = (from o in filteredAssets
                         join o1 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetStatusRepository.GetAll() on o.AssetStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetAssetForViewDto()
                         {
                             Asset = new AssetDto
                             {
                                 Reference = o.Reference,
                                 VehicleRegistrationNo = o.VehicleRegistrationNo,
                                 IsExternalAsset = o.IsExternalAsset,
                                 Location = o.Location,
                                 SerialNumber = o.SerialNumber,
                                 EngineNo = o.EngineNo,
                                 ChassisNo = o.ChassisNo,
                                 Description = o.Description,
                                 PurchaseOrderNo = o.PurchaseOrderNo,
                                 PurchaseDate = o.PurchaseDate,
                                 PurchaseCost = o.PurchaseCost,
                                 AssetLoc8GUID = o.AssetLoc8GUID,

                                 Id = o.Id
                             },
                             AssetClassClass = s1 == null ? "" : s1.Class.ToString(),
                             AssetStatusStatus = s2 == null ? "" : s2.Status.ToString()
                         });


            var assetListDtos = await query.ToListAsync();

            return _assetsExcelExporter.ExportToFile(assetListDtos);
        }

        protected async void CreateLocation(string location)
        {
            var query = _lookup_locationRepository.GetAll().Where(e => e.LocationName.Trim() == location.Trim()).FirstOrDefault();
            if (query == null)
            {
                Location loc = new Location()
                {
                    LocationName = location,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId != null ? AbpSession.TenantId : 0
                };

                await _lookup_locationRepository.InsertAsync(loc);
            }
        }

        public async Task<PagedResultDto<AssetAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetClassRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Class.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetClassList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetAssetClassLookupTableDto>();
            foreach (var assetClass in assetClassList)
            {
                lookupTableDtoList.Add(new AssetAssetClassLookupTableDto
                {
                    Id = assetClass.Id,
                    DisplayName = assetClass.Class?.ToString()
                });
            }

            return new PagedResultDto<AssetAssetClassLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<AssetAssetStatusLookupTableDto>> GetAllAssetStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<AssetAssetStatusLookupTableDto>();
            foreach (var assetStatus in assetStatusList)
            {
                lookupTableDtoList.Add(new AssetAssetStatusLookupTableDto
                {
                    Id = assetStatus.Id,
                    DisplayName = assetStatus.Status?.ToString()
                });
            }

            return new PagedResultDto<AssetAssetStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<Organizations.Dtos.LocationLookupTableDto>> GetAllLocationForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_locationRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.LocationName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var locationList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<Organizations.Dtos.LocationLookupTableDto>();
            foreach (var loc in locationList)
            {
                lookupTableDtoList.Add(new Organizations.Dtos.LocationLookupTableDto
                {
                    Id = loc.Id,
                    DisplayName = loc.LocationName
                });
            }

            return new PagedResultDto<Organizations.Dtos.LocationLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<GetLeaseItemForViewDto>> GetAllLeaseItems(int assetId, PagedAndSortedResultRequestDto input)
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
                        .Where(e => e.AssetId == assetId);

                var pagedAndFilteredLeaseItems = filteredLeaseItems
                    .OrderBy(input.Sorting ?? "id desc")
                    .PageBy(input);

                var leaseItems = from o in pagedAndFilteredLeaseItems
                                 join o1 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _assetRepository.GetAll() on o.AssetId equals o2.Id into j2
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
                                     LeaseAgreementTitle = s3 == null ? "" : s3.Title.ToString(),
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

        public async Task<PagedResultDto<Support.Dtos.GetSupportItemForViewDto>> GetAllSupportItems(int assetId, PagedAndSortedResultRequestDto input)
        {
            var filteredSupportItems = _supportItemRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.AssetClassFk)
                        .Include(e => e.UomFk)
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.ConsumableTypeFk)
                        .Include(e => e.SupportTypeFk)
                        .Where(e => e.AssetId == assetId);


            var pagedAndFilteredSupportItems = filteredSupportItems
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);

            var supportItems = from o in pagedAndFilteredSupportItems
                               join o1 in _assetRepository.GetAll() on o.AssetId equals o1.Id into j1
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

                               select new Support.Dtos.GetSupportItemForViewDto()
                               {
                                   SupportItem = new Support.Dtos.SupportItemDto
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

            return new PagedResultDto<Support.Dtos.GetSupportItemForViewDto>(
                totalCount,
                await supportItems.ToListAsync()
            );
        }

        public async Task<PagedResultDto<Support.Dtos.GetWorkOrderForViewDto>> GetAllWorkOrders(int assetId, PagedAndSortedResultRequestDto input)
        {
            var filteredWorkOrders = _workOrderRepository.GetAll()
                            .Include(e => e.WorkOrderPriorityFk)
                            .Include(e => e.WorkOrderTypeFk)
                            .Include(e => e.VendorFk)
                            .Include(e => e.IncidentFk)
                            .Include(e => e.SupportItemFk)
                            .Include(e => e.UserFk)
                            .Include(e => e.CustomerFk)
                            .Include(e => e.AssetOwnershipFk)
                            .Include(e => e.WorkOrderStatusFk)
                            .Where(e => e.AssetOwnershipFk != null && e.AssetOwnershipFk.AssetFk.Id == assetId);

            var pagedAndFilteredWorkOrders = filteredWorkOrders
                    .OrderBy(input.Sorting ?? "id desc")
                    .PageBy(input);

            var workOrders = from o in pagedAndFilteredWorkOrders
                             join o1 in _lookup_workOrderPriorityRepository.GetAll() on o.WorkOrderPriorityId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             join o2 in _lookup_workOrderTypeRepository.GetAll() on o.WorkOrderTypeId equals o2.Id into j2
                             from s2 in j2.DefaultIfEmpty()

                             join o3 in _lookup_vendorRepository.GetAll() on o.VendorId equals o3.Id into j3
                             from s3 in j3.DefaultIfEmpty()

                             join o4 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o4.Id into j4
                             from s4 in j4.DefaultIfEmpty()

                             join o5 in _supportItemRepository.GetAll() on o.SupportItemId equals o5.Id into j5
                             from s5 in j5.DefaultIfEmpty()

                             join o6 in _lookup_userRepository.GetAll() on o.UserId equals o6.Id into j6
                             from s6 in j6.DefaultIfEmpty()

                             join o7 in _lookup_customerRepository.GetAll() on o.CustomerId equals o7.Id into j7
                             from s7 in j7.DefaultIfEmpty()

                             join o8 in _assetOwnershipRepository.GetAll() on o.AssetOwnershipId equals o8.Id into j8
                             from s8 in j8.DefaultIfEmpty()

                             join o9 in _lookup_workOrderStatusRepository.GetAll() on o.WorkOrderStatusId equals o9.Id into j9
                             from s9 in j9.DefaultIfEmpty()

                             select new Support.Dtos.GetWorkOrderForViewDto()
                             {
                                 WorkOrder = new Support.Dtos.WorkOrderDto
                                 {
                                     Loc8GUID = o.Loc8GUID,
                                     Subject = o.Subject,
                                     Description = o.Description,
                                     Location = o.Location,
                                     StartDate = o.StartDate,
                                     EndDate = o.EndDate,
                                     Remarks = o.Remarks,
                                     Attachments = o.Attachments,
                                     Id = o.Id,
                                     AssetOwnershipId = o.AssetOwnershipId,
                                     CustomerId = o.CustomerId,
                                     IncidentId = o.IncidentId,
                                     SupportItemId = o.SupportItemId,
                                     UserId = o.UserId,
                                     VendorId = o.VendorId,
                                     WorkOrderPriorityId = o.WorkOrderPriorityId,
                                     WorkOrderStatusId = o.WorkOrderStatusId,
                                     WorkOrderTypeId = o.WorkOrderTypeId
                                 },
                                 WorkOrderPriorityPriority = s1 == null ? "" : s1.Priority.ToString(),
                                 WorkOrderTypeType = s2 == null ? "" : s2.Type.ToString(),
                                 VendorName = s3 == null ? "" : s3.Name.ToString(),
                                 IncidentDescription = s4 == null ? "" : s4.Description.ToString(),
                                 SupportItemDescription = s5 == null ? "" : s5.Description.ToString(),
                                 UserName = s6 == null ? "" : s6.Name.ToString(),
                                 CustomerName = s7 == null ? "" : s7.Name.ToString(),
                                 AssetOwnershipAssetDisplayName = s8 == null ? "" : s8.AssetFk.Reference.ToString(),
                                 WorkOrderStatusStatus = s9 == null ? "" : s9.Status.ToString()
                             };

            var totalCount = await filteredWorkOrders.CountAsync();

            return new PagedResultDto<Support.Dtos.GetWorkOrderForViewDto>(
                totalCount,
                await workOrders.ToListAsync()
            );
        }

    }
}