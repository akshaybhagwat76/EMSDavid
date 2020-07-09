using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Ems.Authorization.Roles;
using Ems.Authorization.Users;
using Ems.Editions;
using Ems.MultiTenancy.Demo;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Ems.Notifications;
using System;
using System.Diagnostics;
using Abp.Runtime.Session;
using Abp.UI;
using Ems.MultiTenancy.Payments;
using Ems.Customers;
using Ems.Vendors;
using Ems.Assets;
using Ems.Authorization;
using System.Collections.Generic;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Ems.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public IAbpSession AbpSession { get; set; }


        private readonly IRolePermissionStore<Role> _rolePermissionStore; // Added by TC
        private readonly IRepository<AssetOwner> _assetOwnerRepository; // Added by TC
        private readonly IRepository<Vendor> _vendorRepository; // Added by TC
        private readonly IRepository<Customer> _customerRepository; // Added by TC
        private readonly IRepository<CustomerType> _customerTypeRepository; // Added by TC
        private readonly IRepository<Tenant> _tenantRepository; // Added by TC
        private readonly IRepository<CrossTenantPermission> _crossTenantPermissionRepository; // Added by TC
        private readonly IPermissionManager _permissionManager; 

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IUserEmailer _userEmailer;
        private readonly TenantDemoDataBuilder _demoDataBuilder;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;

        private readonly ICacheManager _cacheManager; // Added by TC

        public TenantManager(
            IRolePermissionStore<Role> rolePermissionStore, // Added by TC
            //IRepository<Permission> permissionRepository, // Added by TC
            IRepository<Customer> customerRepository, // Added by TC
            IRepository<CustomerType> customerTypeRepository, // Added by TC
            IRepository<Vendor> vendorRepository, // Added by TC
            IRepository<AssetOwner> assetOwnerRepository, // Added by TC
            IRepository<CrossTenantPermission> crossTenantPermissionRepository, // Added by TC

            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            TenantDemoDataBuilder demoDataBuilder,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IAbpZeroFeatureValueStore featureValueStore,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher,
            IRepository<SubscribableEdition> subscribableEditionRepository,
            ICacheManager cacheManager, // Added by TC
            IPermissionManager permissionManager

            ) : base(
                        tenantRepository,
                        tenantFeatureRepository,
                        editionManager,
                        featureValueStore
                    )
        {
            AbpSession = NullAbpSession.Instance;
            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _demoDataBuilder = demoDataBuilder;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _subscribableEditionRepository = subscribableEditionRepository;

            _rolePermissionStore = rolePermissionStore; // Added by TC
            _vendorRepository = vendorRepository; // Added by TC
            _assetOwnerRepository = assetOwnerRepository;// Added by TC
            _customerRepository = customerRepository;// Added by TC
            _customerTypeRepository = customerTypeRepository;// Added by TC
            _tenantRepository = tenantRepository;// Added by TC
            _crossTenantPermissionRepository = crossTenantPermissionRepository; // Added by TC
            _cacheManager = cacheManager;// Added by TC
            _permissionManager = permissionManager;

        }
        public async Task<int> CreateWithAdminUserAsync(
            string tenantType,
            string tenancyName,
            string name,
            string hostAdminEmailAddress,
            string hostAdminPassword,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            string emailActivationLink)
        {
            int newTenantId;
            long newAdminId;

            await CheckEditionAsync(editionId, isInTrialPeriod);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    TenantType = tenantType,
                    IsActive = isActive,
                    EditionId = editionId,
                    SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                    IsInTrialPeriod = isInTrialPeriod,
                    ConnectionString = connectionString.IsNullOrWhiteSpace() ? null : SimpleStringCipher.Instance.Encrypt(connectionString)
                };

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //Create tenant database
                _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

                //We are working with entities of the new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    List<string> adminPermissions = new List<string>();
                    List<string> userPermissions = new List<string>();

                    string staticAdminRoleName = string.Empty;
                    string staticUserRoleName = string.Empty;

                    switch (tenantType)
                    {
                        case "A":
                            staticAdminRoleName = StaticRoleNames.Tenants.AssetOwnerAdmin;
                            staticUserRoleName = StaticRoleNames.Tenants.AssetOwnerUser;
                            adminPermissions = TenantPermissionConsts.AssetOwnerAdminPermissions.Split(',').ToList();
                            userPermissions = TenantPermissionConsts.AssetOwnerUserPermissions.Split(',').ToList();
                            break;

                        case "C":
                            staticAdminRoleName = StaticRoleNames.Tenants.CustomerAdmin;
                            staticUserRoleName = StaticRoleNames.Tenants.CustomerUser;
                            adminPermissions = TenantPermissionConsts.CustomerAdminPermissions.Split(',').ToList();
                            userPermissions = TenantPermissionConsts.CustomerUserPermissions.Split(',').ToList();
                            break;

                        case "V":
                            staticAdminRoleName = StaticRoleNames.Tenants.VendorAdmin;
                            staticUserRoleName = StaticRoleNames.Tenants.VendorUser;
                            adminPermissions = TenantPermissionConsts.VendorAdminPermissions.Split(',').ToList();
                            userPermissions = TenantPermissionConsts.VendorUserPermissions.Split(',').ToList();
                            break;

                        default:
                            throw new Exception($"Cannot determine TenantType for {tenant.TenancyName}!");
                    }

                    //Create static roles for new tenant

                    var hostAdminRole = new Role() { TenantId = tenant.Id, IsDefault = false, IsStatic = true, Name = StaticRoleNames.Tenants.Admin, DisplayName = StaticRoleNames.Tenants.Admin, NormalizedName = StaticRoleNames.Tenants.Admin.ToUpper() };
                    CheckErrors(await _roleManager.CreateAsync(hostAdminRole));

                    var adminRole = new Role() { TenantId = tenant.Id, IsDefault = false, IsStatic = true, Name = staticAdminRoleName, DisplayName = staticAdminRoleName, NormalizedName = staticAdminRoleName.ToUpper() };
                    CheckErrors(await _roleManager.CreateAsync(adminRole));

                    var userRole = new Role() { TenantId = tenant.Id, IsDefault = true, IsStatic = true, Name = staticUserRoleName, DisplayName = staticUserRoleName, NormalizedName = staticUserRoleName.ToUpper() };
                    CheckErrors(await _roleManager.CreateAsync(userRole));

                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    // Grant permissions to the Admin role
                    foreach (var permission in adminPermissions)
                    {
                        if (permission != "")
                        {
                            await _rolePermissionStore.AddPermissionAsync(adminRole, new PermissionGrantInfo(permission, true));
                        }
                    }

                    // Grant permissions to the User role
                    foreach (var permission in userPermissions)
                    {
                        if (permission != "")
                        {
                            await _rolePermissionStore.AddPermissionAsync(userRole, new PermissionGrantInfo(permission, true));
                        }
                    }

                    //Create the host admin user for the tenant

                    var hostAdminUser = User.CreateTenantHostAdminUser(tenant.Id, hostAdminEmailAddress);
                    hostAdminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    hostAdminUser.IsActive = true;

                    if (hostAdminPassword.IsNullOrEmpty())
                    {
                        hostAdminPassword = await _userManager.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, hostAdminUser, hostAdminPassword));
                        }
                    }

                    hostAdminUser.Password = _passwordHasher.HashPassword(hostAdminUser, hostAdminPassword);

                    CheckErrors(await _userManager.CreateAsync(hostAdminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get hostAdmin user's id

                    //Assign hostAdmin user to hostAdmin role!
                    CheckErrors(await _userManager.AddToRoleAsync(hostAdminUser, hostAdminRole.Name));


                    //Create admin user for the tenant

                    var firstName = adminEmailAddress.Split('@').ToList().FirstOrDefault();
                    
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    var newUserName = string.Format("{0}Admin", rgx.Replace(tenancyName, "").ToLower()).Replace(" ","");

                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress, newUserName, firstName, "Admin");
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = await _userManager.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }

                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    //Notifications
                    await _appNotifier.WelcomeToTheApplicationAsync(adminUser);
                    
                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    await _demoDataBuilder.BuildForAsync(tenant);

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;
                }

                // Create the AssetOwner, Vendor or Customer

                switch (tenantType)
                {
                    case "A":
                        AssetOwner assetOwner = new AssetOwner() { Reference = tenant.TenancyName, Identifier = tenant.TenancyName, Name = tenant.Name, TenantId = tenant.Id };
                        _assetOwnerRepository.Insert(assetOwner);
                        break;

                    case "C":

                        var defaultCustomerTypeId = _customerTypeRepository.GetAll().FirstOrDefault().Id;

                        Customer customer = new Customer() { CustomerTypeId = defaultCustomerTypeId, Reference = tenant.TenancyName, Identifier = tenant.TenancyName, Name = tenant.Name, TenantId = tenant.Id };
                        _customerRepository.Insert(customer);
                        break;

                    case "V":
                        Vendor vendor = new Vendor() { Reference = tenant.TenancyName, Identifier = tenant.TenancyName, Name = tenant.Name, TenantId = tenant.Id };
                        _vendorRepository.Insert(vendor);
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenant.TenancyName}!");
                }

                await uow.CompleteAsync();
            }

            // Used a second UOW since the UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }

        public async Task CheckEditionAsync(int? editionId, bool isInTrialPeriod)
        {
            if (!editionId.HasValue || !isInTrialPeriod)
            {
                return;
            }

            var edition = await _subscribableEditionRepository.GetAsync(editionId.Value);
            if (!edition.IsFree)
            {
                return;
            }

            var error = LocalizationManager.GetSource(EmsConsts.LocalizationSourceName).GetString("FreeEditionsCannotHaveTrialVersions");
            throw new UserFriendlyException(error);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public decimal GetUpgradePrice(SubscribableEdition currentEdition, SubscribableEdition targetEdition, int totalRemainingDayCount, PaymentPeriodType paymentPeriodType)
        {
            var unusedPeriodCount = totalRemainingDayCount / (int)paymentPeriodType;
            var unusedDayCount = totalRemainingDayCount % (int)paymentPeriodType;

            decimal currentEditionPriceForUnusedPeriod = 0;
            decimal targetEditionPriceForUnusedPeriod = 0;

            var currentEditionPrice = currentEdition.GetPaymentAmount(paymentPeriodType);
            var targetEditionPrice = targetEdition.GetPaymentAmount(paymentPeriodType);

            if (currentEditionPrice > 0)
            {
                currentEditionPriceForUnusedPeriod = currentEditionPrice * unusedPeriodCount;
                currentEditionPriceForUnusedPeriod += (currentEditionPrice / (int)paymentPeriodType) * unusedDayCount;
            }

            if (targetEditionPrice > 0)
            {
                targetEditionPriceForUnusedPeriod = targetEditionPrice * unusedPeriodCount;
                targetEditionPriceForUnusedPeriod += (targetEditionPrice / (int)paymentPeriodType) * unusedDayCount;
            }

            return targetEditionPriceForUnusedPeriod - currentEditionPriceForUnusedPeriod;
        }

        public async Task<Tenant> UpdateTenantAsync(int tenantId, bool isActive, bool? isInTrialPeriod, PaymentPeriodType? paymentPeriodType, int editionId, EditionPaymentType editionPaymentType)
        {
            var tenant = await FindByIdAsync(tenantId);

            tenant.IsActive = isActive;

            if (isInTrialPeriod.HasValue)
            {
                tenant.IsInTrialPeriod = isInTrialPeriod.Value;
            }

            tenant.EditionId = editionId;

            if (paymentPeriodType.HasValue)
            {
                tenant.UpdateSubscriptionDateForPayment(paymentPeriodType.Value, editionPaymentType);
            }

            return tenant;
        }

        public async Task<EndSubscriptionResult> EndSubscriptionAsync(Tenant tenant, SubscribableEdition edition, DateTime nowUtc)
        {
            if (tenant.EditionId == null || tenant.HasUnlimitedTimeSubscription())
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} tenant has unlimited time subscription!");
            }

            Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

            var subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value;
            if (!tenant.IsInTrialPeriod)
            {
                subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0);
            }

            if (subscriptionEndDateUtc >= nowUtc)
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} since subscription has not expired yet!");
            }

            if (!tenant.IsInTrialPeriod && edition.ExpiringEditionId.HasValue)
            {
                tenant.EditionId = edition.ExpiringEditionId.Value;
                tenant.SubscriptionEndDateUtc = null;

                await UpdateAsync(tenant);

                return EndSubscriptionResult.AssignedToAnotherEdition;
            }

            tenant.IsActive = false;
            tenant.IsInTrialPeriod = false;

            await UpdateAsync(tenant);

            return EndSubscriptionResult.TenantSetInActive;
        }

        public async Task<TenantInfo> GetTenantInfo()
        {
            //TODO: Cache this in the user Session - check before looking up again

            if(AbpSession.TenantId == null)
            {
                return new TenantInfo() { IsHost = true, Tenant = new Tenant() {TenantType="H", TenancyName = "Host" } };
            }

            Tenant tenant =  await _tenantRepository.GetAsync((int)AbpSession.TenantId);
            TenantInfo tenantInfo = new TenantInfo()
            {
                Tenant = tenant
            };
            
            switch (tenant.TenantType)
            {
                case "A":
                    tenantInfo.AssetOwner = _assetOwnerRepository.GetAll().Where(a => a.TenantId == tenant.Id).FirstOrDefault();
                    break;
                case "C":
                    tenantInfo.Customer = _customerRepository.GetAll().Where(a => a.TenantId == tenant.Id).FirstOrDefault();
                    break;
                case "V":
                    tenantInfo.Vendor = _vendorRepository.GetAll().Where(a => a.TenantId == tenant.Id).FirstOrDefault();
                    break;
                default:
                    throw new Exception($"Cannot determine TenantType for {tenant.TenancyName}!");
            }

            return tenantInfo;
        }

        public async Task<List<int>> GetCrossTenantPermissions(TenantInfo tenantInfo, string entityType)
        {
            if (tenantInfo.IsHost)
            {
                var tenants = await _tenantRepository.GetAllListAsync();
                var tenantIds = tenants.Select(t => t.Id).ToList();

                return tenantIds;
            }
            else if (tenantInfo.Tenant.TenantType == "A" | tenantInfo.Tenant.TenantType == "V" | tenantInfo.Tenant.TenantType == "C")
            {

                // Check Cache
                var cacheKey = string.Format("{0}|{1}", AbpSession.ToUserIdentifier().ToString(), entityType);

                bool addToCache = false;
                CtpCacheItem ctpCacheItem = await _cacheManager.GetCrossTenantPermissionCache().GetOrDefaultAsync(cacheKey);
                CrossTenantPermission crossTenantPermission = new CrossTenantPermission();
                List<CrossTenantPermission> allCrossTenantPermissions = new List<CrossTenantPermission>();
                List<CrossTenantPermission> tenantsCrossTenantPermissions = new List<CrossTenantPermission>();

                if (ctpCacheItem != null)
                {
                    crossTenantPermission = ctpCacheItem.CrossTenantPermission;
                }
                else
                {
                    allCrossTenantPermissions = await _crossTenantPermissionRepository.GetAllListAsync();
                    tenantsCrossTenantPermissions = allCrossTenantPermissions.Where(t => t.TenantRefId == tenantInfo.Tenant.Id).ToList();
                    crossTenantPermission = allCrossTenantPermissions.Where(t => t.TenantRefId == tenantInfo.Tenant.Id && t.EntityType == entityType).FirstOrDefault();
                    addToCache = true;
                }

                var authorizedForTenants = crossTenantPermission?.Tenants;

                if (authorizedForTenants == null) // No CrossTenenantPermissions found, so just send back the current Tenant's ID
                {
                    return new List<int>() { tenantInfo.Tenant.Id };
                }

                var tenantIds = authorizedForTenants?.Split(',').Select(int.Parse).ToList();
                tenantIds?.Add(tenantInfo.Tenant.Id); // To select the current tenant data (Including CrossTenenantPermissions)

                // update the cache

                if (addToCache)
                {
                    foreach(var ctp in tenantsCrossTenantPermissions)
                    {
                        var cacheItem = new CtpCacheItem { CrossTenantPermission = ctp, LastRefresh = DateTime.Now};

                        _cacheManager.GetCrossTenantPermissionCache().Set(
                            cacheKey,
                            cacheItem
                        );
                    }
                }

                return tenantIds;
            }
            else
            {
                throw new Exception($"Illegal TenantType for {tenantInfo.Tenant.TenancyName}!");
            }
        }

    }

    public class TenantInfo
    {
        public Tenant Tenant { get; set; }
        public Customer Customer { get; set; }
        public Vendor Vendor { get; set; }
        public AssetOwner AssetOwner { get; set; }
        public bool IsHost { get; set; }
    }
}
