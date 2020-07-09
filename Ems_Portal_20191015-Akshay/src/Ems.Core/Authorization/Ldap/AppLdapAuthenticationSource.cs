using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Ems.Authorization.Users;
using Ems.MultiTenancy;

namespace Ems.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}