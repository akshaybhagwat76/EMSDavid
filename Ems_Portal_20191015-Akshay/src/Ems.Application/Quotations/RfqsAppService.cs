using Ems.Quotations;
using Ems.Assets;
using Ems.Customers;
using Ems.Support;
using Ems.Vendors;
using Ems.Authorization.Users;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Quotations.Exporting;
using Ems.Quotations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Quotations
{
	[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
    public class RfqsAppService : EmsAppServiceBase, IRfqsAppService
    {
		 private readonly IRepository<Rfq> _rfqRepository;
		 private readonly IRfqsExcelExporter _rfqsExcelExporter;
		 private readonly IRepository<RfqType,int> _lookup_rfqTypeRepository;
		 private readonly IRepository<AssetOwner,int> _lookup_assetOwnerRepository;
		 private readonly IRepository<Customer,int> _lookup_customerRepository;
		 private readonly IRepository<AssetClass,int> _lookup_assetClassRepository;
		 private readonly IRepository<Incident,int> _lookup_incidentRepository;
		 private readonly IRepository<Vendor,int> _lookup_vendorRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public RfqsAppService(IRepository<Rfq> rfqRepository, IRfqsExcelExporter rfqsExcelExporter , IRepository<RfqType, int> lookup_rfqTypeRepository, IRepository<AssetOwner, int> lookup_assetOwnerRepository, IRepository<Customer, int> lookup_customerRepository, IRepository<AssetClass, int> lookup_assetClassRepository, IRepository<Incident, int> lookup_incidentRepository, IRepository<Vendor, int> lookup_vendorRepository, IRepository<User, long> lookup_userRepository) 
		  {
			_rfqRepository = rfqRepository;
			_rfqsExcelExporter = rfqsExcelExporter;
			_lookup_rfqTypeRepository = lookup_rfqTypeRepository;
		_lookup_assetOwnerRepository = lookup_assetOwnerRepository;
		_lookup_customerRepository = lookup_customerRepository;
		_lookup_assetClassRepository = lookup_assetClassRepository;
		_lookup_incidentRepository = lookup_incidentRepository;
		_lookup_vendorRepository = lookup_vendorRepository;
		_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetRfqForViewDto>> GetAll(GetAllRfqsInput input)
         {
			
			var filteredRfqs = _rfqRepository.GetAll()
						.Include( e => e.RfqTypeFk)
						.Include( e => e.AssetOwnerFk)
						.Include( e => e.CustomerFk)
						.Include( e => e.AssetClassFk)
						.Include( e => e.IncidentFk)
						.Include( e => e.VendorFk)
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Requirements.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter),  e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
						.WhereIf(input.MinRequestDateFilter != null, e => e.RequestDate >= input.MinRequestDateFilter)
						.WhereIf(input.MaxRequestDateFilter != null, e => e.RequestDate <= input.MaxRequestDateFilter)
						.WhereIf(input.MinRequiredByFilter != null, e => e.RequiredBy >= input.MinRequiredByFilter)
						.WhereIf(input.MaxRequiredByFilter != null, e => e.RequiredBy <= input.MaxRequiredByFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.RequirementsFilter),  e => e.Requirements.ToLower() == input.RequirementsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.RfqTypeTypeFilter), e => e.RfqTypeFk != null && e.RfqTypeFk.Type.ToLower() == input.RfqTypeTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim());

			var pagedAndFilteredRfqs = filteredRfqs
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var rfqs = from o in pagedAndFilteredRfqs
                         join o1 in _lookup_rfqTypeRepository.GetAll() on o.RfqTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_customerRepository.GetAll() on o.CustomerId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_vendorRepository.GetAll() on o.VendorId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         join o7 in _lookup_userRepository.GetAll() on o.UserId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()
                         
                         select new GetRfqForViewDto() {
							Rfq = new RfqDto
							{
                                Title = o.Title,
                                RequestDate = o.RequestDate,
                                RequiredBy = o.RequiredBy,
                                Description = o.Description,
                                Requirements = o.Requirements,
                                Id = o.Id
							},
                         	RfqTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	AssetOwnerName = s2 == null ? "" : s2.Name.ToString(),
                         	CustomerName = s3 == null ? "" : s3.Name.ToString(),
                         	AssetClassClass = s4 == null ? "" : s4.Class.ToString(),
                         	IncidentDescription = s5 == null ? "" : s5.Description.ToString(),
                         	VendorName = s6 == null ? "" : s6.Name.ToString(),
                         	UserName = s7 == null ? "" : s7.Name.ToString()
						};

            var totalCount = await filteredRfqs.CountAsync();

            return new PagedResultDto<GetRfqForViewDto>(
                totalCount,
                await rfqs.ToListAsync()
            );
         }
		 
		 public async Task<GetRfqForViewDto> GetRfqForView(int id)
         {
            var rfq = await _rfqRepository.GetAsync(id);

            var output = new GetRfqForViewDto { Rfq = ObjectMapper.Map<RfqDto>(rfq) };

		    if (output.Rfq != null)
            {
                var _lookupRfqType = await _lookup_rfqTypeRepository.FirstOrDefaultAsync((int)output.Rfq.RfqTypeId);
                output.RfqTypeType = _lookupRfqType.Type.ToString();
            }

		    if (output.Rfq.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.Rfq.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

		    if (output.Rfq.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Rfq.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

		    if (output.Rfq.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.Rfq.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

		    if (output.Rfq.IncidentId != null)
            {
                var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.Rfq.IncidentId);
                output.IncidentDescription = _lookupIncident.Description.ToString();
            }

		    if (output.Rfq != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.Rfq.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

		    if (output.Rfq.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Rfq.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_Rfqs_Edit)]
		 public async Task<GetRfqForEditOutput> GetRfqForEdit(EntityDto input)
         {
            var rfq = await _rfqRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetRfqForEditOutput {Rfq = ObjectMapper.Map<CreateOrEditRfqDto>(rfq)};

		    if (output.Rfq != null)
            {
                var _lookupRfqType = await _lookup_rfqTypeRepository.FirstOrDefaultAsync((int)output.Rfq.RfqTypeId);
                output.RfqTypeType = _lookupRfqType.Type.ToString();
            }

		    if (output.Rfq.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.Rfq.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

		    if (output.Rfq.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Rfq.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

		    if (output.Rfq.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.Rfq.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

		    if (output.Rfq.IncidentId != null)
            {
                var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.Rfq.IncidentId);
                output.IncidentDescription = _lookupIncident.Description.ToString();
            }

		    if (output.Rfq != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.Rfq.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

		    if (output.Rfq.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Rfq.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditRfqDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Rfqs_Create)]
		 protected virtual async Task Create(CreateOrEditRfqDto input)
         {
            var rfq = ObjectMapper.Map<Rfq>(input);

			
			if (AbpSession.TenantId != null)
			{
				rfq.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _rfqRepository.InsertAsync(rfq);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Rfqs_Edit)]
		 protected virtual async Task Update(CreateOrEditRfqDto input)
         {
            var rfq = await _rfqRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, rfq);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Rfqs_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _rfqRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetRfqsToExcel(GetAllRfqsForExcelInput input)
         {
			
			var filteredRfqs = _rfqRepository.GetAll()
						.Include( e => e.RfqTypeFk)
						.Include( e => e.AssetOwnerFk)
						.Include( e => e.CustomerFk)
						.Include( e => e.AssetClassFk)
						.Include( e => e.IncidentFk)
						.Include( e => e.VendorFk)
						.Include( e => e.UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Requirements.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter),  e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
						.WhereIf(input.MinRequestDateFilter != null, e => e.RequestDate >= input.MinRequestDateFilter)
						.WhereIf(input.MaxRequestDateFilter != null, e => e.RequestDate <= input.MaxRequestDateFilter)
						.WhereIf(input.MinRequiredByFilter != null, e => e.RequiredBy >= input.MinRequiredByFilter)
						.WhereIf(input.MaxRequiredByFilter != null, e => e.RequiredBy <= input.MaxRequiredByFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.RequirementsFilter),  e => e.Requirements.ToLower() == input.RequirementsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.RfqTypeTypeFilter), e => e.RfqTypeFk != null && e.RfqTypeFk.Type.ToLower() == input.RfqTypeTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim());

			var query = (from o in filteredRfqs
                         join o1 in _lookup_rfqTypeRepository.GetAll() on o.RfqTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_customerRepository.GetAll() on o.CustomerId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_vendorRepository.GetAll() on o.VendorId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         join o7 in _lookup_userRepository.GetAll() on o.UserId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()
                         
                         select new GetRfqForViewDto() { 
							Rfq = new RfqDto
							{
                                Title = o.Title,
                                RequestDate = o.RequestDate,
                                RequiredBy = o.RequiredBy,
                                Description = o.Description,
                                Requirements = o.Requirements,
                                Id = o.Id
							},
                         	RfqTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	AssetOwnerName = s2 == null ? "" : s2.Name.ToString(),
                         	CustomerName = s3 == null ? "" : s3.Name.ToString(),
                         	AssetClassClass = s4 == null ? "" : s4.Class.ToString(),
                         	IncidentDescription = s5 == null ? "" : s5.Description.ToString(),
                         	VendorName = s6 == null ? "" : s6.Name.ToString(),
                         	UserName = s7 == null ? "" : s7.Name.ToString()
						 });


            var rfqListDtos = await query.ToListAsync();

            return _rfqsExcelExporter.ExportToFile(rfqListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqRfqTypeLookupTableDto>> GetAllRfqTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_rfqTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var rfqTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqRfqTypeLookupTableDto>();
			foreach(var rfqType in rfqTypeList){
				lookupTableDtoList.Add(new RfqRfqTypeLookupTableDto
				{
					Id = rfqType.Id,
					DisplayName = rfqType.Type?.ToString()
				});
			}

            return new PagedResultDto<RfqRfqTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetOwnerRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetOwnerList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqAssetOwnerLookupTableDto>();
			foreach(var assetOwner in assetOwnerList){
				lookupTableDtoList.Add(new RfqAssetOwnerLookupTableDto
				{
					Id = assetOwner.Id,
					DisplayName = assetOwner.Name?.ToString()
				});
			}

            return new PagedResultDto<RfqAssetOwnerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_customerRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqCustomerLookupTableDto>();
			foreach(var customer in customerList){
				lookupTableDtoList.Add(new RfqCustomerLookupTableDto
				{
					Id = customer.Id,
					DisplayName = customer.Name?.ToString()
				});
			}

            return new PagedResultDto<RfqCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetClassRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Class.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetClassList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqAssetClassLookupTableDto>();
			foreach(var assetClass in assetClassList){
				lookupTableDtoList.Add(new RfqAssetClassLookupTableDto
				{
					Id = assetClass.Id,
					DisplayName = assetClass.Class?.ToString()
				});
			}

            return new PagedResultDto<RfqAssetClassLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_incidentRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Description.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var incidentList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqIncidentLookupTableDto>();
			foreach(var incident in incidentList){
				lookupTableDtoList.Add(new RfqIncidentLookupTableDto
				{
					Id = incident.Id,
					DisplayName = incident.Description?.ToString()
				});
			}

            return new PagedResultDto<RfqIncidentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_vendorRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqVendorLookupTableDto>();
			foreach(var vendor in vendorList){
				lookupTableDtoList.Add(new RfqVendorLookupTableDto
				{
					Id = vendor.Id,
					DisplayName = vendor.Name?.ToString()
				});
			}

            return new PagedResultDto<RfqVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Rfqs)]
         public async Task<PagedResultDto<RfqUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_userRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<RfqUserLookupTableDto>();
			foreach(var user in userList){
				lookupTableDtoList.Add(new RfqUserLookupTableDto
				{
					Id = user.Id,
					DisplayName = user.Name?.ToString()
				});
			}

            return new PagedResultDto<RfqUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}