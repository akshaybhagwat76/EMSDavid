using Ems.Customers;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Customers.Exporting;
using Ems.Customers.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Customers
{
	[AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships)]
    public class CustomerGroupMembershipsAppService : EmsAppServiceBase, ICustomerGroupMembershipsAppService
    {
		 private readonly IRepository<CustomerGroupMembership> _customerGroupMembershipRepository;
		 private readonly ICustomerGroupMembershipsExcelExporter _customerGroupMembershipsExcelExporter;
		 private readonly IRepository<CustomerGroup,int> _lookup_customerGroupRepository;
		 private readonly IRepository<Customer,int> _lookup_customerRepository;
		 

		  public CustomerGroupMembershipsAppService(IRepository<CustomerGroupMembership> customerGroupMembershipRepository, ICustomerGroupMembershipsExcelExporter customerGroupMembershipsExcelExporter , IRepository<CustomerGroup, int> lookup_customerGroupRepository, IRepository<Customer, int> lookup_customerRepository) 
		  {
			_customerGroupMembershipRepository = customerGroupMembershipRepository;
			_customerGroupMembershipsExcelExporter = customerGroupMembershipsExcelExporter;
			_lookup_customerGroupRepository = lookup_customerGroupRepository;
		_lookup_customerRepository = lookup_customerRepository;
		
		  }

		 public async Task<PagedResultDto<GetCustomerGroupMembershipForViewDto>> GetAll(GetAllCustomerGroupMembershipsInput input)
         {
			
			var filteredCustomerGroupMemberships = _customerGroupMembershipRepository.GetAll()
						.Include( e => e.CustomerGroupFk)
						.Include( e => e.CustomerFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinDateJoinedFilter != null, e => e.DateJoined >= input.MinDateJoinedFilter)
						.WhereIf(input.MaxDateJoinedFilter != null, e => e.DateJoined <= input.MaxDateJoinedFilter)
						.WhereIf(input.MinDateLeftFilter != null, e => e.DateLeft >= input.MinDateLeftFilter)
						.WhereIf(input.MaxDateLeftFilter != null, e => e.DateLeft <= input.MaxDateLeftFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerGroupNameFilter), e => e.CustomerGroupFk != null && e.CustomerGroupFk.Name.ToLower() == input.CustomerGroupNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim());

			var pagedAndFilteredCustomerGroupMemberships = filteredCustomerGroupMemberships
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var customerGroupMemberships = from o in pagedAndFilteredCustomerGroupMemberships
                         join o1 in _lookup_customerGroupRepository.GetAll() on o.CustomerGroupId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_customerRepository.GetAll() on o.CustomerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetCustomerGroupMembershipForViewDto() {
							CustomerGroupMembership = new CustomerGroupMembershipDto
							{
                                DateJoined = o.DateJoined,
                                DateLeft = o.DateLeft,
                                Id = o.Id
							},
                         	CustomerGroupName = s1 == null ? "" : s1.Name.ToString(),
                         	CustomerName = s2 == null ? "" : s2.Name.ToString()
						};

            var totalCount = await filteredCustomerGroupMemberships.CountAsync();

            return new PagedResultDto<GetCustomerGroupMembershipForViewDto>(
                totalCount,
                await customerGroupMemberships.ToListAsync()
            );
         }
		 
		 public async Task<GetCustomerGroupMembershipForViewDto> GetCustomerGroupMembershipForView(int id)
         {
            var customerGroupMembership = await _customerGroupMembershipRepository.GetAsync(id);

            var output = new GetCustomerGroupMembershipForViewDto { CustomerGroupMembership = ObjectMapper.Map<CustomerGroupMembershipDto>(customerGroupMembership) };

		    if (output.CustomerGroupMembership.CustomerGroupId != null)
            {
                var _lookupCustomerGroup = await _lookup_customerGroupRepository.FirstOrDefaultAsync((int)output.CustomerGroupMembership.CustomerGroupId);
                output.CustomerGroupName = _lookupCustomerGroup.Name.ToString();
            }

		    if (output.CustomerGroupMembership.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.CustomerGroupMembership.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships_Edit)]
		 public async Task<GetCustomerGroupMembershipForEditOutput> GetCustomerGroupMembershipForEdit(EntityDto input)
         {
            var customerGroupMembership = await _customerGroupMembershipRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCustomerGroupMembershipForEditOutput {CustomerGroupMembership = ObjectMapper.Map<CreateOrEditCustomerGroupMembershipDto>(customerGroupMembership)};

		    if (output.CustomerGroupMembership.CustomerGroupId != null)
            {
                var _lookupCustomerGroup = await _lookup_customerGroupRepository.FirstOrDefaultAsync((int)output.CustomerGroupMembership.CustomerGroupId);
                output.CustomerGroupName = _lookupCustomerGroup.Name.ToString();
            }

		    if (output.CustomerGroupMembership.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.CustomerGroupMembership.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCustomerGroupMembershipDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships_Create)]
		 protected virtual async Task Create(CreateOrEditCustomerGroupMembershipDto input)
         {
            var customerGroupMembership = ObjectMapper.Map<CustomerGroupMembership>(input);

			
			if (AbpSession.TenantId != null)
			{
				customerGroupMembership.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _customerGroupMembershipRepository.InsertAsync(customerGroupMembership);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships_Edit)]
		 protected virtual async Task Update(CreateOrEditCustomerGroupMembershipDto input)
         {
            var customerGroupMembership = await _customerGroupMembershipRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, customerGroupMembership);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _customerGroupMembershipRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetCustomerGroupMembershipsToExcel(GetAllCustomerGroupMembershipsForExcelInput input)
         {
			
			var filteredCustomerGroupMemberships = _customerGroupMembershipRepository.GetAll()
						.Include( e => e.CustomerGroupFk)
						.Include( e => e.CustomerFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinDateJoinedFilter != null, e => e.DateJoined >= input.MinDateJoinedFilter)
						.WhereIf(input.MaxDateJoinedFilter != null, e => e.DateJoined <= input.MaxDateJoinedFilter)
						.WhereIf(input.MinDateLeftFilter != null, e => e.DateLeft >= input.MinDateLeftFilter)
						.WhereIf(input.MaxDateLeftFilter != null, e => e.DateLeft <= input.MaxDateLeftFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerGroupNameFilter), e => e.CustomerGroupFk != null && e.CustomerGroupFk.Name.ToLower() == input.CustomerGroupNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim());

			var query = (from o in filteredCustomerGroupMemberships
                         join o1 in _lookup_customerGroupRepository.GetAll() on o.CustomerGroupId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_customerRepository.GetAll() on o.CustomerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetCustomerGroupMembershipForViewDto() { 
							CustomerGroupMembership = new CustomerGroupMembershipDto
							{
                                DateJoined = o.DateJoined,
                                DateLeft = o.DateLeft,
                                Id = o.Id
							},
                         	CustomerGroupName = s1 == null ? "" : s1.Name.ToString(),
                         	CustomerName = s2 == null ? "" : s2.Name.ToString()
						 });


            var customerGroupMembershipListDtos = await query.ToListAsync();

            return _customerGroupMembershipsExcelExporter.ExportToFile(customerGroupMembershipListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships)]
         public async Task<PagedResultDto<CustomerGroupMembershipCustomerGroupLookupTableDto>> GetAllCustomerGroupForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_customerGroupRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var customerGroupList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CustomerGroupMembershipCustomerGroupLookupTableDto>();
			foreach(var customerGroup in customerGroupList){
				lookupTableDtoList.Add(new CustomerGroupMembershipCustomerGroupLookupTableDto
				{
					Id = customerGroup.Id,
					DisplayName = customerGroup.Name?.ToString()
				});
			}

            return new PagedResultDto<CustomerGroupMembershipCustomerGroupLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_CustomerGroupMemberships)]
         public async Task<PagedResultDto<CustomerGroupMembershipCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_customerRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CustomerGroupMembershipCustomerLookupTableDto>();
			foreach(var customer in customerList){
				lookupTableDtoList.Add(new CustomerGroupMembershipCustomerLookupTableDto
				{
					Id = customer.Id,
					DisplayName = customer.Name?.ToString()
				});
			}

            return new PagedResultDto<CustomerGroupMembershipCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}