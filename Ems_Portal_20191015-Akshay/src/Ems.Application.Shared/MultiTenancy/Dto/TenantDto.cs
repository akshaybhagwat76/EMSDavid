using Abp.Application.Services.Dto;
using Ems.MultiTenancy.Payments;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ems.MultiTenancy.Dto
{
    public class TenantDto : EntityDto
    {
        public const int MaxLogoMimeTypeLength = 64;

        public string TenantType { get; set; } 

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        public virtual Guid? CustomCssId { get; set; }

        public virtual Guid? LogoId { get; set; }

        [MaxLength(MaxLogoMimeTypeLength)]
        public virtual string LogoFileType { get; set; }

        public SubscriptionPaymentType SubscriptionPaymentType { get; set; }

        public bool HasLogo { get; set; }

        private bool IsSubscriptionEnded { get; set; }

        public bool HasUnlimitedTimeSubscription { get; set; }
    }
}
