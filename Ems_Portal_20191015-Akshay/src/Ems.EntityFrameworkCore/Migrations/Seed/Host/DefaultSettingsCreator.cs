using System.Linq;
using Abp.Configuration;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Ems.EntityFrameworkCore;

namespace Ems.Migrations.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly EmsDbContext _context;

        public DefaultSettingsCreator(EmsDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            int? tenantId = null;

            if (EmsConsts.MultiTenancyEnabled == false)
            {
                tenantId = MultiTenancyConsts.DefaultTenantId;
            }

            //Generic Settings

            AddSettingIfNotExists("App.TenantManagement.AllowSelfRegistration", "false", null);

            //Emailing

            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "mailer@firstconnexions.com", tenantId);
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "firstconnexions.com mailer", tenantId);

            //Languages

            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en", tenantId);

            //Theme

            AddSettingIfNotExists("App.UiManagement.Theme", "theme4", null);
            AddSettingIfNotExists("theme4.App.UiManagement.Header.DesktopFixedHeader", "True", null);
            AddSettingIfNotExists("theme4.App.UiManagement.Header.MobileFixedHeader", "False", null);
            AddSettingIfNotExists("theme4.App.UiManagement.Header.MenuArrows", "True", null);

            //Emai;

            AddSettingIfNotExists("Abp.Net.Mail.Smtp.Host", "smtp.zoho.com", null);
            AddSettingIfNotExists("Abp.Net.Mail.Smtp.Port", "465", null);
            AddSettingIfNotExists("Abp.Net.Mail.Smtp.UserName", "mailer@firstconnexions.com", null);
            AddSettingIfNotExists("Abp.Net.Mail.Smtp.Password", "bu24UDVW6FuwzgGsV06bEA==", null);
            AddSettingIfNotExists("Abp.Net.Mail.Smtp.Domain", "firstconnexions.com", null);
            AddSettingIfNotExists("Abp.Net.Mail.Smtp.EnableSsl", "true", null);
            AddSettingIfNotExists("Abp.Net.Mail.Smtp.UseDefaultCredentials", "false", null);
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}