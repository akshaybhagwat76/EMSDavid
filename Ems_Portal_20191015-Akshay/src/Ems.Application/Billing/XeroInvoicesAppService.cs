using Ems.Billing;


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
	[AbpAuthorize(AppPermissions.Pages_Administration_XeroInvoices)]
    public class XeroInvoicesAppService : EmsAppServiceBase, IXeroInvoicesAppService
    {
		 private readonly IRepository<XeroInvoice> _xeroInvoiceRepository;
		 private readonly IXeroInvoicesExcelExporter _xeroInvoicesExcelExporter;
		 private readonly IRepository<CustomerInvoice,int> _lookup_customerInvoiceRepository;
		 

		  public XeroInvoicesAppService(IRepository<XeroInvoice> xeroInvoiceRepository, IXeroInvoicesExcelExporter xeroInvoicesExcelExporter , IRepository<CustomerInvoice, int> lookup_customerInvoiceRepository) 
		  {
			_xeroInvoiceRepository = xeroInvoiceRepository;
			_xeroInvoicesExcelExporter = xeroInvoicesExcelExporter;
			_lookup_customerInvoiceRepository = lookup_customerInvoiceRepository;
		
		  }

		 public async Task<PagedResultDto<GetXeroInvoiceForViewDto>> GetAll(GetAllXeroInvoicesInput input)
         {
			
			var filteredXeroInvoices = _xeroInvoiceRepository.GetAll()
						.Include( e => e.CustomerInvoiceFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.ApiResponse.Contains(input.Filter) || e.Exception.Contains(input.Filter) || e.XeroReference.Contains(input.Filter))
						.WhereIf(input.XeroInvoiceCreatedFilter > -1,  e => (input.XeroInvoiceCreatedFilter == 1 && e.XeroInvoiceCreated) || (input.XeroInvoiceCreatedFilter == 0 && !e.XeroInvoiceCreated) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ApiResponseFilter),  e => e.ApiResponse == input.ApiResponseFilter)
						.WhereIf(input.FailedFilter > -1,  e => (input.FailedFilter == 1 && e.Failed) || (input.FailedFilter == 0 && !e.Failed) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ExceptionFilter),  e => e.Exception == input.ExceptionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.XeroReferenceFilter),  e => e.XeroReference == input.XeroReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerInvoiceCustomerReferenceFilter), e => e.CustomerInvoiceFk != null && e.CustomerInvoiceFk.CustomerReference == input.CustomerInvoiceCustomerReferenceFilter);

			var pagedAndFilteredXeroInvoices = filteredXeroInvoices
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var xeroInvoices = from o in pagedAndFilteredXeroInvoices
                         join o1 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetXeroInvoiceForViewDto() {
							XeroInvoice = new XeroInvoiceDto
							{
                                XeroInvoiceCreated = o.XeroInvoiceCreated,
                                ApiResponse = o.ApiResponse,
                                Failed = o.Failed,
                                Exception = o.Exception,
                                XeroReference = o.XeroReference,
                                Id = o.Id
							},
                         	CustomerInvoiceCustomerReference = s1 == null ? "" : s1.CustomerReference.ToString()
						};

            var totalCount = await filteredXeroInvoices.CountAsync();

            return new PagedResultDto<GetXeroInvoiceForViewDto>(
                totalCount,
                await xeroInvoices.ToListAsync()
            );
         }
		 
		 public async Task<GetXeroInvoiceForViewDto> GetXeroInvoiceForView(int id)
         {
            var xeroInvoice = await _xeroInvoiceRepository.GetAsync(id);

            var output = new GetXeroInvoiceForViewDto { XeroInvoice = ObjectMapper.Map<XeroInvoiceDto>(xeroInvoice) };

		    if (output.XeroInvoice.CustomerInvoiceId != null)
            {
                var _lookupCustomerInvoice = await _lookup_customerInvoiceRepository.FirstOrDefaultAsync((int)output.XeroInvoice.CustomerInvoiceId);
                output.CustomerInvoiceCustomerReference = _lookupCustomerInvoice.CustomerReference.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Administration_XeroInvoices_Edit)]
		 public async Task<GetXeroInvoiceForEditOutput> GetXeroInvoiceForEdit(EntityDto input)
         {
            var xeroInvoice = await _xeroInvoiceRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetXeroInvoiceForEditOutput {XeroInvoice = ObjectMapper.Map<CreateOrEditXeroInvoiceDto>(xeroInvoice)};

		    if (output.XeroInvoice.CustomerInvoiceId != null)
            {
                var _lookupCustomerInvoice = await _lookup_customerInvoiceRepository.FirstOrDefaultAsync((int)output.XeroInvoice.CustomerInvoiceId);
                output.CustomerInvoiceCustomerReference = _lookupCustomerInvoice.CustomerReference.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditXeroInvoiceDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_XeroInvoices_Create)]
		 protected virtual async Task Create(CreateOrEditXeroInvoiceDto input)
         {
            var xeroInvoice = ObjectMapper.Map<XeroInvoice>(input);

			
			if (AbpSession.TenantId != null)
			{
				xeroInvoice.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _xeroInvoiceRepository.InsertAsync(xeroInvoice);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_XeroInvoices_Edit)]
		 protected virtual async Task Update(CreateOrEditXeroInvoiceDto input)
         {
            var xeroInvoice = await _xeroInvoiceRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, xeroInvoice);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_XeroInvoices_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _xeroInvoiceRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetXeroInvoicesToExcel(GetAllXeroInvoicesForExcelInput input)
         {
			
			var filteredXeroInvoices = _xeroInvoiceRepository.GetAll()
						.Include( e => e.CustomerInvoiceFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.ApiResponse.Contains(input.Filter) || e.Exception.Contains(input.Filter) || e.XeroReference.Contains(input.Filter))
						.WhereIf(input.XeroInvoiceCreatedFilter > -1,  e => (input.XeroInvoiceCreatedFilter == 1 && e.XeroInvoiceCreated) || (input.XeroInvoiceCreatedFilter == 0 && !e.XeroInvoiceCreated) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ApiResponseFilter),  e => e.ApiResponse == input.ApiResponseFilter)
						.WhereIf(input.FailedFilter > -1,  e => (input.FailedFilter == 1 && e.Failed) || (input.FailedFilter == 0 && !e.Failed) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ExceptionFilter),  e => e.Exception == input.ExceptionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.XeroReferenceFilter),  e => e.XeroReference == input.XeroReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CustomerInvoiceCustomerReferenceFilter), e => e.CustomerInvoiceFk != null && e.CustomerInvoiceFk.CustomerReference == input.CustomerInvoiceCustomerReferenceFilter);

			var query = (from o in filteredXeroInvoices
                         join o1 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetXeroInvoiceForViewDto() { 
							XeroInvoice = new XeroInvoiceDto
							{
                                XeroInvoiceCreated = o.XeroInvoiceCreated,
                                ApiResponse = o.ApiResponse,
                                Failed = o.Failed,
                                Exception = o.Exception,
                                XeroReference = o.XeroReference,
                                Id = o.Id
							},
                         	CustomerInvoiceCustomerReference = s1 == null ? "" : s1.CustomerReference.ToString()
						 });


            var xeroInvoiceListDtos = await query.ToListAsync();

            return _xeroInvoicesExcelExporter.ExportToFile(xeroInvoiceListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Administration_XeroInvoices)]
         public async Task<PagedResultDto<XeroInvoiceCustomerInvoiceLookupTableDto>> GetAllCustomerInvoiceForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_customerInvoiceRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.CustomerReference != null && e.CustomerReference.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var customerInvoiceList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<XeroInvoiceCustomerInvoiceLookupTableDto>();
			foreach(var customerInvoice in customerInvoiceList){
				lookupTableDtoList.Add(new XeroInvoiceCustomerInvoiceLookupTableDto
				{
					Id = customerInvoice.Id,
					DisplayName = customerInvoice.CustomerReference?.ToString()
				});
			}

            return new PagedResultDto<XeroInvoiceCustomerInvoiceLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}