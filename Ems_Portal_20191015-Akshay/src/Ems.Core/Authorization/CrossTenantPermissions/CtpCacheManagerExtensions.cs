using Abp.Runtime.Caching;
using System;

namespace Ems.Authorization
{
    public static class CtpCacheManagerExtensions
    {
        public static ITypedCache<string, CtpCacheItem> GetCrossTenantPermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, CtpCacheItem>(CtpCacheItem.CacheName);
        }
    }

    [Serializable]
    public class CtpCacheItem
    {
        public const string CacheName = "CrossTenantPermissionCache";

        public CrossTenantPermission CrossTenantPermission { get; set; }

        public DateTime LastRefresh { get; set; }
    }
}
