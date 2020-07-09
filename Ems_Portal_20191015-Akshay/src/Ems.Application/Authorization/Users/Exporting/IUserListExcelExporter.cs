using System.Collections.Generic;
using Ems.Authorization.Users.Dto;
using Ems.Dto;

namespace Ems.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}