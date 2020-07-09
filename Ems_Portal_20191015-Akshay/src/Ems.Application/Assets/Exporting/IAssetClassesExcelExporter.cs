using System.Collections.Generic;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets.Exporting
{
    public interface IAssetClassesExcelExporter
    {
        FileDto ExportToFile(List<GetAssetClassForViewDto> assetClasses);
    }
}