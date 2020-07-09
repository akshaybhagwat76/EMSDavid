using System.Collections.Generic;
using Ems.Chat.Dto;
using Ems.Dto;

namespace Ems.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(List<ChatMessageExportDto> messages);
    }
}
