using System.Collections.Generic;
using Ems.Authorization.Users.Importing.Dto;
using Ems.Dto;

namespace Ems.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
