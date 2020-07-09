using Ems.Assets;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Telematics.Exporting;
using Ems.Telematics.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Support;
using Ems.Metrics;
using System;

namespace Ems.Telematics
{
    [AbpAuthorize(AppPermissions.Pages_Main_UsageMetrics)]
    public class UsageMetricsAppService : EmsAppServiceBase, IUsageMetricsAppService
    {
        private readonly string _entityType = "UsageMetric";

        private readonly IRepository<UsageMetric> _usageMetricRepository;
        private readonly IUsageMetricsExcelExporter _usageMetricsExcelExporter;
        private readonly IRepository<LeaseItem, int> _lookup_leaseItemRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<UsageMetricRecord, int> _lookup_usageMetricRecordRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;

        public UsageMetricsAppService
            (
                IRepository<UsageMetric> usageMetricRepository, 
                IUsageMetricsExcelExporter usageMetricsExcelExporter, 
                IRepository<LeaseItem, int> lookup_leaseItemRepository, 
                IRepository<Asset, int> lookup_assetRepository, 
                IRepository<WorkOrder, int> lookup_workOrderRepository, 
                IRepository<Uom, int> lookup_uomRepository, 
                IRepository<UsageMetricRecord, int> lookup_usageMetricRecordRepository
            )
        {
            _usageMetricRepository = usageMetricRepository;
            _usageMetricsExcelExporter = usageMetricsExcelExporter;
            _lookup_leaseItemRepository = lookup_leaseItemRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_usageMetricRecordRepository = lookup_usageMetricRecordRepository;
        }

        public async Task<PagedResultDto<GetUsageMetricForViewDto>> GetAll(GetAllUsageMetricsInput input)
        {

            var filteredUsageMetrics = _usageMetricRepository.GetAll()
                        .Include(e => e.UomFk)
                        .Include(e => e.LeaseItemFk)
                        .Include(e => e.AssetFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Metric.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MetricFilter), e => e.Metric.ToLower() == input.MetricFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.LeaseItemItemFilter.ToLower().Trim());

            var pagedAndFilteredUsageMetrics = filteredUsageMetrics
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var usageMetrics = from o in pagedAndFilteredUsageMetrics
                               join o1 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                               from s2 in j2.DefaultIfEmpty()

                               join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                               from s3 in j3.DefaultIfEmpty()

                               select new GetUsageMetricForViewDto()
                               {
                                   UsageMetric = new UsageMetricDto
                                   {
                                       Metric = o.Metric,
                                       Description = o.Description,
                                       Id = o.Id,
                                       LastModificationTime = o.LastModificationTime
                                   },
                                   LeaseItemItem = s1 == null ? "" : s1.Description + (!string.IsNullOrWhiteSpace(s1.Item) ? $" ({s1.Item})" : ""),
                                   UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement.ToString(),
                                   AssetReference = s2 == null ? "" : s2.Reference
                               };

            var totalCount = await filteredUsageMetrics.CountAsync();

            return new PagedResultDto<GetUsageMetricForViewDto>(
                totalCount,
                await usageMetrics.ToListAsync()
            );
        }

        public async Task<GetUsageMetricForViewDto> GetUsageMetricForView(int id)
        {
            var usageMetric = await _usageMetricRepository.GetAsync(id);

            var output = new GetUsageMetricForViewDto { UsageMetric = ObjectMapper.Map<UsageMetricDto>(usageMetric) };

            if (output.UsageMetric.LeaseItemId != null)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.UsageMetric.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Description + (!string.IsNullOrWhiteSpace(_lookupLeaseItem.Item) ? $" ({_lookupLeaseItem.Item})" : "");
            }

