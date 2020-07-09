using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Ems.Authorization;
using Ems.Metrics;
using Ems.Quotations;
using Ems.Support.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Main_Estimates)]
    public class EstimateDetailsAppService : EmsAppServiceBase, IEstimateDetailsAppService
    {
        private readonly IRepository<EstimateDetail> _estimateDetailRepository;
        private readonly IRepository<Estimate> _estimateRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<WorkOrderAction, int> _lookup_workOrderActionRepository;

        public EstimateDetailsAppService(IRepository<EstimateDetail> estimateDetailRepository, IRepository<Estimate> estimateRepository, IRepository<ItemType, int> lookup_itemTypeRepository, IRepository<Uom, int> lookup_uomRepository, IRepository<WorkOrderAction, int> lookup_workOrderActionRepository)
        {
            _estimateDetailRepository = estimateDetailRepository;
            _estimateRepository = estimateRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_workOrderActionRepository = lookup_workOrderActionRepository;
        }

        public async Task<PagedResultDto<GetEstimateDetailForViewDto>> GetAll(GetAllEstimateDetailsInput input)
        {
            var filteredEstimateDetails = _estimateDetailRepository.GetAll()
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.EstimateFk)
                        .Include(e => e.UomFk)
                        .Include(e=> e.WorkOrderActionFk)
                        .WhereIf(input.EstimateId > 0, e => e.EstimateId == input.EstimateId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type.ToLower().Contains(input.ItemTypeTypeFilter.ToLower().Trim()))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EstimateTitleFilter), e => e.EstimateFk != null && e.EstimateFk.Title.ToLower().Contains(input.EstimateTitleFilter.ToLower().Trim()))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower().Contains(input.UomUnitOfMeasurementFilter.ToLower().Trim()))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ActionWorkOrderActionFilter), e => e.WorkOrderActionFk != null && e.WorkOrderActionFk.Action.ToLower().Contains(input.ActionWorkOrderActionFilter.ToLower().Trim()));

            var pagedAndFilteredEstimateDetails = filteredEstimateDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var estimateDetails = from o in pagedAndFilteredEstimateDetails

                                   join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                                   from s3 in j3.DefaultIfEmpty()

                                   join o5 in _estimateRepository.GetAll() on o.EstimateId equals o5.Id into j5
                                   from s5 in j5.DefaultIfEmpty()

                                   join o6 in _lookup_uomRepository.GetAll() on o.UomId equals o6.Id into j6
                                   from s6 in j6.DefaultIfEmpty()

                                  join o7 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o7.Id into j7
                                  from s7 in j7.DefaultIfEmpty()

                                  select new GetEstimateDetailForViewDto()
                                   {
                                       EstimateDetail = new EstimateDetailDto
                                       {
                                           Description = o.Description,
                                           Quantity = o.Quantity,
                                           UnitPrice = o.UnitPrice,
                                           Cost = o.Cost,
                                           Tax = o.Tax,
                                           Charge = o.Charge,
                                           Discount = o.Discount,
                                           MarkUp = o.MarkUp,
                                           IsChargeable = o.IsChargeable,
                                           IsAdHoc = o.IsAdHoc,
                                           IsStandbyReplacementUnit = o.IsStandbyReplacementUnit,
                                           IsOptionalItem = o.IsOptionalItem,
                                           Remark = o.Remark,
                                           Loc8GUID = o.Loc8GUID,
                                           EstimateId = o.EstimateId,
                                           Id = o.Id
                                       },
                                       ItemTypeType = s3 == null ? "" : s3.Type.ToString(),
                                       EstimateTitle = s5 == null ? "" : s5.Title.ToString(),
                                       UomUnitOfMeasurement = s6 == null ? "" : s6.UnitOfMeasurement.ToString(),
                                       ActionWorkOrderAction = s7 == null ? "" : s7.Action
                                   };

            var totalCount = await filteredEstimateDetails.CountAsync();

            return new PagedResultDto<GetEstimateDetailForViewDto>(
                totalCount,
                await estimateDetails.ToListAsync()
            );
        }


        public async Task<GetEstimateDetailForViewDto> GetEstimateDetailForView(int id)
        {
            var estimateDetail = await _estimateDetailRepository.GetAsync(id);
            var output = new GetEstimateDetailForViewDto { EstimateDetail = ObjectMapper.Map<EstimateDetailDto>(estimateDetail) };

            if (output.EstimateDetail.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.EstimateDetail.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.EstimateDetail != null && output.EstimateDetail.EstimateId > 0)
            {
                var _lookupEstimate = await _estimateRepository.FirstOrDefaultAsync((int)output.EstimateDetail.EstimateId);
                output.EstimateTitle = _lookupEstimate.Title.ToString();
            }

            if (output.EstimateDetail.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.EstimateDetail.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.EstimateDetail.WorkOrderActionId != null)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.EstimateDetail.WorkOrderActionId);
                output.ActionWorkOrderAction = _lookupWorkOrderAction.Action;
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_EstimateDetailsEdit)]
        public async Task<GetEstimateDetailForEditOutput> GetEstimateDetailForEdit(EntityDto input)
        {
            var estimateDetail = await _estimateDetailRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEstimateDetailForEditOutput { EstimateDetail = ObjectMapper.Map<CreateOrEditEstimateDetailDto>(estimateDetail) };

            if (output.EstimateDetail.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.EstimateDetail.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.EstimateDetail != null && output.EstimateDetail.EstimateId > 0)
            {
                var _lookupEstimate = await _estimateRepository.FirstOrDefaultAsync((int)output.EstimateDetail.EstimateId);
                output.EstimateTitle = _lookupEstimate.Title.ToString();
            }

            if (output.EstimateDetail.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.EstimateDetail.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.EstimateDetail.WorkOrderActionId != null)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.EstimateDetail.WorkOrderActionId);
                output.ActionWorkOrderAction = _lookupWorkOrderAction.Action;
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEstimateDetailDto input)
        {
            decimal discountPrice = 0, taxPrice = 0;

            decimal markUp = input.MarkUp;
            decimal unitPrice = input.UnitPrice;
            decimal quantity = input.Quantity;
            decimal tax = input.Tax;
            decimal discount = input.Discount;
            decimal costPrice = unitPrice * quantity;

            if (markUp > 0)
                costPrice += costPrice * (markUp / 100);

            if (discount > 0)
                discountPrice = costPrice * (discount / 100);

            if (tax > 0)
                taxPrice = (costPrice - discountPrice) * (tax / 100);

            input.Cost = costPrice;
            input.Charge = (costPrice - discountPrice) + taxPrice;

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_EstimateDetailsCreate)]
        protected virtual async Task Create(CreateOrEditEstimateDetailDto input)
        {
            var estimateDetail = ObjectMapper.Map<EstimateDetail>(input);

            if (AbpSession.TenantId != null)
            {
                estimateDetail.TenantId = (int?)AbpSession.TenantId;
            }

            await _estimateDetailRepository.InsertAsync(estimateDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_EstimateDetailsEdit)]
        protected virtual async Task Update(CreateOrEditEstimateDetailDto input)
        {
            var estimateDetail = await _estimateDetailRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, estimateDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_EstimateDetailsDelete)]
        public async Task Delete(EntityDto input)
        {
            await _estimateDetailRepository.DeleteAsync(input.Id);
        }

    }
}
