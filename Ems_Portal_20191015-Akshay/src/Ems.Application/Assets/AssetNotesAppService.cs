using Abp.Application.Services.Dto;
using Abp.Authorization;
using System.Threading.Tasks;
using Ems.Assets.Dtos;
using Ems.Authorization;
using Abp.Domain.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Ems.Authorization.Users;

namespace Ems.Assets
{
    [AbpAuthorize(AppPermissions.Pages_Main_Assets)]
    public class AssetNotesAppService : EmsAppServiceBase, IAssetNotesAppService
    {
        private readonly IRepository<AssetNote> _assetNotesRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<User, long> _lookup_userRepository;

        public AssetNotesAppService(IRepository<AssetNote> assetNotesRepository, IRepository<Asset, int> lookup_assetRepository, IRepository<User, long> lookup_userRepository)
        {
            _assetNotesRepository = assetNotesRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_userRepository = lookup_userRepository;
        }

        public async Task<PagedResultDto<GetAssetNotesForViewDto>> GetAll(int assetId, PagedAndSortedResultRequestDto input)
        {
            var filteredAssetNotes = _assetNotesRepository.GetAll()
                        .Include(e => e.AssetFk)
                        .Include(e => e.UserFk)
                        .Where(e => e.AssetId == assetId);

            var pagedAndFilteredAssetNotes = filteredAssetNotes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var assetNotes = from o in pagedAndFilteredAssetNotes
                             join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             join o2 in _lookup_userRepository.GetAll() on o.UserId equals o2.Id into j2
                             from s2 in j2.DefaultIfEmpty()

                             select new GetAssetNotesForViewDto()
                             {
                                 AssetNotes = new AssetNotesDto
                                 {
                                     AssetId = o.AssetId,
                                     Notes = o.Notes,
                                     TenantId = o.TenantId,
                                     Title = o.Title,
                                     UserId = o.UserId,
                                     Id = o.Id
                                 },
                                 AssetReference = s1 == null ? "" : s1.Reference,
                                 Username = s2 == null ? "" : s2.UserName
                             };

            var totalCount = await pagedAndFilteredAssetNotes.CountAsync();

            return new PagedResultDto<GetAssetNotesForViewDto>(
                totalCount,
                await assetNotes.ToListAsync()
            );
        }

        public async Task<GetAssetNotesForViewDto> GetAssetNotesForView(int id)
        {
            var assetNotes = await _assetNotesRepository.GetAsync(id);

            var output = new GetAssetNotesForViewDto { AssetNotes = ObjectMapper.Map<AssetNotesDto>(assetNotes) };

            if (output.AssetNotes != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetNotes.AssetId);
                output.AssetReference = _lookupAsset.Reference;

                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((int)output.AssetNotes.UserId);
                output.Username = _lookupUser.UserName;
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_EditNotes)]
        public async Task<GetAssetNotesForEditOutput> GetAssetNotesForEdit(EntityDto input)
        {
            var assetNotes = await _assetNotesRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetAssetNotesForEditOutput { AssetNotes = ObjectMapper.Map<CreateOrEditAssetNotesDto>(assetNotes) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditAssetNotesDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_CreateNotes)]
        protected virtual async Task Create(CreateOrEditAssetNotesDto input)
        {
            var assetNotes = ObjectMapper.Map<AssetNote>(input);

            if (AbpSession != null)
            {
                assetNotes.TenantId = (int?)AbpSession.TenantId;
                assetNotes.UserId = AbpSession?.UserId;
            }

            await _assetNotesRepository.InsertAsync(assetNotes);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_EditNotes)]
        protected virtual async Task Update(CreateOrEditAssetNotesDto input)
        {
            var assetNotes = await _assetNotesRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, assetNotes);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Assets_DeleteNotes)]
        public async Task Delete(EntityDto input)
        {
            await _assetNotesRepository.DeleteAsync(input.Id);
        }
    }
}

