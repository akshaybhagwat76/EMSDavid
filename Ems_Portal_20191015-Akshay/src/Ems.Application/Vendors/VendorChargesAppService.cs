using Ems.Vendors;
using Ems.Support;
using Ems.Billing;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Vendors.Exporting;
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
    public class VendorChargesAppService : EmsAppServiceBase, IVendorChargesAppService
    {
		 private readonly IRepository<VendorCharge> _vendorChargeRepository;
		 private readonly IVendorChargesExcelExporter _vendorChargesExcelExporter;
		 private readonly IRepository<Vendor,int> _lookup_vendorRepository;
		 private readonly IRepository<SupportContract,int> _lookup_supportContractRepository;
		 private readonly IRepository<WorkOrder,int> _lookup_workOrderRepository;
		 private readonly IRepository<VendorChargeStatus,int> _lookup_vendorChargeStatusRepository;
		 

		  public VendorChargesAppService(IRepository<VendorCharge> vendorChargeRepository, IVendorChargesExcelExporter vendorChargesExcelExporter , IRepository<Vendor, int> lookup_vendorRepository, IRepository<SupportContract, int> lookup_supportContractRepository, IRepository<WorkOrder, int> lookup_workOrderRepository, IRepository<VendorChargeStatus, int> lookup_vendorChargeStatusRepository) 
		  {
			_vendorChargeRepository = vendorChargeRepository;
			_vendorChargesExcelExporter = vendorChargesExcelExporter;
			_lookup_vendorRepository = lookup_vendorRepository;
		_lookup_supportContractRepository = lookup_supportContractRepository;
		_lookup_workOrderRepository = lookup_workOrderRepository;
		_lookup_vendorChargeStatusRepository = lookup_vendorChargeStatusRepository;
		
		  }

		 public async Task<PagedResultDto<GetVendorChargeForViewDto>> GetAll(GetAllVendorChargesInput input)
         {
			
			var filteredVendorCharges = _vendorChargeRepository.GetAll()
						.Include( e => e.VendorFk)
						.Include( e => e.SupportContractFk)
						.Include( e => e.WorkOrderFk)
						.Include( e => e.VendorChargeStatusFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
						.WhereIf(input.MinDateIssuedFilter != null, e => e.DateIssued >= input.MinDateIssuedFilter)
						.WhereIf(input.MaxDateIssuedFilter != null, e => e.DateIssued <= input.MaxDateIssuedFilter)
						.WhereIf(input.MinDateDueFilter != null, e => e.DateDue >= input.MinDateDueFilter)
						.WhereIf(input.MaxDateDueFilter != null, e => e.DateDue <= input.MaxDateDueFilter)
						.WhereIf(input.MinTotalTaxFilter != null, e => e.TotalTax >= input.MinTotalTaxFilter)
						.WhereIf(input.MaxTotalTaxFilter != null, e => e.TotalTax <= input.MaxTotalTaxFilter)
						.WhereIf(input.MinTotalPriceFilter != null, e => e.TotalPrice >= input.MinTotalPriceFilter)
						.WhereIf(input.MaxTotalPriceFilter != null, e => e.TotalPrice <= input.MaxTotalPriceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorChargeStatusStatusFilter), e => e.VendorChargeStatusFk != null && e.VendorChargeStatusFk.Status.ToLower() == input.VendorChargeStatusStatusFilter.ToLower().Trim());

			var pagedAndFilteredVendorCharges = filteredVendorCharges
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var vendorCharges = from o in pagedAndFilteredVendorCharges
                         join o1 in _lookup_vendorRepository.GetAll() on o.VendorId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_vendorChargeStatusRepository.GetAll() on o.VendorChargeStatusId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         select new GetVendorChargeForViewDto() {
							VendorCharge = new VendorChargeDto
							{
                                Reference = o.Reference,
                                Description = o.Description,
                                DateIssued = o.DateIssued,
                                DateDue = o.DateDue,
                                TotalTax = o.TotalTax,
                                TotalPrice = o.TotalPrice,
                                Id = o.Id
							},
                         	VendorName = s1 == null ? "" : s1.Name.ToString(),
                         	SupportContractTitle = s2 == null ? "" : s2.Title.ToString(),
                         	WorkOrderSubject = s3 == null ? "" : s3.Subject.ToString(),
                         	VendorChargeStatusStatus = s4 == null ? "" : s4.Status.ToString()
						};

            var totalCount = await filteredVendorCharges.CountAsync();

            return new PagedResultDto<GetVendorChargeForViewDto>(
                totalCount,
                await vendorCharges.ToListAsync()
            );
         }
		 
		 public async Task<GetVendorChargeForViewDto> GetVendorChargeForView(int id)
         {
            var vendorCharge = await _vendorChargeRepository.GetAsync(id);

            var output = new GetVendorChargeForViewDto { VendorCharge = ObjectMapper.Map<VendorChargeDto>(vendorCharge) };

		    if (output.VendorCharge.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.VendorCharge.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

		    if (output.VendorCharge.SupportContractId != null)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.VendorCharge.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

		    if (output.VendorCharge.WorkOrderId != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.VendorCharge.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

		    if (output.VendorCharge.VendorChargeStatusId != null)
            {
                var _lookupVendorChargeStatus = await _lookup_vendorChargeStatusRepository.FirstOrDefaultAsync((int)output.VendorCharge.VendorChargeStatusId);
                output.VendorChargeStatusStatus = _lookupVendorChargeStatus.Status.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_Edit)]
		 public async Task<GetVendorChargeForEditOutput> GetVendorChargeForEdit(EntityDto input)
         {
            var vendorCharge = await _vendorChargeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetVendorChargeForEditOutput {VendorCharge = ObjectMapper.Map<CreateOrEditVendorChargeDto>(vendorCharge)};

		    if (output.VendorCharge.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.VendorCharge.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

		    if (output.VendorCharge.SupportContractId != null)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.VendorCharge.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

		    if (output.VendorCharge.WorkOrderId != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.VendorCharge.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

		    if (output.VendorCharge.VendorChargeStatusId != null)
            {
                var _lookupVendorChargeStatus = await _lookup_vendorChargeStatusRepository.FirstOrDefaultAsync((int)output.VendorCharge.VendorChargeStatusId);
                output.VendorChargeStatusStatus = _lookupVendorChargeStatus.Status.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditVendorChargeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_Create)]
		 protected virtual async Task Create(CreateOrEditVendorChargeDto input)
         {
            var vendorCharge = ObjectMapper.Map<VendorCharge>(input);

			
			if (AbpSession.TenantId != null)
			{
				vendorCharge.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _vendorChargeRepository.InsertAsync(vendorCharge);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_Edit)]
		 protected virtual async Task Update(CreateOrEditVendorChargeDto input)
         {
            var vendorCharge = await _vendorChargeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, vendorCharge);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_VendorCharges_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _vendorChargeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetVendorChargesToExcel(GetAllVendorChargesForExcelInput input)
         {
			
			var filteredVendorCharges = _vendorChargeRepository.GetAll()
						.Include( e => e.VendorFk)
						.Include( e => e.SupportContractFk)
						.Include( e => e.WorkOrderFk)
						.Include( e => e.VendorChargeStatusFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
						.WhereIf(input.MinDateIssuedFilter != null, e => e.DateIssued >= input.MinDateIssuedFilter)
						.WhereIf(input.MaxDateIssuedFilter != null, e => e.DateIssued <= input.MaxDateIssuedFilter)
						.WhereIf(input.MinDateDueFilter != null, e => e.DateDue >= input.MinDateDueFilter)
						.WhereIf(input.MaxDateDueFilter != null, e => e.DateDue <= input.MaxDateDueFilter)
						.WhereIf(input.MinTotalTaxFilter != null, e => e.TotalTax >= input.MinTotalTaxFilter)
						.WhereIf(input.MaxTotalTaxFilter != null, e => e.TotalTax <= input.MaxTotalTaxFilter)
						.WhereIf(input.MinTotalPriceFilter != null, e => e.TotalPrice >= input.MinTotalPriceFilter)
						.WhereIf(input.MaxTotalPriceFilter != null, e => e.TotalPrice <= input.MaxTotalPriceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorChargeStatusStatusFilter), e => e.VendorChargeStatusFk != null && e.VendorChargeStatusFk.Status.ToLower() == input.VendorChargeStatusStatusFilter.ToLower().Trim());

			var query = (from o in filteredVendorCharges
                         join o1 in _lookup_vendorRepository.GetAll() on o.VendorId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_vendorChargeStatusRepository.GetAll() on o.VendorChargeStatusId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         select new GetVendorChargeForViewDto() { 
							VendorCharge = new VendorChargeDto
							{
                                Reference = o.Reference,
                                Description = o.Description,
                                DateIssued = o.DateIssued,
                                DateDue = o.DateDue,
                                TotalTax = o.TotalTax,
                                TotalPrice = o.TotalPrice,
                                Id = o.Id
							},
                         	VendorName = s1 == null ? "" : s1.Name.ToString(),
                         	SupportContractTitle = s2 == null ? "" : s2.Title.ToString(),
                         	WorkOrderSubject = s3 == null ? "" : s3.Subject.ToString(),
                         	VendorChargeStatusStatus = s4 == null ? "" : s4.Status.ToString()
						 });


            var vendorChargeListDtos = await query.ToListAsync();

            return _vendorChargesExcelExporter.ExportToFile(vendorChargeListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_VendorCharges)]
         public async Task<PagedResultDto<VendorChargeVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_vendorRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<VendorChargeVendorLookupTableDto>();
			foreach(var vendor in vendorList){
				lookupTableDtoList.Add(new VendorChargeVendorLookupTableDto
				{
					Id = vendor.Id,
					DisplayName = vendor.Name?.ToString()
				});
			}

            return new PagedResultDto<VendorChargeVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_VendorCharges)]
         public async Task<PagedResultDto<VendorChargeSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_supportContractRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Title.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var supportContractList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<VendorChargeSupportContractLookupTableDto>();
			foreach(var supportContract in supportContractList){
				lookupTableDtoList.Add(new VendorChargeSupportContractLookupTableDto
				{
					Id = supportContract.Id,
					DisplayName = supportContract.Title?.ToString()
				});
			}

            return new PagedResultDto<VendorChargeSupportContractLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_VendorCharges)]
         public async Task<PagedResultDto<VendorChargeWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_workOrderRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Subject.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var workOrderList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<VendorChargeWorkOrderLookupTableDto>();
			foreach(var workOrder in workOrderList){
				lookupTableDtoList.Add(new VendorChargeWorkOrderLookupTableDto
				{
					Id = workOrder.Id,
					DisplayName = workOrder.Subject?.ToString()
				});
			}

            return new PagedResultDto<VendorChargeWorkOrderLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_VendorCharges)]
         public async Task<PagedResultDto<VendorChargeVendorChargeStatusLookupTableDto>> GetAllVendorChargeStatusForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_vendorChargeStatusRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Status.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var vendorChargeStatusList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<VendorChargeVendorChargeStatusLookupTableDto>();
			foreach(var vendorChargeStatus in vendorChargeStatusList){
				lookupTableDtoList.Add(new VendorChargeVendorChargeStatusLookupTableDto
				{
					Id = vendorChargeStatus.Id,
					DisplayName = vendorChargeStatus.Status?.ToString()
				});
			}

            return new PagedResultDto<VendorChargeVendorChargeStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}