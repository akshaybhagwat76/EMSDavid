using Ems.Organizations;
using Ems.Assets;
using Ems.Customers;


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

namespace Ems.Assets
{
    [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
    public class LeaseAgreementsAppService : EmsAppServiceBase, ILeaseAgreementsAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<LeaseAgreement> _leaseAgreementRepository;
        private readonly ILeaseAgreementsExcelExporter _leaseAgreementsExcelExporter;
        private readonly IRepository<Contact, int> _lookup_contactRepository;
        private readonly IRepository<AssetOwner, int> _lookup_assetOwnerRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;


        public LeaseAgreementsAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<LeaseAgreement> leaseAgreementRepository, ILeaseAgreementsExcelExporter leaseAgreementsExcelExporter, IRepository<Contact, int> lookup_contactRepository, IRepository<AssetOwner, int> lookup_assetOwnerRepository, IRepository<Customer, int> lookup_customerRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _leaseAgreementRepository = leaseAgreementRepository;
            _leaseAgreementsExcelExporter = leaseAgreementsExcelExporter;
            _lookup_contactRepository = lookup_contactRepository;
            _lookup_assetOwnerRepository = lookup_assetOwnerRepository;
            _lookup_customerRepository = lookup_customerRepository;
        }

        public async Task<PagedResultDto<GetLeaseAgreementForViewDto>> GetAll(GetAllLeaseAgreementsInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to enable global access to "User" entity
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var tenantType = tenantInfo.Tenant.TenantType;

                var filteredLeaseAgreements = _leaseAgreementRepository.GetAll()
                        .Include(e => e.ContactFk)
                        .Include(e => e.AssetOwnerFk)
                        .Include(e => e.CustomerFk)
                        .WhereIf(tenantType == "A", e => e.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any LeaseAgreements that are not relevant to the AssetOwener
                        .WhereIf(tenantType == "C", e => e.CustomerId == tenantInfo.Customer.Id) // Filter out any LeaseAgreements that are not relevant to the Customer
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Title.Contains(input.Filter) || e.Terms.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TermsFilter), e => e.Terms.ToLower() == input.TermsFilter.ToLower().Trim())
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactContactNameFilter), e => e.ContactFk != null && e.ContactFk.ContactName.ToLower() == input.ContactContactNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim());

                var pagedAndFilteredLeaseAgreements = filteredLeaseAgreements
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var leaseAgreements = from o in pagedAndFilteredLeaseAgreements
                                      join o1 in _lookup_contactRepository.GetAll() on o.ContactId equals o1.Id into j1
                                      from s1 in j1.DefaultIfEmpty()

                                      join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                                      from s2 in j2.DefaultIfEmpty()

                                      join o3 in _lookup_customerRepository.GetAll() on o.CustomerId equals o3.Id into j3
                                      from s3 in j3.DefaultIfEmpty()

                                      select new GetLeaseAgreementForViewDto()
                                      {
                                          LeaseAgreement = new LeaseAgreementDto
                                          {
                                              Reference = o.Reference,
                                              Description = o.Description,
                                              Title = o.Title,
                                              Terms = o.Terms,
                                              StartDate = o.StartDate,
                                              EndDate = o.EndDate,

                                              Id = o.Id
                                          },
                                          ContactContactName = s1 == null ? "" : s1.ContactName.ToString(),
                                          AssetOwnerName = s2 == null ? "" : s2.Name.ToString(),
                                          CustomerName = s3 == null ? "" : s3.Name.ToString()
                                      };

                var totalCount = await filteredLeaseAgreements.CountAsync();

                return new PagedResultDto<GetLeaseAgreementForViewDto>(
                    totalCount,
                    await leaseAgreements.ToListAsync()
                );
            }
        }

        public async Task<GetLeaseAgreementForViewDto> GetLeaseAgreementForView(int id)
        {
            var leaseAgreement = await _leaseAgreementRepository.GetAsync(id);

            var output = new GetLeaseAgreementForViewDto { LeaseAgreement = ObjectMapper.Map<LeaseAgreementDto>(leaseAgreement) };

            if (output.LeaseAgreement.ContactId != null)
            {
                var _lookupContact = await _lookup_contactRepository.FirstOrDefaultAsync((int)output.LeaseAgreement.ContactId);
                output.ContactContactName = _lookupContact.ContactName.ToString();
            }

            if (output.LeaseAgreement.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.LeaseAgreement.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

            if (output.LeaseAgreement.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.LeaseAgreement.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_Edit)]
        public async Task<GetLeaseAgreementForEditOutput> GetLeaseAgreementForEdit(EntityDto input)
        {
            var leaseAgreement = await _leaseAgreementRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetLeaseAgreementForEditOutput { LeaseAgreement = ObjectMapper.Map<CreateOrEditLeaseAgreementDto>(leaseAgreement) };

            if (output.LeaseAgreement.ContactId != null)
            {
                var _lookupContact = await _lookup_contactRepository.FirstOrDefaultAsync((int)output.LeaseAgreement.ContactId);
                output.ContactContactName = _lookupContact.ContactName.ToString();
            }

            if (output.LeaseAgreement.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.LeaseAgreement.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

            if (output.LeaseAgreement.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.LeaseAgreement.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditLeaseAgreementDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_Create)]
        protected virtual async Task Create(CreateOrEditLeaseAgreementDto input)
        {
            var leaseAgreement = ObjectMapper.Map<LeaseAgreement>(input);


            if (AbpSession.TenantId != null)
            {
                leaseAgreement.TenantId = (int?)AbpSession.TenantId;
            }


            await _leaseAgreementRepository.InsertAsync(leaseAgreement);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_Edit)]
        protected virtual async Task Update(CreateOrEditLeaseAgreementDto input)
        {
            var leaseAgreement = await _leaseAgreementRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, leaseAgreement);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _leaseAgreementRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetLeaseAgreementsToExcel(GetAllLeaseAgreementsForExcelInput input)
        {

            var filteredLeaseAgreements = _leaseAgreementRepository.GetAll()
                        .Include(e => e.ContactFk)
                        .Include(e => e.AssetOwnerFk)
                        .Include(e => e.CustomerFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Title.Contains(input.Filter) || e.Terms.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TermsFilter), e => e.Terms.ToLower() == input.TermsFilter.ToLower().Trim())
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactContactNameFilter), e => e.ContactFk != null && e.ContactFk.ContactName.ToLower() == input.ContactContactNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim());

            var query = (from o in filteredLeaseAgreements
                         join o1 in _lookup_contactRepository.GetAll() on o.ContactId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_customerRepository.GetAll() on o.CustomerId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetLeaseAgreementForViewDto()
                         {
                             LeaseAgreement = new LeaseAgreementDto
                             {
                                 Reference = o.Reference,
                                 Description = o.Description,
                                 Title = o.Title,
                                 Terms = o.Terms,
                                 StartDate = o.StartDate,
                                 EndDate = o.EndDate,

                                 Id = o.Id
                             },
                             ContactContactName = s1 == null ? "" : s1.ContactName.ToString(),
                             AssetOwnerName = s2 == null ? "" : s2.Name.ToString(),
                             CustomerName = s3 == null ? "" : s3.Name.ToString()
                         });


            var leaseAgreementListDtos = await query.ToListAsync();

            return _leaseAgreementsExcelExporter.ExportToFile(leaseAgreementListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseAgreementContactLookupTableDto>> GetAllContactForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_contactRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.ContactName.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var contactList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<LeaseAgreementContactLookupTableDto>();
            foreach (var contact in contactList)
            {
                lookupTableDtoList.Add(new LeaseAgreementContactLookupTableDto
                {
                    Id = contact.Id,
                    DisplayName = contact.ContactName?.ToString()
                });
            }

            return new PagedResultDto<LeaseAgreementContactLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseAgreementAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Contact ID

            int assetOwnerId = 0;

            if (input.FilterId > 0)
                assetOwnerId = _lookup_contactRepository.Get(input.FilterId)?.AssetOwnerId ?? 0;

            var query = _lookup_assetOwnerRepository
                .GetAll()
                .WhereIf(assetOwnerId > 0, e=> e.Id == assetOwnerId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.ToString().Contains(input.Filter));

            var totalCount = await query.CountAsync();

            var assetOwnerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<LeaseAgreementAssetOwnerLookupTableDto>();
            foreach (var assetOwner in assetOwnerList)
            {
                lookupTableDtoList.Add(new LeaseAgreementAssetOwnerLookupTableDto
                {
                    Id = assetOwner.Id,
                    DisplayName = assetOwner.Name?.ToString()
                });
            }

            return new PagedResultDto<LeaseAgreementAssetOwnerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public async Task<PagedResultDto<LeaseAgreementCustomerLookupTableDto>> GetAllCustomerForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Contact ID

            int customerId = 0;

            if (input.FilterId > 0)
                customerId = _lookup_contactRepository.Get(input.FilterId)?.CustomerId ?? 0;

            var query = _lookup_customerRepository
                .GetAll()
                .WhereIf(customerId > 0, e=> e.Id == customerId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.ToString().Contains(input.Filter));

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<LeaseAgreementCustomerLookupTableDto>();
            foreach (var customer in customerList)
            {
                lookupTableDtoList.Add(new LeaseAgreementCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer.Name?.ToString()
                });
            }

            return new PagedResultDto<LeaseAgreementCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        [AbpAuthorize(AppPermissions.Pages_Main_LeaseAgreements)]
        public LeaseAgreementCustomerAndAssetOwnerDto GetCustomerAndAssetOwnerInfo(int contactId)
        {
            var contactInfo = _lookup_contactRepository.Get(contactId);

            if (contactInfo != null)
            {
                LeaseAgreementCustomerLookupTableDto customerInfo = new LeaseAgreementCustomerLookupTableDto();
                LeaseAgreementAssetOwnerLookupTableDto assetOwnerInfo = new LeaseAgreementAssetOwnerLookupTableDto();

                if(contactInfo.CustomerId > 0)
                {
                    var cInfo = _lookup_customerRepository.Get((int)contactInfo.CustomerId);
                    if(cInfo != null)
                    {
                        customerInfo.DisplayName = cInfo.Name;
                        customerInfo.Id = cInfo.Id;
                    }
                }

                if (contactInfo.AssetOwnerId > 0)
                {
                    var aInfo = _lookup_assetOwnerRepository.Get((int)contactInfo.AssetOwnerId);
                    if (aInfo != null)
                    {
                        assetOwnerInfo.DisplayName = aInfo.Name;
                        assetOwnerInfo.Id = aInfo.Id;
                    }
                }

                return new LeaseAgreementCustomerAndAssetOwnerDto { CustomerInfo = customerInfo, AssetOwnerInfo = assetOwnerInfo };
            }

            return null;
        }

    }
}