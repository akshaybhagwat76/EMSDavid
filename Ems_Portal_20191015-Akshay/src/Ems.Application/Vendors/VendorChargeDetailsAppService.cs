

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Vendors.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Vendors
{
	[AbpAuthorize(AppPermissions.Pages_Main_VendorCharges)]
    public class VendorChargeDetailsAppService : EmsAppServiceBase, IVendorChargeDetailsAppService
    {
		 private readonly IRepository<VendorChargeDetail> _vendorChargeDetailRepository;
		 

		  public VendorChargeDetailsAppService(IRepository<VendorChargeDetail> vendorChargeDetailRepository ) 
		  {
			_vendorChargeDetailRepository = vendorChargeDetailRepository;
			
		  }

		 public async Task<PagedResultDto<GetVendorChargeDetailForViewDto>> GetAll(GetAllVendorChargeDetailsInput input)
         {
			
			var filteredVendorChargeDetails = _vendorChargeDetailRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.InvoiceDetail.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.InvoiceDetailFilter),  e => e.InvoiceDetail.ToLower() == input.InvoiceDetailFilter.ToLower().Trim())
						.WhereIf(input.MinQuantityFilter != null, e => e.Quantity >= input.MinQuantityFilter)
						.WhereIf(input.MaxQuantityFilter != null, e => e.Quantity <= input.MaxQuantityFilter)
						.WhereIf(input.MinUnitPriceFilter != null, e => e.UnitPrice >= input.MinUnitPriceFilter)
						.WhereIf(input.MaxUnitPriceFilter != null, e => e.UnitPrice <= input.MaxUnitPriceFilter)
						.WhereIf(input.MinTaxFilter != null, e => e.Tax >= input.MinTaxFilter)
						.WhereIf(input.MaxTaxFilter != null, e => e.Tax <= input.MaxTaxFilter)
						.WhereIf(input.MinSubTotalFilter != null, e => e.SubTotal >= input.MinSubTotalFilter)
						.WhereIf(input.MaxSubTotalFilter != null, e => e.SubTotal <= input.MaxSubTotalFilter);

			var pagedAndFilteredVendorChargeDetails = filteredVendorChargeDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var vendorChargeDetails = from o in pagedAndFilteredVendorChargeDetails
                         select new GetVendorChargeDetailForViewDto() {
							VendorChargeDetail = new VendorChargeDetailDto
							{
                                InvoiceDetail = o.InvoiceDetail,
                                Quantity = o.Quantity,
                                UnitPrice = o.UnitPrice,
                                Tax = o.Tax,
                                SubTotal = o.SubTotal,
                                Id = o.Id
							}
						};

            var totalCount = await filteredVendorChargeDetails.CountAsync();

            return new PagedResultDto<GetVendorChargeDetailForViewDto>(
                totalCount,
                await vendorChargeDetails.ToListAsync()
            );
         }
		 
		 public async Task<GetVendorChargeDetailForViewDto> GetVendorChargeDetailForView(int id)
         {
            var vendorChargeDetail = await _vendorChargeDetailRepository.GetAsync(id);

            var output = new GetVendorChargeDetailForViewDto { VendorChargeDetail = ObjectMapper.Map<VendorChargeDetailDto>(vendorChargeDetail) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsEdit)]
		 public async Task<GetVendorChargeDetailForEditOutput> GetVendorChargeDetailForEdit(EntityDto input)
         {
            var vendorChargeDetail = await _vendorChargeDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetVendorChargeDetailForEditOutput {VendorChargeDetail = ObjectMapper.Map<CreateOrEditVendorChargeDetailDto>(vendorChargeDetail)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditVendorChargeDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsCreate)]
		 protected virtual async Task Create(CreateOrEditVendorChargeDetailDto input)
         {
            var vendorChargeDetail = ObjectMapper.Map<VendorChargeDetail>(input);

			
			if (AbpSession.TenantId != null)
			{
				vendorChargeDetail.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _vendorChargeDetailRepository.InsertAsync(vendorChargeDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsEdit)]
		 protected virtual async Task Update(CreateOrEditVendorChargeDetailDto input)
         {
            var vendorChargeDetail = await _vendorChargeDetailRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, vendorChargeDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_VendorChargeDetailsDelete)]
         public async Task Delete(EntityDto input)
         {
            await _vendorChargeDetailRepository.DeleteAsync(input.Id);
         } 
    }
}