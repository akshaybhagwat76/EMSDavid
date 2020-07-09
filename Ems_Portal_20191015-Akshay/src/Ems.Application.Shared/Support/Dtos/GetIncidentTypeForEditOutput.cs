using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class GetIncidentTypeForEditOutput
    {
		public CreateOrEditIncidentTypeDto IncidentType { get; set; }


    }
}