            if (output.UsageMetric.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.UsageMetric.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.UsageMetric.AssetId > 0)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync(output.UsageMetric.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetrics_Edit)]
        public async Task<GetUsageMetricForEditOutput> GetUsageMetricForEdit(EntityDto input)
        {
            var usageMetric = await _usageMetricRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUsageMetricForEditOutput { UsageMetric = ObjectMapper.Map<CreateOrEditUsageMetricDto>(usageMetric) };

            if (output.UsageMetric.LeaseItemId != null)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.UsageMetric.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Description + (!string.IsNullOrWhiteSpace(_lookupLeaseItem.Item) ? $" ({_lookupLeaseItem.Item})" : "");
            }

            if (output.UsageMetric.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.UsageMetric.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.UsageMetric.AssetId > 0)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync(output.UsageMetric.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUsageMetricDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetrics_Create)]
        protected virtual async Task Create(CreateOrEditUsageMetricDto input)
        {
            var usageMetric = ObjectMapper.Map<UsageMetric>(input);

            if (AbpSession.TenantId != null)
            {
                usageMetric.TenantId = (int?)AbpSession.TenantId;
            }

            await _usageMetricRepository.InsertAsync(usageMetric);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetrics_Edit)]
        protected virtual async Task Update(CreateOrEditUsageMetricDto input)
        {
            var usageMetric = await _usageMetricRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, usageMetric);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetrics_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _usageMetricRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetUsageMetricsToExcel(GetAllUsageMetricsForExcelInput input)
        {
            var filteredUsageMetrics = _usageMetricRepository.GetAll()
                        .Include(e => e.UomFk)
                        .Include(e => e.LeaseItemFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Metric.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MetricFilter), e => e.Metric.ToLower() == input.MetricFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim());

            var query = (from o in filteredUsageMetrics
                         join o1 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetUsageMetricForViewDto()
                         {
                             UsageMetric = new UsageMetricDto
                             {
                                 Metric = o.Metric,
                                 Description = o.Description,
                                 Id = o.Id
                             },
                             LeaseItemItem = s1 == null ? "" : s1.Description + (!string.IsNullOrWhiteSpace(s1.Item) ? $"({s1.Item})" : ""),
                             UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement.ToString(),
                             AssetReference = s2 == null ? "" : s2.Reference
                         });


            var usageMetricListDtos = await query.ToListAsync();

            return _usageMetricsExcelExporter.ExportToFile(usageMetricListDtos);
        }



        public async Task<PagedResultDto<UsageMetricLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_leaseItemRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Item.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var leaseItemList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<UsageMetricLeaseItemLookupTableDto>();
            foreach (var leaseItem in leaseItemList)
            {
                lookupTableDtoList.Add(new UsageMetricLeaseItemLookupTableDto
                {
                    Id = leaseItem.Id,
                    DisplayName = leaseItem.Description + (!string.IsNullOrWhiteSpace(leaseItem.Item) ? $" ({leaseItem.Item})" : "")
                });
            }

            return new PagedResultDto<UsageMetricLeaseItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<UsageMetricAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
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

            var lookupTableDtoList = new List<UsageMetricAssetLookupTableDto>();
            foreach (var asset in assetList)
            {
                lookupTableDtoList.Add(new UsageMetricAssetLookupTableDto
                {
                    Id = asset.Id,
                    DisplayName = asset.Reference
                });
            }

            return new PagedResultDto<UsageMetricAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<GetUsageMetricForViewDto>> GetSome(GetSomeUsageMetricsInput input)
        {
            int assetId = 0;
            DateTime? workOrderStartDate = null;

            if (input.RelatedEntity == "Asset")
                assetId = input.ReferenceId;

            if (input.RelatedEntity == "WorkOrder")
            {
                var workOrderInfo = _lookup_workOrderRepository.GetAll()
                        .Include(e => e.AssetOwnershipFk)
                        .Where(e => e.Id == input.ReferenceId).FirstOrDefault();

                if (workOrderInfo != null)
                {
                    assetId = workOrderInfo.AssetOwnershipFk.AssetId ?? 0;
                    workOrderStartDate = workOrderInfo.StartDate;
                }
            }


            var filteredUsageMetrics = _usageMetricRepository.GetAll()
                    .Include(e => e.UomFk)
                    .Include(e => e.LeaseItemFk)
                    .Include(e => e.AssetFk)
                    .Where(e => e.AssetId == assetId);

            var pagedAndFilteredUsageMetrics = filteredUsageMetrics
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var usageMetrics = from o in pagedAndFilteredUsageMetrics
                               join o1 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o1.Id into j1
                               from s1 in j1.DefaultIfEmpty()

                               join o2 in _lookup_assetRepository.GetAll() on o.AssetId equals o2.Id into j2
                               from s2 in j2.DefaultIfEmpty()

                               join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                               from s3 in j3.DefaultIfEmpty()

                               select new GetUsageMetricForViewDto()
                               {
                                   UsageMetric = new UsageMetricDto
                                   {
                                       Metric = o.Metric,
                                       Description = o.Description,
                                       Id = o.Id,
                                       LastModificationTime = o.LastModificationTime
                                   },
                                   LeaseItemItem = s1 == null ? "" : s1.Description + (!string.IsNullOrWhiteSpace(s1.Item) ? $" ({s1.Item})" : ""),
                                   UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement.ToString(),
                                   AssetReference = s2 == null ? "" : s2.Reference,
                                   NeedRecordUpdate = (input.RelatedEntity == "WorkOrder" && workOrderStartDate != null) ? ((_lookup_usageMetricRecordRepository.GetAll().Where(w => w.UsageMetricId == o.Id).Count() > 0) ? (_lookup_usageMetricRecordRepository.GetAll().Where(w => w.UsageMetricId == o.Id && (w.CreationTime > workOrderStartDate || w.LastModificationTime > workOrderStartDate)).Count() == 0) : false) : false
                               };

            var totalCount = await filteredUsageMetrics.CountAsync();

            return new PagedResultDto<GetUsageMetricForViewDto>(
                totalCount,
                await usageMetrics.ToListAsync()
            );
        }
    }
}