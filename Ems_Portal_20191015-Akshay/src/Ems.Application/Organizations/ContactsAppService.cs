using Ems.Authorization.Users;
using Ems.Vendors;
using Ems.Assets;
using Ems.Customers;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Organizations.Exporting;
using Ems.Organizations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Organizations
{
    [AbpAuthorize(AppPermissions.Pages_Main_Contacts)]
    public class ContactsAppService : EmsAppServiceBase, IContactsAppService
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly IContactsExcelExporter _contactsExcelExporter;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<Vendor, int> _lookup_vendorRepository;
        private readonly IRepository<AssetOwner, int> _lookup_assetOwnerRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;


        public ContactsAppService(IRepository<Contact> contactRepository, IContactsExcelExporter contactsExcelExporter, IRepository<User, long> lookup_userRepository, IRepository<Vendor, int> lookup_vendorRepository, IRepository<AssetOwner, int> lookup_assetOwnerRepository, IRepository<Customer, int> lookup_customerRepository)
        {
            _contactRepository = contactRepository;
            _contactsExcelExporter = contactsExcelExporter;
            _lookup_userRepository = lookup_userRepository;
            _lookup_vendorRepository = lookup_vendorRepository;
            _lookup_assetOwnerRepository = lookup_assetOwnerRepository;
            _lookup_customerRepository = lookup_customerRepository;

        }

        public async Task<PagedResultDto<GetContactForViewDto>> GetAll(GetAllContactsInput input)
        {

            var filteredContacts = _contactRepository.GetAll()
                        .Include(e => e.UserFk)
                        .Include(e => e.VendorFk)
                        .Include(e => e.AssetOwnerFk)
                        .Include(e => e.CustomerFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ContactName.Contains(input.Filter) || e.PhoneOffice.Contains(input.Filter) || e.PhoneMobile.Contains(input.Filter) || e.Fax.Contains(input.Filter) || e.EmailAddress.Contains(input.Filter) || e.Position.Contains(input.Filter) || e.Department.Contains(input.Filter) || e.ContactLoc8GUID.Contains(input.Filter))
                        .WhereIf(input.HeadOfficeContactFilter > -1, e => Convert.ToInt32(e.HeadOfficeContact) == input.HeadOfficeContactFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactNameFilter), e => e.ContactName.ToLower() == input.ContactNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneOfficeFilter), e => e.PhoneOffice.ToLower() == input.PhoneOfficeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneMobileFilter), e => e.PhoneMobile.ToLower() == input.PhoneMobileFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FaxFilter), e => e.Fax.ToLower() == input.FaxFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AddressFilter), e => e.Address.ToLower().Contains(input.AddressFilter.ToLower().Trim()))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailAddressFilter), e => e.EmailAddress.ToLower() == input.EmailAddressFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PositionFilter), e => e.Position.ToLower() == input.PositionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DepartmentFilter), e => e.Department.ToLower() == input.DepartmentFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactLoc8GUIDFilter), e => e.ContactLoc8GUID.ToLower() == input.ContactLoc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim());

            var pagedAndFilteredContacts = filteredContacts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var contacts = from o in pagedAndFilteredContacts
                           join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                           from s1 in j1.DefaultIfEmpty()

                           join o2 in _lookup_vendorRepository.GetAll() on o.VendorId equals o2.Id into j2
                           from s2 in j2.DefaultIfEmpty()

                           join o3 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o3.Id into j3
                           from s3 in j3.DefaultIfEmpty()

                           join o4 in _lookup_customerRepository.GetAll() on o.CustomerId equals o4.Id into j4
                           from s4 in j4.DefaultIfEmpty()

                           select new GetContactForViewDto()
                           {
                               Contact = new ContactDto
                               {
                                   HeadOfficeContact = o.HeadOfficeContact,
                                   ContactName = o.ContactName,
                                   PhoneOffice = o.PhoneOffice,
                                   PhoneMobile = o.PhoneMobile,
                                   Fax = o.Fax,
                                   EmailAddress = o.EmailAddress,
                                   Position = o.Position,
                                   Department = o.Department,
                                   ContactLoc8GUID = o.ContactLoc8GUID,
                                   Id = o.Id,
                                   Address = o.Address
                               },
                               UserName = s1 == null ? "" : s1.Name.ToString(),
                               VendorName = s2 == null ? "" : s2.Name.ToString(),
                               AssetOwnerName = s3 == null ? "" : s3.Name.ToString(),
                               CustomerName = s4 == null ? "" : s4.Name.ToString()
                           };

            var totalCount = await filteredContacts.CountAsync();

            return new PagedResultDto<GetContactForViewDto>(
                totalCount,
                await contacts.ToListAsync()
            );
        }

        public async Task<GetContactForViewDto> GetContactForView(int id)
        {
            var contact = await _contactRepository.GetAsync(id);

            var output = new GetContactForViewDto { Contact = ObjectMapper.Map<ContactDto>(contact) };

            if (output.Contact.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Contact.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }

            if (output.Contact.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.Contact.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

            if (output.Contact.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.Contact.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

            if (output.Contact.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Contact.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts_Edit)]
        public async Task<GetContactForEditOutput> GetContactForEdit(EntityDto input)
        {
            var contact = await _contactRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetContactForEditOutput { Contact = ObjectMapper.Map<CreateOrEditContactDto>(contact) };

            if (output.Contact.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Contact.UserId);
                output.UserName = _lookupUser.Name.ToString();
            }

            if (output.Contact.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.Contact.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

            if (output.Contact.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.Contact.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }

            if (output.Contact.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.Contact.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditContactDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts_Create)]
        protected virtual async Task Create(CreateOrEditContactDto input)
        {
            var contact = ObjectMapper.Map<Contact>(input);


            if (AbpSession.TenantId != null)
            {
                contact.TenantId = (int?)AbpSession.TenantId;
            }


            await _contactRepository.InsertAsync(contact);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts_Edit)]
        protected virtual async Task Update(CreateOrEditContactDto input)
        {
            var contact = await _contactRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, contact);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _contactRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetContactsToExcel(GetAllContactsForExcelInput input)
        {

            var filteredContacts = _contactRepository.GetAll()
                        .Include(e => e.UserFk)
                        .Include(e => e.VendorFk)
                        .Include(e => e.AssetOwnerFk)
                        .Include(e => e.CustomerFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.ContactName.Contains(input.Filter) || e.PhoneOffice.Contains(input.Filter) || e.PhoneMobile.Contains(input.Filter) || e.Fax.Contains(input.Filter) || e.EmailAddress.Contains(input.Filter) || e.Position.Contains(input.Filter) || e.Department.Contains(input.Filter) || e.ContactLoc8GUID.Contains(input.Filter))
                        .WhereIf(input.HeadOfficeContactFilter > -1, e => Convert.ToInt32(e.HeadOfficeContact) == input.HeadOfficeContactFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactNameFilter), e => e.ContactName.ToLower() == input.ContactNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneOfficeFilter), e => e.PhoneOffice.ToLower() == input.PhoneOfficeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneMobileFilter), e => e.PhoneMobile.ToLower() == input.PhoneMobileFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.FaxFilter), e => e.Fax.ToLower() == input.FaxFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AddressFilter), e => e.Address.ToLower().Contains(input.EmailAddressFilter.ToLower().Trim()))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailAddressFilter), e => e.EmailAddress.ToLower() == input.EmailAddressFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PositionFilter), e => e.Position.ToLower() == input.PositionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DepartmentFilter), e => e.Department.ToLower() == input.DepartmentFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ContactLoc8GUIDFilter), e => e.ContactLoc8GUID.ToLower() == input.ContactLoc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim());

            var query = (from o in filteredContacts
                         join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_vendorRepository.GetAll() on o.VendorId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_customerRepository.GetAll() on o.CustomerId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetContactForViewDto()
                         {
                             Contact = new ContactDto
                             {
                                 HeadOfficeContact = o.HeadOfficeContact,
                                 ContactName = o.ContactName,
                                 PhoneOffice = o.PhoneOffice,
                                 PhoneMobile = o.PhoneMobile,
                                 Fax = o.Fax,
                                 EmailAddress = o.EmailAddress,
                                 Position = o.Position,
                                 Department = o.Department,
                                 ContactLoc8GUID = o.ContactLoc8GUID,
                                 Id = o.Id,
                                 Address = o.Address
                             },
                             UserName = s1 == null ? "" : s1.Name.ToString(),
                             VendorName = s2 == null ? "" : s2.Name.ToString(),
                             AssetOwnerName = s3 == null ? "" : s3.Name.ToString(),
                             CustomerName = s4 == null ? "" : s4.Name.ToString()
                         });


            var contactListDtos = await query.ToListAsync();

            return _contactsExcelExporter.ExportToFile(contactListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_Contacts)]
        public async Task<PagedResultDto<ContactUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ContactUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new ContactUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.Name?.ToString()
                });
            }

            return new PagedResultDto<ContactUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts)]
        public async Task<PagedResultDto<ContactVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_vendorRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ContactVendorLookupTableDto>();
            foreach (var vendor in vendorList)
            {
                lookupTableDtoList.Add(new ContactVendorLookupTableDto
                {
                    Id = vendor.Id,
                    DisplayName = vendor.Name?.ToString()
                });
            }

            return new PagedResultDto<ContactVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts)]
        public async Task<PagedResultDto<ContactAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_assetOwnerRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var assetOwnerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ContactAssetOwnerLookupTableDto>();
            foreach (var assetOwner in assetOwnerList)
            {
                lookupTableDtoList.Add(new ContactAssetOwnerLookupTableDto
                {
                    Id = assetOwner.Id,
                    DisplayName = assetOwner.Name?.ToString()
                });
            }

            return new PagedResultDto<ContactAssetOwnerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Contacts)]
        public async Task<PagedResultDto<ContactCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_customerRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<ContactCustomerLookupTableDto>();
            foreach (var customer in customerList)
            {
                lookupTableDtoList.Add(new ContactCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer.Name?.ToString()
                });
            }

            return new PagedResultDto<ContactCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }
    }
}