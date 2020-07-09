using System.Collections.Generic;
using Ems.Storage.Dtos;
using Ems.Dto;

namespace Ems.Storage.Exporting
{
    public interface IAttachmentsExcelExporter
    {
        FileDto ExportToFile(List<GetAttachmentForViewDto> attachments);
    }
}