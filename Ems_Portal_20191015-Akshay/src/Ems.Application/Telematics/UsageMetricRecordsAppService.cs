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
using Ems.Authorization.Users;
using Abp.UI;
using System;

namespace Ems.Telematics
{
    [AbpAuthorize(AppPermissions.Pages_Main_UsageMetricRecords)]
    public class UsageMetricRecordsAppService : EmsAppServiceBase, IUsageMetricRecordsAppService
    {
        private readonly IRepository<UsageMetricRecord> _usageMetricRecordRepository;
        private readonly IUsageMetricRecordsExcelExporter _usageMetricRecordsExcelExporter;
        private readonly IRepository<UsageMetric, int> _lookup_usageMetricRepository;


        public UsageMetricRecordsAppService(IRepository<UsageMetricRecord> usageMetricRecordRepository, IUsageMetricRecordsExcelExporter usageMetricRecordsExcelExporter, IRepository<UsageMetric, int> lookup_usageMetricRepository)
        {
            _usageMetricRecordRepository = usageMetricRecordRepository;
            _usageMetricRecordsExcelExporter = usageMetricRecordsExcelExporter;
            _lookup_usageMetricRepository = lookup_usageMetricRepository;
        }

        public async Task<PagedResultDto<GetUsageMetricRecordForViewDto>> GetAll(GetAllUsageMetricRecordsInput input)
        {

            var filteredUsageMetricRecords = _usageMetricRecordRepository.GetAll()
                        .Include(e => e.UsageMetricFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(input.MinStartTimeFilter != null, e => e.StartTime >= input.MinStartTimeFilter)
                        .WhereIf(input.MaxStartTimeFilter != null, e => e.StartTime <= input.MaxStartTimeFilter)
                        .WhereIf(input.MinEndTimeFilter != null, e => e.EndTime >= input.MinEndTimeFilter)
                        .WhereIf(input.MaxEndTimeFilter != null, e => e.EndTime <= input.MaxEndTimeFilter)
                        .WhereIf(input.MinUnitsConsumedFilter != null, e => e.UnitsConsumed >= input.MinUnitsConsumedFilter)
                        .WhereIf(input.MaxUnitsConsumedFilter != null, e => e.UnitsConsumed <= input.MaxUnitsConsumedFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric.ToLower() == input.UsageMetricMetricFilter.ToLower().Trim());

            var pagedAndFilteredUsageMetricRecords = filteredUsageMetricRecords
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var usageMetricRecords = from o in pagedAndFilteredUsageMetricRecords

                                     join o2 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o2.Id into j2
                                     from s2 in j2.DefaultIfEmpty()

                                     select new GetUsageMetricRecordForViewDto()
                                     {
                                         UsageMetricRecord = new UsageMetricRecordDto
                                         {
                                             Reference = o.Reference,
                                             StartTime = o.StartTime,
                                             EndTime = o.EndTime,
                                             UnitsConsumed = o.UnitsConsumed,
                                             Id = o.Id,
                                             LastModificationTime = o.LastModificationTime
                                         },
                                         UsageMetricMetric = s2 == null ? "" : s2.Metric.ToString()
                                     };

            var totalCount = await filteredUsageMetricRecords.CountAsync();

            return new PagedResultDto<GetUsageMetricRecordForViewDto>(
                totalCount,
                await usageMetricRecords.ToListAsync()
            );
        }

        public async Task<GetUsageMetricRecordForViewDto> GetUsageMetricRecordForView(int id)
        {
            var usageMetricRecord = await _usageMetricRecordRepository.GetAsync(id);

            var output = new GetUsageMetricRecordForViewDto { UsageMetricRecord = ObjectMapper.Map<UsageMetricRecordDto>(usageMetricRecord) };

            if (output.UsageMetricRecord != null)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.UsageMetricRecord.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetricRecords_Edit)]
        public async Task<GetUsageMetricRecordForEditOutput> GetUsageMetricRecordForEdit(EntityDto input)
        {
            var usageMetricRecord = await _usageMetricRecordRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetUsageMetricRecordForEditOutput { UsageMetricRecord = ObjectMapper.Map<CreateOrEditUsageMetricRecordDto>(usageMetricRecord) };

            if (output.UsageMetricRecord != null)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.UsageMetricRecord.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditUsageMetricRecordDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetricRecords_Create)]
        protected virtual async Task Create(CreateOrEditUsageMetricRecordDto input)
        {
            var usageMetricRecord = ObjectMapper.Map<UsageMetricRecord>(input);

            if (AbpSession.TenantId != null)
                usageMetricRecord.TenantId = (int?)AbpSession.TenantId;

            await _usageMetricRecordRepository.InsertAsync(usageMetricRecord);
            //UpdateUsageMetric(input.UsageMetricId);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetricRecords_Edit)]
        protected virtual async Task Update(CreateOrEditUsageMetricRecordDto input)
        {
            var usageMetricRecord = await _usageMetricRecordRepository.FirstOrDefaultAsync((int)input.Id);

            if (input.UnitsConsumed >= usageMetricRecord.UnitsConsumed)
            {
                usageMetricRecord.LastModificationTime = DateTime.Now;
                ObjectMapper.Map(input, usageMetricRecord);
               // UpdateUsageMetric(input.UsageMetricId);
            }
            else
            {
                string roleName = "Admin";
                bool okayToUpdate = false;

                if (AbpSession.UserId != null && AbpSession.UserId > 0) 
                {
                    if (AbpSession.TenantId != null)
                    {
                        var tenantInfo = await TenantManager.GetTenantInfo();

                        if (tenantInfo?.Tenant.TenantType == "C")
                            roleName = "Customer Admin";
                        else if (tenantInfo?.Tenant.TenantType == "V")
                            roleName = "Vendor Admin";
                        else if (tenantInfo?.Tenant.TenantType == "A")
                            roleName = "Asset Owner Admin";
                    }

                    User userInfo = await UserManager.GetUserByIdAsync((long)AbpSession.UserId);
                    okayToUpdate = await UserManager.IsInRoleAsync(userInfo, roleName); //WHY THE FVCK is this Role check hard-coded here???
                }

                if (okayToUpdate)
                {
                    usageMetricRecord.LastModificationTime = DateTime.Now;
                    ObjectMapper.Map(input, usageMetricRecord);
                    //UpdateUsageMetric(input.UsageMetricId);
                }
                else
                    throw new UserFriendlyException("No permission", "New value must be equal to or higher than previous record");
            }
        }

        /* // NO FLIPPING IDEA WHAT THIS WAS SUPPOSED TO ACCOMPLISH //
        protected async void UpdateUsageMetric(int usageMetricId)
        {
            var usageMetricRecord = await _lookup_usageMetricRepository.FirstOrDefaultAsync(usageMetricId);
            if (usageMetricRecord != null)
            {
                usageMetricRecord.LastModificationTime = DateTime.Now;
            }
        }
        */

        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetricRecords_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _usageMetricRecordRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetUsageMetricRecordsToExcel(GetAllUsageMetricRecordsForExcelInput input)
        {

            var filteredUsageMetricRecords = _usageMetricRecordRepository.GetAll()
                        .Include(e => e.UsageMetricFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(input.MinStartTimeFilter != null, e => e.StartTime >= input.MinStartTimeFilter)
                        .WhereIf(input.MaxStartTimeFilter != null, e => e.StartTime <= input.MaxStartTimeFilter)
                        .WhereIf(input.MinEndTimeFilter != null, e => e.EndTime >= input.MinEndTimeFilter)
                        .WhereIf(input.MaxEndTimeFilter != null, e => e.EndTime <= input.MaxEndTimeFilter)
                        .WhereIf(input.MinUnitsConsumedFilter != null, e => e.UnitsConsumed >= input.MinUnitsConsumedFilter)
                        .WhereIf(input.MaxUnitsConsumedFilter != null, e => e.UnitsConsumed <= input.MaxUnitsConsumedFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric.ToLower() == input.UsageMetricMetricFilter.ToLower().Trim());

            var query = (from o in filteredUsageMetricRecords

                         join o2 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetUsageMetricRecordForViewDto()
                         {
                             UsageMetricRecord = new UsageMetricRecordDto
                             {
                                 Reference = o.Reference,
                                 StartTime = o.StartTime,
                                 EndTime = o.EndTime,
                                 UnitsConsumed = o.UnitsConsumed,
                                 Id = o.Id
                             },
                             UsageMetricMetric = s2 == null ? "" : s2.Metric.ToString()
                         });


            var usageMetricRecordListDtos = await query.ToListAsync();

            return _usageMetricRecordsExcelExporter.ExportToFile(usageMetricRecordListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_UsageMetricRecords)]
        public async Task<PagedResultDto<UsageMetricRecordUsageMetricLookupTableDto>> GetAllUsageMetricForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_usageMetricRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Metric.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var usageMetricList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<UsageMetricRecordUsageMetricLookupTableDto>();
            foreach (var usageMetric in usageMetricList)
            {
                lookupTableDtoList.Add(new UsageMetricRecordUsageMetricLookupTableDto
                {
                    Id = usageMetric.Id,
                    DisplayName = usageMetric.Metric?.ToString()
                });
            }

            return new PagedResultDto<UsageMetricRecordUsageMetricLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<GetUsageMetricRecordForViewDto>> GetAllByUsageMetric(GetAllRecordsByUsageMetricInput input)
        {
            var filteredUsageMetricRecords = _usageMetricRecordRepository.GetAll()
                        //.Include(e => e.UomFk)
                        .Include(e => e.UsageMetricFk)
                        .Where(e => e.UsageMetricId == input.UsageMetricId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(input.MinStartTimeFilter != null, e => e.StartTime >= input.MinStartTimeFilter)
                        .WhereIf(input.MaxStartTimeFilter != null, e => e.StartTime <= input.MaxStartTimeFilter)
                        .WhereIf(input.MinEndTimeFilter != null, e => e.EndTime >= input.MinEndTimeFilter)
                        .WhereIf(input.MaxEndTimeFilter != null, e => e.EndTime <= input.MaxEndTimeFilter)
                        .WhereIf(input.MinUnitsConsumedFilter != null, e => e.UnitsConsumed >= input.MinUnitsConsumedFilter)
                        .WhereIf(input.MaxUnitsConsumedFilter != null, e => e.UnitsConsumed <= input.MaxUnitsConsumedFilter)
                        //.WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric.ToLower() == input.UsageMetricMetricFilter.ToLower().Trim());

            var pagedAndFilteredUsageMetricRecords = filteredUsageMetricRecords
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var usageMetricRecords = from o in pagedAndFilteredUsageMetricRecords
                                         //join o1 in _lookup_uomRepository.GetAll() on o.UomId equals o1.Id into j1
                                         //from s1 in j1.DefaultIfEmpty()

                                     join o2 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o2.Id into j2
                                     from s2 in j2.DefaultIfEmpty()

                                     select new GetUsageMetricRecordForViewDto()
                                     {
                                         UsageMetricRecord = new UsageMetricRecordDto
                                         {
                                             Reference = o.Reference,
                                             StartTime = o.StartTime,
                                             EndTime = o.EndTime,
                                             UnitsConsumed = o.UnitsConsumed,
                                             Id = o.Id
                                         },
                                         //UomUnitOfMeasurement = s1 == null ? "" : s1.UnitOfMeasurement.ToString(),
                                         UsageMetricMetric = s2 == null ? "" : s2.Metric.ToString()
                                     };

            var totalCount = await filteredUsageMetricRecords.CountAsync();

            return new PagedResultDto<GetUsageMetricRecordForViewDto>(
                totalCount,
                await usageMetricRecords.ToListAsync()
            );
        }

        public async Task<GetUsageMetricRecordForViewDto> GetByUsageMetric(int usageMetricId)
        {
            var usageMetricRecord = _usageMetricRecordRepository.GetAll().Where(e => e.UsageMetricId == usageMetricId).FirstOrDefault();

            var output = new GetUsageMetricRecordForViewDto { UsageMetricRecord = ObjectMapper.Map<UsageMetricRecordDto>(usageMetricRecord) };

            //if (output?.UsageMetricRecord?.UomId != null)
            //{
            //    var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.UsageMetricRecord.UomId);
            //    output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            //}

            if (output?.UsageMetricRecord?.UsageMetricId > 0)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.UsageMetricRecord.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

            return output;
        }

    }
}