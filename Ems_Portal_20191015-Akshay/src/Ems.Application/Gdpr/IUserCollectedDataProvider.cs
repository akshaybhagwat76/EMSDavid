using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Ems.Dto;

namespace Ems.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
