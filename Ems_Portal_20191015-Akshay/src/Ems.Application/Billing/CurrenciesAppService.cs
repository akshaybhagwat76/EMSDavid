

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Billing.Exporting;
using Ems.Billing.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Billing
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_Currencies)]
    public class CurrenciesAppService : EmsAppServiceBase, ICurrenciesAppService
    {
		 private readonly IRepository<Currency> _currencyRepository;
		 private readonly ICurrenciesExcelExporter _currenciesExcelExporter;

        public CurrenciesAppService(IRepository<Currency> currencyRepository, 
                                    ICurrenciesExcelExporter currenciesExcelExporter)
		  {
			_currencyRepository = currencyRepository;
			_currenciesExcelExporter = currenciesExcelExporter;
        }

		 public async Task<PagedResultDto<GetCurrencyForViewDto>> GetAll(GetAllCurrenciesInput input)
         {

            var filteredCurrencies = _currencyRepository.GetAll() 
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Code.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Symbol.Contains(input.Filter) || e.Country.Contains(input.Filter) || e.BaseCountry.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter), e => e.Code.ToLower() == input.CodeFilter.ToLower().Trim())
                .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
                .WhereIf(!string.IsNullOrWhiteSpace(input.SymbolFilter), e => e.Symbol.ToLower() == input.SymbolFilter.ToLower().Trim())
                .WhereIf(!string.IsNullOrWhiteSpace(input.CountryFilter), e => e.Country.ToLower() == input.CountryFilter.ToLower().Trim())
                .WhereIf(!string.IsNullOrWhiteSpace(input.BaseCountryFilter), e => e.BaseCountry.ToLower() == input.BaseCountryFilter.ToLower().Trim());

            var pagedAndFilteredCurrencies = filteredCurrencies
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var currencies = from o in pagedAndFilteredCurrencies
                         select new GetCurrencyForViewDto() {
							Currency = new CurrencyDto
							{
                                Code = o.Code,
                                Name = o.Name,
                                Symbol = o.Symbol,
                                Country = o.Country,
                                BaseCountry = o.BaseCountry,
                                Id = o.Id
							}
						};

            var totalCount = await filteredCurrencies.CountAsync();

            return new PagedResultDto<GetCurrencyForViewDto>(
                totalCount,
                await currencies.ToListAsync()
            );
         }
		 
		 public async Task<GetCurrencyForViewDto> GetCurrencyForView(int id)
         {
            var currency = await _currencyRepository.GetAsync(id);

            var output = new GetCurrencyForViewDto { Currency = ObjectMapper.Map<CurrencyDto>(currency) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_Currencies_Edit)]
		 public async Task<GetCurrencyForEditOutput> GetCurrencyForEdit(EntityDto input)
         {
            var currency = await _currencyRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCurrencyForEditOutput {Currency = ObjectMapper.Map<CreateOrEditCurrencyDto>(currency)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCurrencyDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_Currencies_Create)]
		 protected virtual async Task Create(CreateOrEditCurrencyDto input)
         {
            var currency = ObjectMapper.Map<Currency>(input);

			
			if (AbpSession.TenantId != null)
			{
				currency.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _currencyRepository.InsertAsync(currency);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_Currencies_Edit)]
		 protected virtual async Task Update(CreateOrEditCurrencyDto input)
         {
            var currency = await _currencyRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, currency);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_Currencies_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _currencyRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetCurrenciesToExcel(GetAllCurrenciesForExcelInput input)
         {
			
			var filteredCurrencies = _currencyRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Code.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Symbol.Contains(input.Filter) || e.Country.Contains(input.Filter) || e.BaseCountry.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.CodeFilter),  e => e.Code.ToLower() == input.CodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SymbolFilter),  e => e.Symbol.ToLower() == input.SymbolFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CountryFilter),  e => e.Country.ToLower() == input.CountryFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BaseCountryFilter),  e => e.BaseCountry.ToLower() == input.BaseCountryFilter.ToLower().Trim());

			var query = (from o in filteredCurrencies
                         select new GetCurrencyForViewDto() { 
							Currency = new CurrencyDto
							{
                                Code = o.Code,
                                Name = o.Name,
                                Symbol = o.Symbol,
                                Country = o.Country,
                                BaseCountry = o.BaseCountry,
                                Id = o.Id
							}
						 });


            var currencyListDtos = await query.ToListAsync();

            return _currenciesExcelExporter.ExportToFile(currencyListDtos);
         }


    }
}