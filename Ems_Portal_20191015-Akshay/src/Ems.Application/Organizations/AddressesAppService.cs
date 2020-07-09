using Ems.Customers;
using Ems.Assets;
using Ems.Vendors;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;


namespace Ems.Organizations
{
	[AbpAuthorize(AppPermissions.Pages_Main_Addresses)]
    public class AddressesAppService : EmsAppServiceBase, IAddressesAppService
    {
		private readonly IRepository<Address> _addressRepository;
		private readonly IRepository<Customer,int> _lookup_customerRepository;
		private readonly IRepository<AssetOwner,int> _lookup_assetOwnerRepository;
		private readonly IRepository<Vendor,int> _lookup_vendorRepository;

        public AddressesAppService(IRepository<Address> addressRepository , IRepository<Customer, int> lookup_customerRepository, IRepository<AssetOwner, int> lookup_assetOwnerRepository, IRepository<Vendor, int> lookup_vendorRepository) 
		  {
			_addressRepository = addressRepository;
			_lookup_customerRepository = lookup_customerRepository;
		    _lookup_assetOwnerRepository = lookup_assetOwnerRepository;
		    _lookup_vendorRepository = lookup_vendorRepository;
            
        }

		 public async Task<PagedResultDto<GetAddressForViewDto>> GetAll(GetAllAddressesInput input)
         {

            var filteredAddresses = _addressRepository.GetAll()
						//.Include( e => e.CustomerFk)
						//.Include( e => e.AssetOwnerFk)
						//.Include( e => e.VendorFk)

                        .WhereIf(AbpSession.TenantId != null, e => e.TenantId == AbpSession.TenantId) // This should filter out other tenant data
                        .WhereIf(input.CustomerId != 0 && input.CustomerId != null, e => e.CustomerId == input.CustomerId)
                        .WhereIf(input.VendorId != 0 && input.VendorId != null, e => e.VendorId == input.VendorId)
                        .WhereIf(input.AssetOwnerId != 0 && input.AssetOwnerId != null, e => e.AssetOwnerId == input.AssetOwnerId)

                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.AddressEntryName.Contains(input.Filter) || e.AddressLine1.Contains(input.Filter) || e.AddressLine2.Contains(input.Filter) || e.PostalCode.Contains(input.Filter) || e.City.Contains(input.Filter) || e.State.Contains(input.Filter) || e.Country.Contains(input.Filter) || e.AddressLoc8GUID.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressEntryNameFilter),  e => e.AddressEntryName.ToLower() == input.AddressEntryNameFilter.ToLower().Trim())
						.WhereIf(input.IsHeadOfficeFilter > -1,  e => Convert.ToInt32(e.IsHeadOffice) == input.IsHeadOfficeFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLine1Filter),  e => e.AddressLine1.ToLower() == input.AddressLine1Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLine2Filter),  e => e.AddressLine2.ToLower() == input.AddressLine2Filter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.PostalCodeFilter),  e => e.PostalCode.ToLower() == input.PostalCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CityFilter),  e => e.City.ToLower() == input.CityFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.StateFilter),  e => e.State.ToLower() == input.StateFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CountryFilter),  e => e.Country.ToLower() == input.CountryFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AddressLoc8GUIDFilter),  e => e.AddressLoc8GUID.ToLower() == input.AddressLoc8GUIDFilter.ToLower().Trim())
						.WhereIf(input.IsDefaultForBillingFilter > -1,  e => Convert.ToInt32(e.IsDefaultForBilling) == input.IsDefaultForBillingFilter )
						.WhereIf(input.IsDefaultForShippingFilter > -1,  e => Convert.ToInt32(e.IsDefaultForShipping) == input.IsDefaultForShippingFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim());

			var pagedAndFilteredAddresses = filteredAddresses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var addresses = from o in pagedAndFilteredAddresses
                         join o1 in _lookup_customerRepository.GetAll() on o.CustomerId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_vendorRepository.GetAll() on o.VendorId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetAddressForViewDto() {
							Address = new AddressDto
							{
                                AddressEntryName = o.AddressEntryName,
                                IsHeadOffice = o.IsHeadOffice,
                                AddressLine1 = o.AddressLine1,
                                AddressLine2 = o.AddressLine2,
                                PostalCode = o.PostalCode,
                                City = o.City,
                                State = o.State,
                                Country = o.Country,
                                AddressLoc8GUID = o.AddressLoc8GUID,
                                IsDefaultForBilling = o.IsDefaultForBilling,
                                IsDefaultForShipping = o.IsDefaultForShipping,
                                Id = o.Id
							},
                         	CustomerName = s1 == null ? "" : s1.Name.ToString(),
                         	AssetOwnerName = s2 == null ? "" : s2.Name.ToString(),
                         	VendorName = s3 == null ? "" : s3.Name.ToString()
						};

            var totalCount = await filteredAddresses.CountAsync();

            return new PagedResultDto<GetAddressForViewDto>(
                totalCount,
                await addresses.ToListAsync()
            );
         }
		 

		 public async Task<GetAddressForViewDto> GetAddressForView(int id)
         {
            var address = await _addressRepository.GetAsync(id);

            var output = new GetAddressForViewDto { Address = ObjectMapper.Map<AddressDto>(address) };

		    if (output.Address.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Address.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

		    if (output.Address.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.Address.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

		    if (output.Address.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.Address.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }
			
            return output;
         }
		 

		 [AbpAuthorize(AppPermissions.Pages_Main_Addresses_Edit)]
		 public async Task<GetAddressForEditOutput> GetAddressForEdit(EntityDto input)
         {
            var address = await _addressRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAddressForEditOutput {Address = ObjectMapper.Map<CreateOrEditAddressDto>(address)};
            
            // TODO: REMOVE THESE - NOT NEEDED, and exposes info accross tenants!!!

		    if (output.Address.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Address.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

		    if (output.Address.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.Address.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

		    if (output.Address.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.Address.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }
			
            return output;
         }

        public async Task CreateOrEdit(CreateOrEditAddressDto input)
        {
            input.CustomerId = (input.CustomerId == 0) ? (int?)null : input.CustomerId;
            input.VendorId = (input.VendorId == 0) ? (int?)null : input.VendorId;
            input.AssetOwnerId = (input.AssetOwnerId == 0) ? (int?)null : input.AssetOwnerId;

            if (input.Id == null){
    			await Create(input);
			}
			else{
                await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Addresses_Create)]
		 protected virtual async Task Create(CreateOrEditAddressDto input)
         {
            var address = ObjectMapper.Map<Address>(input);

			if (AbpSession.TenantId != null)
			{
				address.TenantId = (int?) AbpSession.TenantId;
			}

            await _addressRepository.InsertAsync(address);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Addresses_Edit)]
		 protected virtual async Task Update(CreateOrEditAddressDto input)
         {
            /// This has been updated to filter on TenantId because this data 
            /// entity does not implement IMayHaveTenant or IMustHaveTenant...
            /// 
            /// This is to enable users to read this data entity across
            /// Tenants under specific circumstances...
            /// 
            /// For those cases, a new AppService method should be produced
            ///

            var address = await _addressRepository.FirstOrDefaultAsync((int)input.Id);

            if (AbpSession.TenantId != null) { 
                if(address.TenantId == AbpSession.TenantId)
                {
                    ObjectMapper.Map(input, address);
                }
                else
                {
                    //TODO: RAISE A SECURITY EXCEPTION
                }
            }
            else // this should only be the case if a HOST user is logged in
            {
                ObjectMapper.Map(input, address);
            }
            
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Addresses_Delete)]
         public async Task Delete(EntityDto input)
         {
            /// This has been updated to filter on TenantId because this data 
            /// entity does not implement IMayHaveTenant or IMustHaveTenant...
            /// 
            /// This is to enable users to read this data entity across
            /// Tenants under specific circumstances...
            /// 
            /// For those cases, a new AppService method should be produced
            ///

            var address = await _addressRepository.FirstOrDefaultAsync((int)input.Id);

            if (AbpSession.TenantId != null)
            {
                if (address.TenantId == AbpSession.TenantId)
                {
                    await _addressRepository.DeleteAsync(input.Id);
                }
                else
                {
                    //TODO: RAISE A SECURITY EXCEPTION
                }
            }
            else // this should only be the case if a HOST user is logged in
            {
                await _addressRepository.DeleteAsync(input.Id);
            }
            
         } 

		[AbpAuthorize(AppPermissions.Pages_Main_Addresses)]
         public async Task<PagedResultDto<AddressCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_customerRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AddressCustomerLookupTableDto>();
			foreach(var customer in customerList){
				lookupTableDtoList.Add(new AddressCustomerLookupTableDto
				{
					Id = customer.Id,
					DisplayName = customer.Name?.ToString()
				});
			}

            return new PagedResultDto<AddressCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Addresses)]
         public async Task<PagedResultDto<AddressAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetOwnerRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetOwnerList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AddressAssetOwnerLookupTableDto>();
			foreach(var assetOwner in assetOwnerList){
				lookupTableDtoList.Add(new AddressAssetOwnerLookupTableDto
				{
					Id = assetOwner.Id,
					DisplayName = assetOwner.Name?.ToString()
				});
			}

            return new PagedResultDto<AddressAssetOwnerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Addresses)]
         public async Task<PagedResultDto<AddressVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_vendorRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AddressVendorLookupTableDto>();
			foreach(var vendor in vendorList){
				lookupTableDtoList.Add(new AddressVendorLookupTableDto
				{
					Id = vendor.Id,
					DisplayName = vendor.Name?.ToString()
				});
			}

            return new PagedResultDto<AddressVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}