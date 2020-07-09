using Ems.Billing;
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
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Customers
{
	[AbpAuthorize(AppPermissions.Pages_Main_Customers)]
    public class CustomersAppService : EmsAppServiceBase, ICustomersAppService
    {
		 private readonly IRepository<Customer> _customerRepository;
		 private readonly ICustomersExcelExporter _customersExcelExporter;
		 private readonly IRepository<CustomerType,int> _lookup_customerTypeRepository;
		 private readonly IRepository<Currency,int> _lookup_currencyRepository;
		 

		  public CustomersAppService(IRepository<Customer> customerRepository, ICustomersExcelExporter customersExcelExporter , IRepository<CustomerType, int> lookup_customerTypeRepository, IRepository<Currency, int> lookup_currencyRepository) 
		  {
			_customerRepository = customerRepository;
			_customersExcelExporter = customersExcelExporter;
			_lookup_customerTypeRepository = lookup_customerTypeRepository;
		    _lookup_currencyRepository = lookup_currencyRepository;
		
		  }

		 public async Task<PagedResultDto<GetCustomerForViewDto>> GetAll(GetAllCustomersInput input)
         {
			
			var filteredCustomers = _customerRepository.GetAll()
						.Include( e => e.CustomerTypeFk)
						.Include( e => e.CurrencyFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.CustomerLoc8UUID.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter),  e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerLoc8UUIDFilter),  e => e.CustomerLoc8UUID.ToLower() == input.CustomerLoc8UUIDFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerTypeTypeFilter), e => e.CustomerTypeFk != null && e.CustomerTypeFk.Type.ToLower() == input.CustomerTypeTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());

			var pagedAndFilteredCustomers = filteredCustomers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var customers = from o in pagedAndFilteredCustomers
                         join o1 in _lookup_customerTypeRepository.GetAll() on o.CustomerTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetCustomerForViewDto() {
							Customer = new CustomerDto
							{
                                Reference = o.Reference,
                                Name = o.Name,
                                Identifier = o.Identifier,
                                LogoUrl = o.LogoUrl,
                                Website = o.Website,
                                CustomerLoc8UUID = o.CustomerLoc8UUID,
                                Id = o.Id
							},
                         	CustomerTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	CurrencyCode = s2 == null ? "" : s2.Code.ToString()
						};

            var totalCount = await filteredCustomers.CountAsync();

            return new PagedResultDto<GetCustomerForViewDto>(
                totalCount,
                await customers.ToListAsync()
            );
         }
		 
		 public async Task<GetCustomerForViewDto> GetCustomerForView(int? id)
         {
            // START UPDATE // Add this code to enable the Customer Profile to work -- NB: made 'id' nullable and updated the Interface accordingly

            Customer currentCustomer;
            int customerId;

            if (id == null)
            {
                currentCustomer = _customerRepository.GetAll().Where(e => e.TenantId == AbpSession.TenantId).FirstOrDefault();
                if(currentCustomer == null)
                {
                    return new GetCustomerForViewDto();
                }
                customerId = currentCustomer.Id;
            }
            else
            {
                customerId = (int)id;
            }

            var customer = await _customerRepository.GetAsync(customerId);

            // END UPDATE //

            var output = new GetCustomerForViewDto { Customer = ObjectMapper.Map<CustomerDto>(customer) };

		    if (output.Customer != null)
            {
                var _lookupCustomerType = await _lookup_customerTypeRepository.FirstOrDefaultAsync((int)output.Customer.CustomerTypeId);
                output.CustomerTypeType = _lookupCustomerType.Type.ToString();
            }

		    if (output.Customer.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Customer.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

            if(!string.IsNullOrWhiteSpace(output.Customer.LogoUrl))
            {
                int length = (output.Customer.LogoUrl.Length >= 36) ? 36 : output.Customer.LogoUrl.Length;
                output.Customer.LogoUrl = string.Format("{0}...", output.Customer.LogoUrl.Substring(0, 36));
            }
            if (!string.IsNullOrWhiteSpace(output.Customer.Website))
            {
                int length = (output.Customer.Website.Length >= 36) ? 36 : output.Customer.Website.Length;
                output.Customer.Website = string.Format("{0}...", output.Customer.Website.Substring(0, 36));
            }
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_Customers_Edit)]
		 public async Task<GetCustomerForEditOutput> GetCustomerForEdit(EntityDto input)
         {
            var customer = await _customerRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCustomerForEditOutput {Customer = ObjectMapper.Map<CreateOrEditCustomerDto>(customer)};

		    if (output.Customer != null)
            {
                var _lookupCustomerType = await _lookup_customerTypeRepository.FirstOrDefaultAsync((int)output.Customer.CustomerTypeId);
                output.CustomerTypeType = _lookupCustomerType.Type.ToString();
            }

		    if (output.Customer.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Customer.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCustomerDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Customers_Create)]
		 protected virtual async Task Create(CreateOrEditCustomerDto input)
         {
            var customer = ObjectMapper.Map<Customer>(input);

			
			if (AbpSession.TenantId != null)
			{
				customer.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _customerRepository.InsertAsync(customer);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Customers_Edit)]
		 protected virtual async Task Update(CreateOrEditCustomerDto input)
         {
            var customer = await _customerRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, customer);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Customers_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _customerRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetCustomersToExcel(GetAllCustomersForExcelInput input)
         {
			
			var filteredCustomers = _customerRepository.GetAll()
						.Include( e => e.CustomerTypeFk)
						.Include( e => e.CurrencyFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.CustomerLoc8UUID.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter),  e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerLoc8UUIDFilter),  e => e.CustomerLoc8UUID.ToLower() == input.CustomerLoc8UUIDFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerTypeTypeFilter), e => e.CustomerTypeFk != null && e.CustomerTypeFk.Type.ToLower() == input.CustomerTypeTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());

			var query = (from o in filteredCustomers
                         join o1 in _lookup_customerTypeRepository.GetAll() on o.CustomerTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetCustomerForViewDto() { 
							Customer = new CustomerDto
							{
                                Reference = o.Reference,
                                Name = o.Name,
                                Identifier = o.Identifier,
                                LogoUrl = o.LogoUrl,
                                Website = o.Website,
                                CustomerLoc8UUID = o.CustomerLoc8UUID,
                                Id = o.Id
							},
                         	CustomerTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	CurrencyCode = s2 == null ? "" : s2.Code.ToString()
						 });


            var customerListDtos = await query.ToListAsync();

            return _customersExcelExporter.ExportToFile(customerListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_Customers)]
         public async Task<PagedResultDto<CustomerCustomerTypeLookupTableDto>> GetAllCustomerTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_customerTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var customerTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CustomerCustomerTypeLookupTableDto>();
			foreach(var customerType in customerTypeList){
				lookupTableDtoList.Add(new CustomerCustomerTypeLookupTableDto
				{
					Id = customerType.Id,
					DisplayName = customerType.Type?.ToString()
				});
			}

            return new PagedResultDto<CustomerCustomerTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Customers)]
         public async Task<PagedResultDto<CustomerCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_currencyRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Code.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<CustomerCurrencyLookupTableDto>();
			foreach(var currency in currencyList){
				lookupTableDtoList.Add(new CustomerCurrencyLookupTableDto
				{
					Id = currency.Id,
					DisplayName = currency.Code?.ToString()
				});
			}

            return new PagedResultDto<CustomerCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}