using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Assets.Dtos
{
    public class GetInventoryItemForEditOutput
    {
		public CreateOrEditInventoryItemDto InventoryItem { get; set; }

		public string ItemTypeType { get; set;}

		public string AssetReference { get; set;}


    }
}