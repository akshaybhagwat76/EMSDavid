using Ems.Telematics;
using Ems.Storage;
using Ems.Assets;
using Ems.Quotations;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Assets.Exporting;
using Ems.Assets.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Assets.Dto;

namespace Ems.Assets
{
	[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
    public class AssetPartsAppService : EmsAppServiceBase, IAssetPartsAppService
    {
		 private readonly IRepository<AssetPart> _assetPartRepository;
		 private readonly IAssetPartsExcelExporter _assetPartsExcelExporter;
		 private readonly IRepository<AssetPartType,int> _lookup_assetPartTypeRepository;
		 private readonly IRepository<AssetPart,int> _lookup_assetPartRepository;
		 private readonly IRepository<AssetPartStatus,int> _lookup_assetPartStatusRepository;
		 private readonly IRepository<UsageMetric,int> _lookup_usageMetricRepository;
		 private readonly IRepository<Attachment,int> _lookup_attachmentRepository;
		 private readonly IRepository<Asset,int> _lookup_assetRepository;
		 private readonly IRepository<ItemType,int> _lookup_itemTypeRepository;
		 

		  public AssetPartsAppService(IRepository<AssetPart> assetPartRepository, IAssetPartsExcelExporter assetPartsExcelExporter , IRepository<AssetPartType, int> lookup_assetPartTypeRepository, IRepository<AssetPart, int> lookup_assetPartRepository, IRepository<AssetPartStatus, int> lookup_assetPartStatusRepository, IRepository<UsageMetric, int> lookup_usageMetricRepository, IRepository<Attachment, int> lookup_attachmentRepository, IRepository<Asset, int> lookup_assetRepository, IRepository<ItemType, int> lookup_itemTypeRepository) 
		  {
			_assetPartRepository = assetPartRepository;
			_assetPartsExcelExporter = assetPartsExcelExporter;
			_lookup_assetPartTypeRepository = lookup_assetPartTypeRepository;
		    _lookup_assetPartRepository = lookup_assetPartRepository;
		    _lookup_assetPartStatusRepository = lookup_assetPartStatusRepository;
		    _lookup_usageMetricRepository = lookup_usageMetricRepository;
		    _lookup_attachmentRepository = lookup_attachmentRepository;
		    _lookup_assetRepository = lookup_assetRepository;
		    _lookup_itemTypeRepository = lookup_itemTypeRepository;
		
		  }

        /*
        public async Task<ListResultDto<AssetPartDto>> GetAssetParts()
        {
            var query = from ou in _assetPartRepository.GetAll()
                        select new
                        {
                            ou
                        };

            var items = await query.ToListAsync();

            return new ListResultDto<AssetPartDto>(
                items.Select(item =>
                {
                    var assetPartDto = ObjectMapper.Map<AssetPartDto>(item.ou);
                    return assetPartDto;
                }).ToList());
        }
        */

        public async Task<ListResultDto<AssetPartExtendedDto>> GetAssetParts(int assetId)
        {
            var query = from o in _assetPartRepository.GetAll().Where(a => a.AssetId == assetId)
                        join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                        from s1 in j1.DefaultIfEmpty()

                        join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                        from s2 in j2.DefaultIfEmpty()

                        join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                        from s3 in j3.DefaultIfEmpty()

                        join o4 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o4.Id into j4
                        from s4 in j4.DefaultIfEmpty()

                        join o5 in _lookup_attachmentRepository.GetAll() on o.AttachmentId equals o5.Id into j5
                        from s5 in j5.DefaultIfEmpty()

                        join o6 in _lookup_assetRepository.GetAll() on o.AssetId equals o6.Id into j6
                        from s6 in j6.DefaultIfEmpty()

                        join o7 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o7.Id into j7
                        from s7 in j7.DefaultIfEmpty()

                        select new AssetPartExtendedDto()
                        {
                            Name = o.Name,
                            Description = o.Description,
                            SerialNumber = o.SerialNumber,
                            InstallDate = o.InstallDate,
                            Code = o.Code,
                            Installed = o.Installed,
                            Id = o.Id,
                            ParentId = o.ParentId,
                            AssetId = o.AssetId,
                            //AssetId = assetId,
                            AssetPartType = s1 == null ? "" : s1.Type.ToString(),
                            AssetPartStatus = s3 == null ? "" : s3.Status.ToString(),
                            AssetReference = s6 == null ? "" : s6.Reference.ToString(),
                            ItemType = s7 == null ? "" : s7.Type.ToString()
                        };

            var items = await query.ToListAsync();
            //items = items.Where(a => a.AssetId == assetId).ToList();

            return new ListResultDto<AssetPartExtendedDto>(
                items.Select(item =>
                {
                    var extendedAssetPartDto = ObjectMapper.Map<AssetPartExtendedDto>(item);
                    return extendedAssetPartDto;
                }).ToList());
        }

        /*
        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> CreateAssetPart(CreateOrEditAssetPartDto input)
        {
            var assetPart = new AssetPart(
                AbpSession.TenantId,
                input., input.ParentId);
            input.
            await CreateAsync(assetPart);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<AssetPartDto>(assetPart);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> UpdateAssetPart(UpdateAssetPartInput input)
        {
            var assetPart = await _assetPartRepository.GetAsync(input.Id);

            assetPart.DisplayName = input.DisplayName;

            await UpdateAsync(assetPart);

            return await CreateAssetPartDto(assetPart);
        }
        */




        /*
        private async Task<AssetPartDto> CreateAssetPartDto(AssetPart assetPart)
        {
            var dto = ObjectMapper.Map<AssetPartDto>(assetPart);
            dto.MemberCount = await _userAssetPartRepository.CountAsync(uou => uou.AssetPartId == assetPart.Id);
            return dto;
        }
        */

        [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_ManagePartTree)]
        public async Task<AssetPartDto> MoveAssetPart(MoveAssetPartInput input)
        {
            Move(input.Id, input.NewParentId);

            var dto = ObjectMapper.Map<AssetPartDto>(await _assetPartRepository.GetAsync(input.Id));

            return dto;
        }

        private void Move(int id, int? parentId)
        {
            // NOTE: Originally copied the pattern found in Organization Unit that used the field "Code"
            //          to represent the tree structure, but then discovered that it is not needed, so
            //          commented out the relevant lines of code.  Might be needed in future to enable 
            //          functions such as recursive operations.

            // Get the Part that needs to be moved
            var partToMove = _assetPartRepository.Get(id);
            //var partToMoveOldCode = partToMove.Code;

            // If it has been droped into the root
            if (parentId == null)
            {
                var allParts = _assetPartRepository.GetAll().OrderBy(p => p.Code).ToList();
                //var lastRootCode = allParts.Last().Code.Substring(0, 5);
                //var lastCodeNumber = Convert.ToInt32(lastRootCode);
                //var nextRootCode = lastCodeNumber + 1.ToString().PadLeft(5, '0');

                //partToMove.Code = nextRootCode;
                partToMove.ParentId = null;
                _assetPartRepository.Update(partToMove);
                return;
            }

            // Store the current Parent ID of the part to be moved
            var sourceParentId = partToMove.ParentId;
            var destinationParentId = parentId;

            // Store the original parent code and ID
            //var originalParentId = partToMove.Code;
            //var originalParentCode = (partToMove.Code.Length > 6) ?  partToMove.Code.Substring(0, partToMove.Code.Length - 6) : "";

            // Get the new Parent Part
            var destinationParent = GetDestinationParentOrNull(parentId);

            // find the next code under the new parent... 
            //var nextChildCodeOfDestinationParent = GetNextChildCode(destinationParentId);

            // Update the ParentId, and Code of the moved Part to be the next available code
            //partToMove.Code = string.Format("{0}.{1}", destinationParent.Code, nextChildCodeOfDestinationParent);
            partToMove.ParentId = destinationParent.Id;
            _assetPartRepository.Update(partToMove);

            /*
            // Update the code of all children by replacing the old code of the moved part with the new code of the moved part.
            var allMovedPartChildren = FindChildrenOrNull(partToMove.Id);
            if (allMovedPartChildren != null)
            {
                foreach (var child in allMovedPartChildren)
                {
                    child.Code = string.Format("{0}.{1}", partToMove.Code, child.Code.Substring(partToMoveOldCode.Length));
                    _assetPartRepository.Update(child);
                }
            }

            // Update all the children of the old Parent to ensure sequential codes
            var allChildrenOfOldParent = FindChildrenOrNull(sourceParentId);
            var index = 1;

            if (allChildrenOfOldParent != null)
            {
                foreach (var child in allChildrenOfOldParent)
                {
                    child.Code = string.Format("{0}.{1}", partToMoveOldCode, index.ToString().PadLeft(5, '0'));
                    _assetPartRepository.Update(child);
                    index = index + 1;
                }

                // Note: Need to extend this code to walk down every branch of the tree structure
            }
            */
        }

        private List<AssetPart> FindChildrenOrNull(int? parentId, bool recursive = false)
        {
            if (parentId == null) return null;

            if (!recursive)
            {
                var children = _assetPartRepository.GetAll().Where(p => p.ParentId == parentId).OrderBy(p => p.Code).ToList();
                if (children != null)
                {
                    if (children.Count != 0)
                    {
                        return children;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            else
            {
                var parentCode = string.Format("{0}.", _assetPartRepository.Get((int)parentId).Code);
                var children = _assetPartRepository.GetAll().Where(p => p.Code.Contains(parentCode)).OrderBy(p => p.Code).ToList();
                if (children != null)
                {
                    if (children.Count != 0)
                    {
                        return children;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }

        }

        private AssetPart GetDestinationParentOrNull(int? parentId)
        {
            if (parentId == null)
            {
                return null;
            }
            return _assetPartRepository.Get((int)parentId);
        }

        private string GetNextChildCode(int? parentId)
        {
            var children = FindChildrenOrNull(parentId);

            if (children != null)
            {
                if (children.Count != 0)
                {
                    var lastChild = children.Last();

                    var codeNumberElements = lastChild.Code.Split('.').ToList();
                    var lastCodeNumber = Convert.ToInt32(codeNumberElements.Last());

                    var nextCodeNumber = lastCodeNumber + 1;
                    var nextCode = nextCodeNumber.ToString().PadLeft(5, '0');

                    return nextCode;
                }
                else
                {
                    return "00001";
                }
            }
            else
            {
                return "00001";
            }
        }

        public async Task<PagedResultDto<GetAssetPartForViewDto>> GetAll(GetAllAssetPartsInput input)
         {
			
			var filteredAssetParts = _assetPartRepository.GetAll()
						.Include( e => e.AssetPartTypeFk)
						.Include( e => e.ParentFk)
						.Include( e => e.AssetPartStatusFk)
						.Include( e => e.UsageMetricFk)
						.Include( e => e.AttachmentFk)
						.Include( e => e.AssetFk)
						.Include( e => e.ItemTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.Code.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter),  e => e.SerialNumber == input.SerialNumberFilter)
						.WhereIf(input.MinInstallDateFilter != null, e => e.InstallDate >= input.MinInstallDateFilter)
						.WhereIf(input.MaxInstallDateFilter != null, e => e.InstallDate <= input.MaxInstallDateFilter)
						.WhereIf(input.InstalledFilter > -1,  e => (input.InstalledFilter == 1 && e.Installed) || (input.InstalledFilter == 0 && !e.Installed) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartTypeTypeFilter), e => e.AssetPartTypeFk != null && e.AssetPartTypeFk.Type == input.AssetPartTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.ParentFk != null && e.ParentFk.Name == input.AssetPartNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartStatusStatusFilter), e => e.AssetPartStatusFk != null && e.AssetPartStatusFk.Status == input.AssetPartStatusStatusFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric == input.UsageMetricMetricFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AttachmentFilenameFilter), e => e.AttachmentFk != null && e.AttachmentFk.Filename == input.AttachmentFilenameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter);

			var pagedAndFilteredAssetParts = filteredAssetParts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetParts = from o in pagedAndFilteredAssetParts
                         join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_attachmentRepository.GetAll() on o.AttachmentId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_assetRepository.GetAll() on o.AssetId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         join o7 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()
                         
                         select new GetAssetPartForViewDto() {
							AssetPart = new AssetPartDto
							{
                                Name = o.Name,
                                Description = o.Description,
                                SerialNumber = o.SerialNumber,
                                InstallDate = o.InstallDate,
                                Code = o.Code,
                                Installed = o.Installed,
                                Id = o.Id
							},
                         	AssetPartTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	AssetPartName = s2 == null ? "" : s2.Name.ToString(),
                         	AssetPartStatusStatus = s3 == null ? "" : s3.Status.ToString(),
                         	UsageMetricMetric = s4 == null ? "" : s4.Metric.ToString(),
                         	AttachmentFilename = s5 == null ? "" : s5.Filename.ToString(),
                         	AssetReference = s6 == null ? "" : s6.Reference.ToString(),
                         	ItemTypeType = s7 == null ? "" : s7.Type.ToString()
						};

            var totalCount = await filteredAssetParts.CountAsync();

            return new PagedResultDto<GetAssetPartForViewDto>(
                totalCount,
                await assetParts.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetPartForViewDto> GetAssetPartForView(int id)
         {
            var assetPart = await _assetPartRepository.GetAsync(id);

            var output = new GetAssetPartForViewDto { AssetPart = ObjectMapper.Map<AssetPartDto>(assetPart) };

		    if (output.AssetPart.AssetPartTypeId != null)
            {
                var _lookupAssetPartType = await _lookup_assetPartTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartTypeId);
                output.AssetPartTypeType = _lookupAssetPartType.Type.ToString();
            }

		    if (output.AssetPart.ParentId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.AssetPart.ParentId);
                output.AssetPartName = _lookupAssetPart.Name.ToString();
            }

		    if (output.AssetPart.AssetPartStatusId != null)
            {
                var _lookupAssetPartStatus = await _lookup_assetPartStatusRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartStatusId);
                output.AssetPartStatusStatus = _lookupAssetPartStatus.Status.ToString();
            }

		    if (output.AssetPart.UsageMetricId != null)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.AssetPart.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

		    if (output.AssetPart.AttachmentId != null)
            {
                var _lookupAttachment = await _lookup_attachmentRepository.FirstOrDefaultAsync((int)output.AssetPart.AttachmentId);
                output.AttachmentFilename = _lookupAttachment.Filename.ToString();
            }

		    if (output.AssetPart.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

		    if (output.AssetPart.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Edit)]
		 public async Task<GetAssetPartForEditOutput> GetAssetPartForEdit(EntityDto input)
         {
            var assetPart = await _assetPartRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetPartForEditOutput {AssetPart = ObjectMapper.Map<CreateOrEditAssetPartDto>(assetPart)};

		    if (output.AssetPart.AssetPartTypeId != null)
            {
                var _lookupAssetPartType = await _lookup_assetPartTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartTypeId);
                output.AssetPartTypeType = _lookupAssetPartType.Type.ToString();
            }

		    if (output.AssetPart.ParentId != null)
            {
                var _lookupAssetPart = await _lookup_assetPartRepository.FirstOrDefaultAsync((int)output.AssetPart.ParentId);
                output.AssetPartName = _lookupAssetPart.Name.ToString();
            }

		    if (output.AssetPart.AssetPartStatusId != null)
            {
                var _lookupAssetPartStatus = await _lookup_assetPartStatusRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetPartStatusId);
                output.AssetPartStatusStatus = _lookupAssetPartStatus.Status.ToString();
            }

		    if (output.AssetPart.UsageMetricId != null)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.AssetPart.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

		    if (output.AssetPart.AttachmentId != null)
            {
                var _lookupAttachment = await _lookup_attachmentRepository.FirstOrDefaultAsync((int)output.AssetPart.AttachmentId);
                output.AttachmentFilename = _lookupAttachment.Filename.ToString();
            }

		    if (output.AssetPart.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetPart.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

		    if (output.AssetPart.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.AssetPart.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }
			
            return output;
         }

		 public async Task<CreateOrEditAssetPartDto> CreateOrEdit(CreateOrEditAssetPartDto input)
         {
            if(input.Id == null){
				input.Id = await Create(input);
			}
			else{
				await Update(input);
			}
            return ObjectMapper.Map<CreateOrEditAssetPartDto>(input);
        }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Create)]
		 protected virtual async Task<int> Create(CreateOrEditAssetPartDto input)
         {
            var assetPart = ObjectMapper.Map<AssetPart>(input);
			
			if (AbpSession.TenantId != null)
			{
				assetPart.TenantId = (int?) AbpSession.TenantId;
			}

            await _assetPartRepository.InsertAsync(assetPart);
            await CurrentUnitOfWork.SaveChangesAsync();

            return assetPart.Id;
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetPartDto input)
         {
            var assetPart = await _assetPartRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetPart);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetParts_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetPartRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAssetPartsToExcel(GetAllAssetPartsForExcelInput input)
         {
			
			var filteredAssetParts = _assetPartRepository.GetAll()
						.Include( e => e.AssetPartTypeFk)
						.Include( e => e.ParentFk)
						.Include( e => e.AssetPartStatusFk)
						.Include( e => e.UsageMetricFk)
						.Include( e => e.AttachmentFk)
						.Include( e => e.AssetFk)
						.Include( e => e.ItemTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.SerialNumber.Contains(input.Filter) || e.Code.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.SerialNumberFilter),  e => e.SerialNumber == input.SerialNumberFilter)
						.WhereIf(input.MinInstallDateFilter != null, e => e.InstallDate >= input.MinInstallDateFilter)
						.WhereIf(input.MaxInstallDateFilter != null, e => e.InstallDate <= input.MaxInstallDateFilter)
						.WhereIf(input.InstalledFilter > -1,  e => (input.InstalledFilter == 1 && e.Installed) || (input.InstalledFilter == 0 && !e.Installed) )
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartTypeTypeFilter), e => e.AssetPartTypeFk != null && e.AssetPartTypeFk.Type == input.AssetPartTypeTypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartNameFilter), e => e.ParentFk != null && e.ParentFk.Name == input.AssetPartNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetPartStatusStatusFilter), e => e.AssetPartStatusFk != null && e.AssetPartStatusFk.Status == input.AssetPartStatusStatusFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric == input.UsageMetricMetricFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AttachmentFilenameFilter), e => e.AttachmentFk != null && e.AttachmentFk.Filename == input.AttachmentFilenameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference == input.AssetReferenceFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type == input.ItemTypeTypeFilter);

			var query = (from o in filteredAssetParts
                         join o1 in _lookup_assetPartTypeRepository.GetAll() on o.AssetPartTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetPartRepository.GetAll() on o.ParentId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_assetPartStatusRepository.GetAll() on o.AssetPartStatusId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_attachmentRepository.GetAll() on o.AttachmentId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_assetRepository.GetAll() on o.AssetId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         join o7 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()
                         
                         select new GetAssetPartForViewDto() { 
							AssetPart = new AssetPartDto
							{
                                Name = o.Name,
                                Description = o.Description,
                                SerialNumber = o.SerialNumber,
                                InstallDate = o.InstallDate,
                                Code = o.Code,
                                Installed = o.Installed,
                                Id = o.Id
							},
                         	AssetPartTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	AssetPartName = s2 == null ? "" : s2.Name.ToString(),
                         	AssetPartStatusStatus = s3 == null ? "" : s3.Status.ToString(),
                         	UsageMetricMetric = s4 == null ? "" : s4.Metric.ToString(),
                         	AttachmentFilename = s5 == null ? "" : s5.Filename.ToString(),
                         	AssetReference = s6 == null ? "" : s6.Reference.ToString(),
                         	ItemTypeType = s7 == null ? "" : s7.Type.ToString()
						 });


            var assetPartListDtos = await query.ToListAsync();

            return _assetPartsExcelExporter.ExportToFile(assetPartListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetPartTypeLookupTableDto>> GetAllAssetPartTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetPartTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Type != null ? e.Type.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetPartTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetPartTypeLookupTableDto>();
			foreach(var assetPartType in assetPartTypeList){
				lookupTableDtoList.Add(new AssetPartAssetPartTypeLookupTableDto
				{
					Id = assetPartType.Id,
					DisplayName = assetPartType.Type?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetPartTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetPartLookupTableDto>> GetAllAssetPartForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetPartRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Name != null ? e.Name.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetPartList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetPartLookupTableDto>();
			foreach(var assetPart in assetPartList){
				lookupTableDtoList.Add(new AssetPartAssetPartLookupTableDto
				{
					Id = assetPart.Id,
					DisplayName = assetPart.Name?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetPartLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetPartStatusLookupTableDto>> GetAllAssetPartStatusForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetPartStatusRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Status != null ? e.Status.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetPartStatusList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetPartStatusLookupTableDto>();
			foreach(var assetPartStatus in assetPartStatusList){
				lookupTableDtoList.Add(new AssetPartAssetPartStatusLookupTableDto
				{
					Id = assetPartStatus.Id,
					DisplayName = assetPartStatus.Status?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetPartStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartUsageMetricLookupTableDto>> GetAllUsageMetricForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_usageMetricRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Metric != null ? e.Metric.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var usageMetricList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartUsageMetricLookupTableDto>();
			foreach(var usageMetric in usageMetricList){
				lookupTableDtoList.Add(new AssetPartUsageMetricLookupTableDto
				{
					Id = usageMetric.Id,
					DisplayName = usageMetric.Metric?.ToString()
				});
			}

            return new PagedResultDto<AssetPartUsageMetricLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAttachmentLookupTableDto>> GetAllAttachmentForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_attachmentRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Filename != null ? e.Filename.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var attachmentList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAttachmentLookupTableDto>();
			foreach(var attachment in attachmentList){
				lookupTableDtoList.Add(new AssetPartAttachmentLookupTableDto
				{
					Id = attachment.Id,
					DisplayName = attachment.Filename?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAttachmentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Reference != null ? e.Reference.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartAssetLookupTableDto>();
			foreach(var asset in assetList){
				lookupTableDtoList.Add(new AssetPartAssetLookupTableDto
				{
					Id = asset.Id,
					DisplayName = asset.Reference?.ToString()
				});
			}

            return new PagedResultDto<AssetPartAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetParts)]
         public async Task<PagedResultDto<AssetPartItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_itemTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> (e.Type != null ? e.Type.ToString():"").Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var itemTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetPartItemTypeLookupTableDto>();
			foreach(var itemType in itemTypeList){
				lookupTableDtoList.Add(new AssetPartItemTypeLookupTableDto
				{
					Id = itemType.Id,
					DisplayName = itemType.Type?.ToString()
				});
			}

            return new PagedResultDto<AssetPartItemTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}