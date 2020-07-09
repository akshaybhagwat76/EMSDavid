using System.Collections.Generic;
using Ems.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace Ems.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
