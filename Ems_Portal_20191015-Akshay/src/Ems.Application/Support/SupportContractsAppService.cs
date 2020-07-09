using Ems.Vendors;
using Ems.Assets;
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
    public class SupportContractsAppService : EmsAppServiceBase, ISupportContractsAppService
    {
        private readonly IRepository<SupportContract> _supportContractRepository;
        private readonly ISupportContractsExcelExporter _supportContractsExcelExporter;
        private readonly IRepository<Vendor, int> _lookup_vendorRepository;
        private readonly IRepository<AssetOwner, int> _lookup_assetOwnerRepository;


        public SupportContractsAppService(IRepository<SupportContract> supportContractRepository, ISupportContractsExcelExporter supportContractsExcelExporter, IRepository<Vendor, int> lookup_vendorRepository, IRepository<AssetOwner, int> lookup_assetOwnerRepository)
        {
            _supportContractRepository = supportContractRepository;
            _supportContractsExcelExporter = supportContractsExcelExporter;
            _lookup_vendorRepository = lookup_vendorRepository;
            _lookup_assetOwnerRepository = lookup_assetOwnerRepository;

        }

        public async Task<PagedResultDto<GetSupportContractForViewDto>> GetAll(GetAllSupportContractsInput input)
        {
            var filteredSupportContracts = _supportContractRepository.GetAll()
                        .Include(e => e.VendorFk)
                        .Include(e => e.AssetOwnerFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Title.Contains(input.Filter) || e.Reference.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.AcknowledgedBy.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)

                        .WhereIf(input.IsRFQTemplateFilter > -1, e => Convert.ToInt32(e.IsRFQTemplate) == input.IsRFQTemplateFilter)
                        .WhereIf(input.IsAcknowledgedFilter > -1, e => Convert.ToInt32(e.IsAcknowledged) == input.IsAcknowledgedFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AcknowledgedByFilter), e => e.AcknowledgedBy.ToLower() == input.AcknowledgedByFilter.ToLower().Trim())
                        .WhereIf(input.MinAcknowledgedAtFilter != null, e => e.AcknowledgedAt >= input.MinAcknowledgedAtFilter)
                        .WhereIf(input.MaxAcknowledgedAtFilter != null, e => e.AcknowledgedAt <= input.MaxAcknowledgedAtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim());

            var pagedAndFilteredSupportContracts = filteredSupportContracts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var supportContracts = from o in pagedAndFilteredSupportContracts
                                   join o1 in _lookup_vendorRepository.GetAll() on o.VendorId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                                   from s2 in j2.DefaultIfEmpty()

                                   select new GetSupportContractForViewDto()
                                   {
                                       SupportContract = new SupportContractDto
                                       {
                                           Title = o.Title,
                                           Reference = o.Reference,
                                           Description = o.Description,
                                           StartDate = o.StartDate,
                                           EndDate = o.EndDate,

                                           IsRFQTemplate = o.IsRFQTemplate,
                                           IsAcknowledged = o.IsAcknowledged,
                                           AcknowledgedBy = o.AcknowledgedBy,
                                           AcknowledgedAt = o.AcknowledgedAt,
                                           Id = o.Id
                                       },
                                       VendorName = s1 == null ? "" : s1.Name.ToString(),
                                       AssetOwnerName = s2 == null ? "" : s2.Name.ToString()
                                   };

            var totalCount = await filteredSupportContracts.CountAsync();

            return new PagedResultDto<GetSupportContractForViewDto>(
                totalCount,
                await supportContracts.ToListAsync()
            );
        }

        public async Task<GetSupportContractForViewDto> GetSupportContractForView(int id)
        {
            var supportContract = await _supportContractRepository.GetAsync(id);

            var output = new GetSupportContractForViewDto { SupportContract = ObjectMapper.Map<SupportContractDto>(supportContract) };

            if (output.SupportContract.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.SupportContract.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

            if (output.SupportContract.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.SupportContract.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_Edit)]
        public async Task<GetSupportContractForEditOutput> GetSupportContractForEdit(EntityDto input)
        {
            var supportContract = await _supportContractRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetSupportContractForEditOutput { SupportContract = ObjectMapper.Map<CreateOrEditSupportContractDto>(supportContract) };

            if (output.SupportContract.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.SupportContract.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

            if (output.SupportContract.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.SupportContract.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditSupportContractDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_Create)]
        protected virtual async Task Create(CreateOrEditSupportContractDto input)
        {
            var supportContract = ObjectMapper.Map<SupportContract>(input);


            if (AbpSession.TenantId != null)
            {
                supportContract.TenantId = (int?)AbpSession.TenantId;
            }


            await _supportContractRepository.InsertAsync(supportContract);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_Edit)]
        protected virtual async Task Update(CreateOrEditSupportContractDto input)
        {
            var supportContract = await _supportContractRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, supportContract);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _supportContractRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetSupportContractsToExcel(GetAllSupportContractsForExcelInput input)
        {

            var filteredSupportContracts = _supportContractRepository.GetAll()
                        .Include(e => e.VendorFk)
                        .Include(e => e.AssetOwnerFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Title.Contains(input.Filter) || e.Reference.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.AcknowledgedBy.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)

                        .WhereIf(input.IsRFQTemplateFilter > -1, e => Convert.ToInt32(e.IsRFQTemplate) == input.IsRFQTemplateFilter)
                        .WhereIf(input.IsAcknowledgedFilter > -1, e => Convert.ToInt32(e.IsAcknowledged) == input.IsAcknowledgedFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AcknowledgedByFilter), e => e.AcknowledgedBy.ToLower() == input.AcknowledgedByFilter.ToLower().Trim())
                        .WhereIf(input.MinAcknowledgedAtFilter != null, e => e.AcknowledgedAt >= input.MinAcknowledgedAtFilter)
                        .WhereIf(input.MaxAcknowledgedAtFilter != null, e => e.AcknowledgedAt <= input.MaxAcknowledgedAtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim());

            var query = (from o in filteredSupportContracts
                         join o1 in _lookup_vendorRepository.GetAll() on o.VendorId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         select new GetSupportContractForViewDto()
                         {
                             SupportContract = new SupportContractDto
                             {
                                 Title = o.Title,
                                 Reference = o.Reference,
                                 Description = o.Description,
                                 StartDate = o.StartDate,
                                 EndDate = o.EndDate,

                                 IsRFQTemplate = o.IsRFQTemplate,
                                 IsAcknowledged = o.IsAcknowledged,
                                 AcknowledgedBy = o.AcknowledgedBy,
                                 AcknowledgedAt = o.AcknowledgedAt,
                                 Id = o.Id
                             },
                             VendorName = s1 == null ? "" : s1.Name.ToString(),
                             AssetOwnerName = s2 == null ? "" : s2.Name.ToString()
                         });


            var supportContractListDtos = await query.ToListAsync();

            return _supportContractsExcelExporter.ExportToFile(supportContractListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportContractVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_vendorRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportContractVendorLookupTableDto>();
            foreach (var vendor in vendorList)
            {
                lookupTableDtoList.Add(new SupportContractVendorLookupTableDto
                {
                    Id = vendor.Id,
                    DisplayName = vendor.Name?.ToString()
                });
            }

            return new PagedResultDto<SupportContractVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_SupportContracts)]
        public async Task<PagedResultDto<SupportContractAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetOwnerRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetOwnerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<SupportContractAssetOwnerLookupTableDto>();
            foreach (var assetOwner in assetOwnerList)
            {
                lookupTableDtoList.Add(new SupportContractAssetOwnerLookupTableDto
                {
                    Id = assetOwner.Id,
                    DisplayName = assetOwner.Name?.ToString()
                });
            }

            return new PagedResultDto<SupportContractAssetOwnerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}