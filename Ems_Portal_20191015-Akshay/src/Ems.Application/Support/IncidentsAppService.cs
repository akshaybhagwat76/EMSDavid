using Ems.Support;
using Ems.Customers;
using Ems.Assets;
using Ems.Authorization.Users;


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
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using Ems.Organizations;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
    public class IncidentsAppService : EmsAppServiceBase, IIncidentsAppService
    {
        private readonly string _entityType = "Incident";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<Incident> _incidentRepository;
        private readonly IIncidentsExcelExporter _incidentsExcelExporter;
        private readonly IRepository<IncidentPriority, int> _lookup_incidentPriorityRepository;
        private readonly IRepository<IncidentStatus, int> _lookup_incidentStatusRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<SupportItem, int> _lookup_supportItemRepository;
        private readonly IRepository<IncidentType, int> _lookup_incidentTypeRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<AssetOwnership, int> _lookup_assetOwnershipRepository;
        private readonly IRepository<IncidentUpdate> _incidentUpdateRepository;
        private readonly IRepository<Incident, int> _lookup_incidentRepository;
        private readonly IRepository<Location, int> _lookup_locationRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;

        public IncidentsAppService
            (
              IUnitOfWorkManager unitOfWorkManager,
              IRepository<LeaseItem> leaseItemtRepository,
              IRepository<Incident> incidentRepository,
              IRepository<IncidentUpdate> incidentUpdateRepository,
              IIncidentsExcelExporter incidentsExcelExporter,
              IRepository<IncidentPriority, int> lookup_incidentPriorityRepository,
              IRepository<IncidentStatus, int> lookup_incidentStatusRepository,
              IRepository<Customer, int> lookup_customerRepository,
              IRepository<Asset, int> lookup_assetRepository,
              IRepository<SupportItem, int> lookup_supportItemRepository,
              IRepository<IncidentType, int> lookup_incidentTypeRepository,
              IRepository<User, long> lookup_userRepository,
              IRepository<AssetOwnership, int> lookup_assetOwnershipRepository,
              IRepository<Incident, int> lookup_incidentRepository,
              IRepository<AssetOwnership> assetOwnershipRepository,
              IRepository<Location, int> lookup_locationRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _leaseItemRepository = leaseItemtRepository;
            _incidentRepository = incidentRepository;
            _incidentUpdateRepository = incidentUpdateRepository;
            _incidentsExcelExporter = incidentsExcelExporter;
            _lookup_incidentPriorityRepository = lookup_incidentPriorityRepository;
            _lookup_incidentStatusRepository = lookup_incidentStatusRepository;
            _lookup_customerRepository = lookup_customerRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_supportItemRepository = lookup_supportItemRepository;
            _lookup_incidentTypeRepository = lookup_incidentTypeRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_assetOwnershipRepository = lookup_assetOwnershipRepository;
            _lookup_incidentRepository = lookup_incidentRepository;
            _lookup_locationRepository = lookup_locationRepository;
            _assetOwnershipRepository = assetOwnershipRepository;
        }

        public async Task<PagedResultDto<GetIncidentForViewDto>> GetAll(GetAllIncidentsInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

                var filteredIncidents = _incidentRepository.GetAll()
                        .Include(e => e.IncidentPriorityFk)
                        .Include(e => e.IncidentStatusFk)
                        .Include(e => e.CustomerFk)
                        .Include(e => e.AssetFk)
                        .Include(e => e.SupportItemFk)
                        .Include(e => e.IncidentTypeFk)
                        .Include(e => e.UserFk)
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter) || e.Location.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinIncidentDateFilter != null, e => e.IncidentDate >= input.MinIncidentDateFilter)
                        .WhereIf(input.MaxIncidentDateFilter != null, e => e.IncidentDate <= input.MaxIncidentDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LocationFilter), e => e.Location.ToLower() == input.LocationFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks.ToLower() == input.RemarksFilter.ToLower().Trim())
                        .WhereIf(input.MinResolvedAtFilter != null, e => e.ResolvedAt >= input.MinResolvedAtFilter)
                        .WhereIf(input.MaxResolvedAtFilter != null, e => e.ResolvedAt <= input.MaxResolvedAtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentPriorityPriorityFilter), e => e.IncidentPriorityFk != null && e.IncidentPriorityFk.Priority.ToLower() == input.IncidentPriorityPriorityFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentStatusStatusFilter), e => e.IncidentStatusFk != null && e.IncidentStatusFk.Status.ToLower() == input.IncidentStatusStatusFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportItemDescriptionFilter), e => e.SupportItemFk != null && e.SupportItemFk.Description.ToLower() == input.SupportItemDescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentTypeTypeFilter), e => e.IncidentTypeFk != null && e.IncidentTypeFk.Type.ToLower() == input.IncidentTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim());

                var pagedAndFilteredIncidents = filteredIncidents
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);

                var incidents = from o in pagedAndFilteredIncidents
                                join o1 in _lookup_incidentPriorityRepository.GetAll() on o.IncidentPriorityId equals o1.Id into j1
                                from s1 in j1.DefaultIfEmpty()

                                join o2 in _lookup_incidentStatusRepository.GetAll() on o.IncidentStatusId equals o2.Id into j2
                                from s2 in j2.DefaultIfEmpty()

                                join o3 in _lookup_customerRepository.GetAll() on o.CustomerId equals o3.Id into j3
                                from s3 in j3.DefaultIfEmpty()

                                join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                                from s4 in j4.DefaultIfEmpty()

                                join o5 in _lookup_supportItemRepository.GetAll() on o.SupportItemId equals o5.Id into j5
                                from s5 in j5.DefaultIfEmpty()

                                join o6 in _lookup_incidentTypeRepository.GetAll() on o.IncidentTypeId equals o6.Id into j6
                                from s6 in j6.DefaultIfEmpty()

                                join o7 in _lookup_userRepository.GetAll() on o.UserId equals o7.Id into j7
                                from s7 in j7.DefaultIfEmpty()

                                select new GetIncidentForViewDto()
                                {
                                    Incident = new IncidentDto
                                    {
                                        Description = o.Description,
                                        IncidentDate = o.IncidentDate,
                                        Location = o.Location,
                                        Remarks = o.Remarks,

                                        ResolvedAt = o.ResolvedAt,
                                        Id = o.Id
                                    },
                                    IncidentPriorityPriority = s1 == null ? "" : s1.Priority.ToString(),
                                    IncidentStatusStatus = s2 == null ? "" : s2.Status.ToString(),
                                    CustomerName = s3 == null ? "" : s3.Name.ToString(),
                                    AssetReference = s4 == null ? "" : s4.Reference.ToString(),
                                    SupportItemDescription = s5 == null ? "" : s5.Description.ToString(),
                                    IncidentTypeType = s6 == null ? "" : s6.Type.ToString(),
                                    UserName = s7 == null ? "" : s7.Name.ToString()
                                };

                var totalCount = await filteredIncidents.CountAsync();

                return new PagedResultDto<GetIncidentForViewDto>(
                    totalCount,
                    await incidents.ToListAsync()
                );
            }
        }

        public async Task<GetIncidentForViewDto> GetIncidentForView(int id, PagedAndSortedResultRequestDto input)
        {
            var incident = await _incidentRepository.GetAsync(id);

            var output = new GetIncidentForViewDto { Incident = ObjectMapper.Map<IncidentDto>(incident) };

            if (output?.Incident != null)
            {

                if (output.Incident.Id > 0)
                {
                    var filteredIncidentUpdates = _incidentUpdateRepository.GetAll()
                        .Include(e => e.UserFk)
                        .Include(e => e.IncidentFk)
                        .Where(e => e.IncidentId == output.Incident.Id);

                    var pagedAndFilteredIncidentUpdates = filteredIncidentUpdates
                        .OrderBy(input.Sorting ?? "id asc")
                        .PageBy(input);

                    var incidentUpdates = from o in pagedAndFilteredIncidentUpdates
                                          join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                                          from s1 in j1.DefaultIfEmpty()

                                          join o2 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o2.Id into j2
                                          from s2 in j2.DefaultIfEmpty()

                                          select new GetIncidentUpdateForViewDto()
                                          {
                                              IncidentUpdate = new IncidentUpdateDto
                                              {
                                                  Updated = o.Updated,
                                                  Update = o.Update,
                                                  Id = o.Id,
                                                  IncidentId = o.IncidentId
                                              },
                                              UserName = s1 == null ? "" : s1.Name.ToString(),
                                              IncidentDescription = s2 == null ? "" : s2.Description.ToString()
                                          };

                    var totalCount = await filteredIncidentUpdates.CountAsync();

                    output.IncidentUpdates = new PagedResultDto<GetIncidentUpdateForViewDto>(
                        totalCount,
                        await incidentUpdates.ToListAsync()
                    );

                }


                if (output.Incident.IncidentPriorityId != null)
                {
                    var _lookupIncidentPriority = await _lookup_incidentPriorityRepository.FirstOrDefaultAsync((int)output.Incident.IncidentPriorityId);
                    output.IncidentPriorityPriority = _lookupIncidentPriority.Priority.ToString();
                }

                if (output.Incident.IncidentStatusId != null)
                {
                    var _lookupIncidentStatus = await _lookup_incidentStatusRepository.FirstOrDefaultAsync((int)output.Incident.IncidentStatusId);
                    output.IncidentStatusStatus = _lookupIncidentStatus.Status.ToString();
                }

                if (output.Incident.CustomerId != null)
                {
                    var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Incident.CustomerId);
                    output.CustomerName = _lookupCustomer.Name.ToString();
                }

                if (output.Incident.AssetId != null)
                {
                    var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.Incident.AssetId);
                    output.AssetReference = _lookupAsset.Reference.ToString();
                }

                if (output.Incident.SupportItemId != null)
                {
                    var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.Incident.SupportItemId);
                    output.SupportItemDescription = _lookupSupportItem.Description.ToString();
                }

                if (output.Incident != null)
                {
                    var _lookupIncidentType = await _lookup_incidentTypeRepository.FirstOrDefaultAsync((int)output.Incident.IncidentTypeId);
                    output.IncidentTypeType = _lookupIncidentType.Type.ToString();
                }

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER
                {
                    if (output.Incident.UserId != null)
                    {
                        var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Incident.UserId);
                        output.UserName = _lookupUser.Name.ToString();
                    }
                }
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents_Edit)]
        public async Task<GetIncidentForEditOutput> GetIncidentForEdit(EntityDto input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var incident = await _incidentRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetIncidentForEditOutput { Incident = ObjectMapper.Map<CreateOrEditIncidentDto>(incident) };

            output.TenantType = tenantInfo.Tenant.TenantType;

            if (output.Incident.IncidentPriorityId != null)
            {
                var _lookupIncidentPriority = await _lookup_incidentPriorityRepository.FirstOrDefaultAsync((int)output.Incident.IncidentPriorityId);
                output.IncidentPriorityPriority = _lookupIncidentPriority.Priority.ToString();
            }

            if (output.Incident.IncidentStatusId != null)
            {
                var _lookupIncidentStatus = await _lookup_incidentStatusRepository.FirstOrDefaultAsync((int)output.Incident.IncidentStatusId);
                output.IncidentStatusStatus = _lookupIncidentStatus.Status.ToString();
            }

            if (output.Incident.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Incident.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

            if (output.Incident.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.Incident.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            if (output.Incident.SupportItemId != null)
            {
                var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.Incident.SupportItemId);
                output.SupportItemDescription = _lookupSupportItem.Description.ToString();
            }

            if (output.Incident != null)
            {
                var _lookupIncidentType = await _lookup_incidentTypeRepository.FirstOrDefaultAsync((int)output.Incident.IncidentTypeId);
                output.IncidentTypeType = _lookupIncidentType.Type.ToString();
            }

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER
            {

                if (output.Incident.UserId != null)
                {
                    var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Incident.UserId);
                    output.UserName = _lookupUser.Name.ToString();
                }
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditIncidentDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents_Create)]
        protected virtual async Task Create(CreateOrEditIncidentDto input)
        {

            //TODO: Check if sensitive related data is legal (i.e. CustomerId, AssetId, SupportItemId, UserId)

            var incident = ObjectMapper.Map<Incident>(input);

            if (AbpSession.TenantId != null)
            {
                incident.TenantId = (int?)AbpSession.TenantId;
            }

            var tenantInfo = await TenantManager.GetTenantInfo();
            if (tenantInfo.Tenant.TenantType == "C")
            {
                incident.CustomerId = tenantInfo.Customer.Id;
            }

            await _incidentRepository.InsertAsync(incident);

            CreateLocation(input.Location);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents_Edit)]
        protected virtual async Task Update(CreateOrEditIncidentDto input)
        {
            var incident = await _incidentRepository.FirstOrDefaultAsync((int)input.Id);

            ObjectMapper.Map(input, incident);

            CreateLocation(input.Location);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _incidentRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetIncidentsToExcel(GetAllIncidentsForExcelInput input)
        {

            var filteredIncidents = _incidentRepository.GetAll()
                        .Include(e => e.IncidentPriorityFk)
                        .Include(e => e.IncidentStatusFk)
                        .Include(e => e.CustomerFk)
                        .Include(e => e.AssetFk)
                        .Include(e => e.SupportItemFk)
                        .Include(e => e.IncidentTypeFk)
                        .Include(e => e.UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter) || e.Location.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinIncidentDateFilter != null, e => e.IncidentDate >= input.MinIncidentDateFilter)
                        .WhereIf(input.MaxIncidentDateFilter != null, e => e.IncidentDate <= input.MaxIncidentDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LocationFilter), e => e.Location.ToLower() == input.LocationFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks.ToLower() == input.RemarksFilter.ToLower().Trim())
                        .WhereIf(input.MinResolvedAtFilter != null, e => e.ResolvedAt >= input.MinResolvedAtFilter)
                        .WhereIf(input.MaxResolvedAtFilter != null, e => e.ResolvedAt <= input.MaxResolvedAtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentPriorityPriorityFilter), e => e.IncidentPriorityFk != null && e.IncidentPriorityFk.Priority.ToLower() == input.IncidentPriorityPriorityFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentStatusStatusFilter), e => e.IncidentStatusFk != null && e.IncidentStatusFk.Status.ToLower() == input.IncidentStatusStatusFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportItemDescriptionFilter), e => e.SupportItemFk != null && e.SupportItemFk.Description.ToLower() == input.SupportItemDescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentTypeTypeFilter), e => e.IncidentTypeFk != null && e.IncidentTypeFk.Type.ToLower() == input.IncidentTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim());

            var query = (from o in filteredIncidents
                         join o1 in _lookup_incidentPriorityRepository.GetAll() on o.IncidentPriorityId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_incidentStatusRepository.GetAll() on o.IncidentStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_customerRepository.GetAll() on o.CustomerId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_supportItemRepository.GetAll() on o.SupportItemId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         join o6 in _lookup_incidentTypeRepository.GetAll() on o.IncidentTypeId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()

                         join o7 in _lookup_userRepository.GetAll() on o.UserId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()

                         select new GetIncidentForViewDto()
                         {
                             Incident = new IncidentDto
                             {
                                 Description = o.Description,
                                 IncidentDate = o.IncidentDate,
                                 Location = o.Location,
                                 Remarks = o.Remarks,

                                 ResolvedAt = o.ResolvedAt,
                                 Id = o.Id
                             },
                             IncidentPriorityPriority = s1 == null ? "" : s1.Priority.ToString(),
                             IncidentStatusStatus = s2 == null ? "" : s2.Status.ToString(),
                             CustomerName = s3 == null ? "" : s3.Name.ToString(),
                             AssetReference = s4 == null ? "" : s4.Reference.ToString(),
                             SupportItemDescription = s5 == null ? "" : s5.Description.ToString(),
                             IncidentTypeType = s6 == null ? "" : s6.Type.ToString(),
                             UserName = s7 == null ? "" : s7.Name.ToString()
                         });


            var incidentListDtos = await query.ToListAsync();

            return _incidentsExcelExporter.ExportToFile(incidentListDtos);
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


        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentIncidentPriorityLookupTableDto>> GetAllIncidentPriorityForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_incidentPriorityRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Priority.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var incidentPriorityList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<IncidentIncidentPriorityLookupTableDto>();
            foreach (var incidentPriority in incidentPriorityList)
            {
                lookupTableDtoList.Add(new IncidentIncidentPriorityLookupTableDto
                {
                    Id = incidentPriority.Id,
                    DisplayName = incidentPriority.Priority
                });
            }

            return new PagedResultDto<IncidentIncidentPriorityLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentIncidentStatusLookupTableDto>> GetAllIncidentStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_incidentStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var incidentStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<IncidentIncidentStatusLookupTableDto>();
            foreach (var incidentStatus in incidentStatusList)
            {
                lookupTableDtoList.Add(new IncidentIncidentStatusLookupTableDto
                {
                    Id = incidentStatus.Id,
                    DisplayName = incidentStatus.Status
                });
            }

            return new PagedResultDto<IncidentIncidentStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllCustomersForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            IQueryable<Customer> query;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A": // Get all Customers related to the Asset (in most cases will 0 or 1)
                case "V": // Same for AssetOwner and Vendor

                    query = _leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == input.AssetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.LeaseAgreementFk.CustomerFk.Name.Contains(input.Filter))
                        .Select(e => e.LeaseAgreementFk.CustomerFk);
                    break;

                case "C": // Just use the Customer's Id
                    query = _lookup_customerRepository
                        .GetAll()
                        .Where(e => e.Id == tenantInfo.Customer.Id);
                    break;

                case "H": // Get Everything
                    query = _lookup_customerRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter)
               );
                    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<IncidentCustomerLookupTableDto>();
            foreach (var customer in customerList)
            {
                lookupTableDtoList.Add(new IncidentCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer.Name
                });
            }

            return new PagedResultDto<IncidentCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Asset");

                IQueryable<Asset> query;

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                        query = _leaseItemRepository
                            .GetAll()
                            .Include(e => e.AssetFk)
                            .Include(e => e.LeaseAgreementFk)
                            .Where(e => e.LeaseAgreementFk.CustomerId == tenantInfo.Customer.Id)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter))
                            .Select(s => s.AssetFk);

                        break;

                    case "V":
                        query = _lookup_supportItemRepository
                            .GetAll()
                            .Include(e => e.AssetFk)
                            .Include(e => e.SupportContractFk)
                            .Where(e => e.SupportContractFk.VendorId == tenantInfo.Vendor.Id)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter))
                            .Select(s => s.AssetFk);

                        break;

                    case "A":

                        var myAssetIds = _assetOwnershipRepository.GetAll()
                               .Where(e => e.AssetOwnerId == tenantInfo.AssetOwner.Id)
                               .Select(e => e.AssetId)
                               .ToList();

                        query = _lookup_assetRepository
                            .GetAll()
                            .Where(e => myAssetIds.Contains(e.Id)) // Get all my Assets
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Reference.Contains(input.Filter));
                        break;

                    case "H":
                        query = _lookup_assetRepository
                            .GetAll()
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Reference.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }
                
                var totalCount = await query.CountAsync();

                var assetList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<IncidentAssetLookupTableDto>();
                foreach (var asset in assetList)
                {
                    lookupTableDtoList.Add(new IncidentAssetLookupTableDto
                    {
                        Id = asset.Id,
                        DisplayName = asset.Reference
                    });
                }

                return new PagedResultDto<IncidentAssetLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentSupportItemLookupTableDto>> GetAllSupportItemForLookupTable(GetAllSupportItemsForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            IQueryable<SupportItem> query;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A": // Get all Support Items related to the selected Asset
                case "C":
                case "V":
                    query = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == input.AssetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Description.Contains(input.Filter));
                    break;

                case "H": // Get Everything
                    query = _lookup_supportItemRepository
                        .GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Description.Contains(input.Filter));
                    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var totalCount = await query.CountAsync();

            var supportItemList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<IncidentSupportItemLookupTableDto>();
            foreach (var supportItem in supportItemList)
            {
                lookupTableDtoList.Add(new IncidentSupportItemLookupTableDto
                {
                    Id = supportItem.Id,
                    DisplayName = supportItem.Description
                });
            }

            return new PagedResultDto<IncidentSupportItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentIncidentTypeLookupTableDto>> GetAllIncidentTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_incidentTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var incidentTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<IncidentIncidentTypeLookupTableDto>();
            foreach (var incidentType in incidentTypeList)
            {
                lookupTableDtoList.Add(new IncidentIncidentTypeLookupTableDto
                {
                    Id = incidentType.Id,
                    DisplayName = incidentType.Type
                });
            }

            return new PagedResultDto<IncidentIncidentTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<PagedResultDto<IncidentUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "User");

                var query = _lookup_userRepository.GetAll()
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter));

                var totalCount = await query.CountAsync();

                var userList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<IncidentUserLookupTableDto>();
                foreach (var user in userList)
                {
                    lookupTableDtoList.Add(new IncidentUserLookupTableDto
                    {
                        Id = user.Id,
                        DisplayName = String.Format("{0} [{1}]", user.Name, user.EmailAddress)
                    });
                }

                return new PagedResultDto<IncidentUserLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<IncidentSupportItemAndCustomerListDto> GetSupportItemAndCustomerList(int assetId)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            IQueryable<SupportItem> supportItemQuery;
            IQueryable<Customer> customerQuery;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A": // Get all Support Items related to the selected Asset
                case "V":
                case "H":
                    supportItemQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetId);

                    customerQuery = _leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.LeaseAgreementFk.CustomerFk);

                    break;

                case "C":
                    supportItemQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetId);

                    customerQuery = null;

                    break;

                //case "H": // Get Everything
                //    supportItemQuery = _lookup_supportItemRepository.GetAll();
                //    customerQuery = _lookup_customerRepository.GetAll();
                //    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var customerTableDtoList = new List<IncidentCustomerLookupTableDto>();
            var supportItemTableDtoList = new List<IncidentSupportItemLookupTableDto>();

            if (customerQuery?.Count() > 0)
            {
                foreach (var customer in customerQuery)
                {
                    customerTableDtoList.Add(new IncidentCustomerLookupTableDto
                    {
                        Id = customer.Id,
                        DisplayName = customer.Name?.ToString()
                    });
                }
            }

            if (supportItemQuery?.Count() > 0)
            {
                foreach (var supportItem in supportItemQuery)
                {
                    supportItemTableDtoList.Add(new IncidentSupportItemLookupTableDto
                    {
                        Id = supportItem.Id,
                        DisplayName = supportItem.Description?.ToString()
                    });
                }
            }

            return new IncidentSupportItemAndCustomerListDto { CustomerList = customerTableDtoList, SupportItemList = supportItemTableDtoList };
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
        public async Task<IncidentUserLookupTableDto> GetDefaultCreator()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.UserId.ToString());

            if(user != null)
            {
                return new IncidentUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = String.Format("{0} [{1}]", user.Name, user.EmailAddress)
                };
            }

            return null;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Incidents)]
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

    }
}