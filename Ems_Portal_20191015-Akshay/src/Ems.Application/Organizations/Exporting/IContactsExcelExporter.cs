using System.Collections.Generic;
using Ems.Organizations.Dtos;
using Ems.Dto;

namespace Ems.Organizations.Exporting
{
    public interface IContactsExcelExporter
    {
        FileDto ExportToFile(List<GetContactForViewDto> contacts);
    }
}