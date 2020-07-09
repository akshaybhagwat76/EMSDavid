using System.Linq;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ems.Authorization;
using Ems.Authorization.Roles;
using Ems.Authorization.Users;
using Ems.EntityFrameworkCore;
using Ems.Notifications;
using System;
using System.Collections.Generic;
using Ems.MultiTenancy;
using System.Text.RegularExpressions;
using Abp.Domain.Uow;

namespace Ems.Migrations.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly EmsDbContext _context;
        private readonly int _tenantId;
        private readonly string _tenantType;

        public TenantRoleAndUserBuilder(EmsDbContext context, int tenantId, string tenantType)
        {
            _context = context;
            _tenantId = tenantId;
            _tenantType = tenantType;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }
        
        [UnitOfWork]
        private void CreateRolesAndUsers()
        {
            var companyName = "default";
            if (_tenantType != null && _tenantId > 0)
            {
                Tenant tenant = _context.Tenants.Where(t => t.Id == _tenantId).FirstOrDefault();
                companyName = (tenant.Name != null) ? tenant.Name : companyName;
            }

            //Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            //User role
            /*
            var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
                _context.SaveChanges();
            }
            */

            //CustomerAdmin role

            if (_tenantType == "C")
            {
                var customerAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.CustomerAdmin);
                if (customerAdminRole == null)
                {
                    customerAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.CustomerAdmin, StaticRoleNames.Tenants.CustomerAdmin) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var customer = _context.Customers.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (customer != null)
                    {
                        CreateTestUser(customerAdminRole.Id, customer.Name, true);
                    };

                    var permissions = TenantPermissionConsts.CustomerAdminPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = customerAdminRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //CustomerUser role

            if (_tenantType == "C")
            {
                var customerUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.CustomerUser);
                if (customerUserRole == null)
                {
                    customerUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.CustomerUser, StaticRoleNames.Tenants.CustomerUser) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var customer = _context.Customers.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (customer != null)
                    {
                        CreateTestUser(customerUserRole.Id, customer.Name, false);
                    };

                    var permissions = TenantPermissionConsts.CustomerUserPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = customerUserRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }


            //VendorAdmin role

            if (_tenantType == "V")
            {
                var vendorAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.VendorAdmin);
                if (vendorAdminRole == null)
                {
                    vendorAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.VendorAdmin, StaticRoleNames.Tenants.VendorAdmin) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var vendor = _context.Vendors.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (vendor != null)
                    {
                        CreateTestUser(vendorAdminRole.Id, vendor.Name, true);
                    };

                    var permissions = TenantPermissionConsts.VendorAdminPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = vendorAdminRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //VendorUser role

            if (_tenantType == "V")
            {
                var vendorUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.VendorUser);
                if (vendorUserRole == null)
                {
                    vendorUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.VendorUser, StaticRoleNames.Tenants.VendorUser) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var vendor = _context.Vendors.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (vendor != null)
                    {
                        CreateTestUser(vendorUserRole.Id, vendor.Name, false);
                    };

                    var permissions = TenantPermissionConsts.VendorUserPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = vendorUserRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //AssetOwnerAdmin role

            if (_tenantType == "A")
            {
                var assetOwnerAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.AssetOwnerAdmin);
                if (assetOwnerAdminRole == null)
                {
                    assetOwnerAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.AssetOwnerAdmin, StaticRoleNames.Tenants.AssetOwnerAdmin) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var assetOwner = _context.AssetOwners.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (assetOwner != null)
                    {
                        CreateTestUser(assetOwnerAdminRole.Id, assetOwner.Name, true);
                    };

                    var permissions = TenantPermissionConsts.AssetOwnerAdminPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = assetOwnerAdminRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //AssetOwnerUser role

            if (_tenantType == "A")
            {
                var assetOwnerUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.AssetOwnerUser);
                if (assetOwnerUserRole == null)
                {
                    assetOwnerUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.AssetOwnerUser, StaticRoleNames.Tenants.AssetOwnerUser) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var assetOwner = _context.AssetOwners.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (assetOwner != null)
                    {
                        CreateTestUser(assetOwnerUserRole.Id, assetOwner.Name, false);
                    };

                    var permissions = TenantPermissionConsts.AssetOwnerUserPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = assetOwnerUserRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }


            //admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantHostAdminUser(_tenantId, "tonyclark69@live.com.au");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                adminUser.IsEmailConfirmed = true;
                adminUser.ShouldChangePasswordOnNextLogin = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                //User "account" of admin user

                if (_tenantId == 1) // WHY oh WHY is this just for tenantId 1 ????????????????????????????????????????????????????
                {
                    _context.UserAccounts.Add(new UserAccount
                    {
                        TenantId = _tenantId,
                        UserId = adminUser.Id,
                        UserName = AbpUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    _context.SaveChanges();
                }

                //Notification subscription
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, adminUser.Id, AppNotificationNames.NewUserRegistered));
                _context.SaveChanges();
            }
        }


        [UnitOfWork]
        public void CreateSeedRolesAndUsers()
        {
            var companyName = "default";
            if(_tenantType != null)
            {
                Tenant tenant = _context.Tenants.Where(t => t.Id == _tenantId).FirstOrDefault();
                companyName = tenant.Name;
            }

            //Admin role

            
            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }


            /*
            //User role

            var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
                _context.SaveChanges();
            }
            */

            //CustomerAdmin role

            if (_tenantType == "C")
            {
                var customerAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.CustomerAdmin);
                if (customerAdminRole == null)
                {
                    customerAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.CustomerAdmin, StaticRoleNames.Tenants.CustomerAdmin) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var customer = _context.Customers.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (customer != null)
                    {
                        CreateTestUser(customerAdminRole.Id, customer.Name, true);
                    };

                    var permissions = TenantPermissionConsts.CustomerAdminPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = customerAdminRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //CustomerUser role

            if (_tenantType == "C")
            {
                var customerUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.CustomerUser);
                if (customerUserRole == null)
                {
                    customerUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.CustomerUser, StaticRoleNames.Tenants.CustomerUser) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var customer = _context.Customers.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (customer != null)
                    {
                        CreateTestUser(customerUserRole.Id, customer.Name, false);
                    };

                    var permissions = TenantPermissionConsts.CustomerUserPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = customerUserRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }


            //VendorAdmin role

            if (_tenantType == "V")
            {
                var vendorAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.VendorAdmin);
                if (vendorAdminRole == null)
                {
                    vendorAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.VendorAdmin, StaticRoleNames.Tenants.VendorAdmin) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var vendor = _context.Vendors.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (vendor != null)
                    {
                        CreateTestUser(vendorAdminRole.Id, vendor.Name, true);
                    };

                    var permissions = TenantPermissionConsts.VendorAdminPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = vendorAdminRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //VendorUser role

            if (_tenantType == "V")
            {
                var vendorUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.VendorUser);
                if (vendorUserRole == null)
                {
                    vendorUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.VendorUser, StaticRoleNames.Tenants.VendorUser) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var vendor = _context.Vendors.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (vendor != null)
                    {
                        CreateTestUser(vendorUserRole.Id, vendor.Name, false);
                    };

                    var permissions = TenantPermissionConsts.VendorUserPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = vendorUserRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //AssetOwnerAdmin role

            if (_tenantType == "A")
            {
                var assetOwnerAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.AssetOwnerAdmin);
                if (assetOwnerAdminRole == null)
                {
                    assetOwnerAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.AssetOwnerAdmin, StaticRoleNames.Tenants.AssetOwnerAdmin) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var assetOwner = _context.AssetOwners.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (assetOwner != null)
                    {
                        CreateTestUser(assetOwnerAdminRole.Id, assetOwner.Name, true);
                    };

                    var permissions = TenantPermissionConsts.AssetOwnerAdminPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = assetOwnerAdminRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //AssetOwnerUser role

            if (_tenantType == "A")
            {
                var assetOwnerUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.AssetOwnerUser);
                if (assetOwnerUserRole == null)
                {
                    assetOwnerUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.AssetOwnerUser, StaticRoleNames.Tenants.AssetOwnerUser) { IsStatic = true }).Entity;
                    _context.SaveChanges();

                    var assetOwner = _context.AssetOwners.Where(c => c.TenantId == _tenantId).FirstOrDefault();

                    if (assetOwner != null)
                    {
                        CreateTestUser(assetOwnerUserRole.Id, assetOwner.Name, false);
                    };
                    
                    var permissions = TenantPermissionConsts.AssetOwnerUserPermissions.Split(',').ToList();

                    foreach (var permission in permissions)
                    {
                        if (permission != "")
                        {
                            _context.RolePermissions.Add(new RolePermissionSetting() { CreationTime = DateTime.Now, CreatorUserId = null, Id = 0, IsGranted = true, Name = permission, RoleId = assetOwnerUserRole.Id, TenantId = _tenantId });
                        }
                    }

                    _context.SaveChanges();

                }
            }

            //admin user
            
            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantHostAdminUser(_tenantId, string.Format("admin@{0}.com", companyName.Replace(" ","").ToLower()));
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
                adminUser.IsEmailConfirmed = true;
                adminUser.ShouldChangePasswordOnNextLogin = false;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                //Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();

                //User account of admin user
                if (_tenantId == 1)
                {
                    _context.UserAccounts.Add(new UserAccount
                    {
                        TenantId = _tenantId,
                        UserId = adminUser.Id,
                        UserName = AbpUserBase.AdminUserName,
                        EmailAddress = adminUser.EmailAddress
                    });
                    _context.SaveChanges();
                }

                //Notification subscription
                _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, adminUser.Id, AppNotificationNames.NewUserRegistered));
                _context.SaveChanges();
            }
        }

        [UnitOfWork]
        private void CreateTestUser(int roleId, string companyName, bool isAdmin)
        {
            Random rnd = new Random();

            var firstNames = "Deny;Balduin;Charo;Chrisse;Patience;Torie;Sharyl;Saunder;Haley;Shayne;Rustie;Christian;Cassandre;Genny;Germain;Balduin;Inge;Monty;Thedric;Westbrook;Bartholemy;Giusto;Waldemar;Darb;Cory;Herminia;Tyson;Roberto;Kellina;Westleigh;Pen;Federica;Kendricks;Viv;Madalyn;Willetta;Gav;Tim;Winni;Austine;Kenn;Ker;Betty;De;Corby;Crosby;Yasmin;Marji;Beryl;Ringo;Tildie;Laurene;Adriaens;Findlay;Brigitte;Wilhelmina;Magda;Khalil;Marcel;Arnold;Clair;Minni;Marty;Rosmunda;Elsey;Claudine;Alexio;Crystal;Aldrich;Alard;Barney;Merrill;Fidole;Damita;Millie;Zahara;Mona;Ximenez;Rainer;Dawna;Malia;Ellen;Brendon;Archer;Morgan;Letty;Perceval;Robb;Kathrine;Willem;Chrisse;Kissiah;Veronica;Chaddy;Kit;Devondra;Yorker;Torey;Aggi;Melessa;Olly;Andris;Imojean;Ellette;Clem;Dulciana;Roarke;Dacy;Harland;Maddie;Hoyt;Benny;Elana;Agneta;Talbot;Evelyn;Robert;Noel;Evangeline;Bram;Andonis;Nicoline;Jacinthe;Netta;Marlyn;Genovera;Philippine;Alastair;Bunni;Hadlee;Yolande;Deeanne;Nadya;Shel;Erena;Darrin;Theresina;Wenda;Frederique;Frans;Breena;Bird;Editha;Channa;Cristal;Abdul;Waring;Iorgos;Carrol;Karoly;Abbe;Damian;Heather;Armin;Luis;Glyn;Caressa;Sax;Hercule;Emlyn;Leoline;Gene;Sonja;Desiri;Max;Mick;Cyrille;Simona;Boyd;Nicol;Consolata;Theodor;Judah;Aileen;Cally;Cecile;Aeriela;Gray;Lidia;Germain;Karoly;Olvan;Ricky;Adara;Pearline;Tiena;Marylee;Tally;Petronilla;Matthiew;Darnell;Clerkclaude;Hersh;Amity;Marabel;Robinet;Karol;Pedro;Marion;David;Any;Adan;Melitta;Juan;Minne;Delia;Odelle;Adriano;Lynelle;Stu;Damon;Nady;Rice;Pablo;Gerladina;Claretta;Hirsch;Thorn;Isabella;Virgina;Dorise;Darbee;Nicoline;Torrie;Karyl;Bard;Paulie;Stephie;Tucker;Blake;Josias;Fairleigh;Paxon;Clarey;Leontine;Ripley;Charisse;Shaw;Arleen;Bryana;Dody;Brendan;Jasmine;Ronnie;Martica;Odelia;Steffen;Rip;Nariko;Jule;Rutger;Oswell;Joete;Bradly;Cobbie;Amandi;Juditha;Upton;Glori;Hazlett;Timmi;Godard;Kory;Tara;Vanni;Kathrine;Mark;Corbin;Merrill;Janeta;Griffy;Zacharie;Sephira;Cecile;Genevra;Ferdy;Pierrette;Cinnamon;Estrellita;Louella;Lizabeth;Dewain;Willdon;Ailbert;Rutledge;Lincoln;Ches;Yvor;Gram;Gaby;Chaunce;Loralyn;Ferris;Gran;Jamesy;Codi;Zacharia;Goldina;Tadio;Sada;Gorden;Cirillo;Rubetta;Orlando;Aime;Sally;Sergent;Fergus;Jaymie;Bat;Madelene;Felita;Rasla;Peria;Emilia;Leora;Lennard;Sergei;Farr;Kath;Eveleen;Edgardo;Conny;Nessi;Clim;Ned;Glennie;Gussie;Free;Der;Gisele;Teresita;Padraic;Elwood;Pru;Ettie;Angie;Anselm;Elana;Maxie;Stirling;Inna;Loise;Ralina;Bethany;Uri;Jo-anne;Gabrila;Rudie;Celestina;Tomi;Lon;Gauthier;Jaquelin;Charmine;Larine;Christine;Eddie;Frans;Fionna;Nan;Mariejeanne;Teriann;Correna;Courtney;Danica;Prescott;Latisha;Horace;Storm;Ira;Petronilla;Enoch;Mariska;Jim;Dyane;Kelly;Cecilio;Idell;Ronald;Erich;Jeannette;Diahann;Gil;Shell;Dora;Wilhelmine;Stepha;Bartlett;Mitchell;Elisha;Storm;Carl;Ira;Abbot;Caprice;Jenelle;Jeniffer;Ettie;Darelle;Guilbert;Laurella;Kirk;Vasily;Brad;Robinett;Aristotle;Jodi;Mehetabel;Ariadne;Cecilia;Gerry;Cozmo;Noel;Cary;Kip;Corrina;Dionis;Lurlene;Mace;Vinny;Aida;Frieda;Sherill;Inger;Rowen;Belicia;Tandie;Brooks;Ros;Elsey;Westbrooke;Jenica;Karolina;Kristyn;Pavel;Malorie;Aloise;Boycie;Marquita;Forbes;Merell;Sibilla;Reginald;Allan;Sayres;Ashla;Jorge;Gwenore;Dalia;Tish;Bondie;Gizela;Portie;Henderson;Jodie;Ferguson;Morton;Rudyard;Corny;Dane;Clemmie;Alejandra;Maridel;Ryann;Udall;Rasla;Cointon;Orella;Ludovico;Minda;Doyle;Ted;Jessika;Diane-marie;Barbette;Tania;Anabella;Mella;Welby;Darryl;Vladimir;Dave;Rudy;Stesha;Olive;Elbertina;Orren;Maurie;Cynde;Laurene;Fawn;Eachelle;Kippie;Colette;Davie;Etan;Berri;Sterne;Lyman".Split(';').ToList();
            var surnames = "Dignam;Overbury;Fullerlove;Diggins;Guitton;Broadbere;Brecon;Bertot;Dunbleton;O'Hanlon;Whiterod;Grimley;Davidsen;Durrand;Howick;McCartney;Kalisch;Lancastle;Furmston;Vockings;Vaux;Schistl;Novotne;Antoinet;Walsham;Norsister;Cogley;Conlaund;Oleszkiewicz;Sillwood;Marmon;Riddles;MacClancey;Douris;Elwill;Petranek;Le Breton De La Vieuville;Deverick;Bandey;Carle;Embra;Teodori;Lonergan;Blueman;Holborn;McWard;Eldered;Mulbry;Caistor;Lamboll;Fredi;Brehault;Ort;Niesing;Camamill;Raittie;Murrow;Burger;Melonby;Askham;Caldecutt;Lowrey;Maud;Capeling;Arrigo;Ashpital;Brauninger;Minmagh;Androsik;Draper;Jankin;Battey;Taree;Tevlin;Rayhill;Pashley;Adamkiewicz;Mauchlen;Janik;Quirke;Verdun;Chatters;Eddowes;McLane;Wallis;Cammocke;Pedrielli;Roller;Hebden;Sewards;Bruyns;Cupitt;Bernaciak;Bedson;Radki;Smeall;Hayers;Fryman;Bartaletti;Earles;Sloyan;Worpole;Jurkiewicz;Marxsen;MacAloren;O'Driscole;Margrem;Harrod;Galia;Kimbley;McCorkindale;Fynes;Caswall;Guillotin;Byer;Setterfield;Dougherty;Jon;Josefovic;Rame;Garaghan;Matzl;Pibsworth;Janicki;Acott;Pettigrew;Casillis;Gadaud;Ragg;Sorrie;Bolle;Clacson;Codrington;Goodhew;Livens;Wortley;Buxcy;Whifen;Scorthorne;Stainbridge;Andriss;McCreary;Le feaver;Breeton;Skipton;Landreth;Dorr;Connolly;Querrard;Idle;Meadley;Yurov;Dally;Gurden;Clemens;Vlasov;Tipper;Fitzer;Maden;Otto;Azemar;Glencros;Pietrowicz;Paxton;Shemmans;Hayworth;McGonagle;Wildbore;Toomer;Dollin;Mizen;Batterton;Edeson;Yarrow;Kristiansen;Burdoun;MacNucator;McIlvaney;Dyter;Calleja;Donnersberg;O'Bradane;Major;Whitland;Greenstead;Osbaldeston;Couvert;Ilden;Madders;Jaime;Fisbey;Gush;Simkin;Romanski;Heaselgrave;Slinger;Pellew;Bingall;Borland;Hartell;Poulsom;O'Rowane;Robatham;Morgan;Francom;Todarini;Zamora;Pickaver;Simonard;Dagnan;Loughan;Fernehough;Mordin;Saxby;Ygou;Eakeley;MacGregor;Barrick;Baccus;Gosford;Allner;Richardet;Artharg;Pressdee;Touret;Camin;Ayre;Phlipon;Cheston;Allon;Cornew;Keeler;Egiloff;Wheelan;Wharlton;Garaghan;Skurray;Johannesson;Marcoolyn;Burg;Kettleson;De Freitas;Jennaway;De Ambrosis;Fearn;Giorgietto;Benedick;Cabrer;Kettlestring;Normadell;Tunmore;Nanson;Lawranson;Lovell;Aindriu;Leggin;Beekmann;Anglin;Jerrolt;Hillen;Waddicor;Perton;Sebastian;Kearn;Allott;Lack;Rackham;Dilrew;Mellmer;Lucien;Tate;Newsham;Goodlad;Edon;Avarne;Charlin;Essery;Cases;Dumbare;Girogetti;Brierton;Ortiga;Corris;Iannelli;Mundie;Mettrick;Sammut;Gabala;Hawk;Arents;Gantlett;Budibent;Kensington;McLenaghan;Vassar;Mougel;Robinet;Andreoletti;Kubatsch;Sanbrooke;Tolumello;Madgwick;Goulthorp;Livezley;Rudolf;Cardwell;Gambles;Uwins;McKniely;Gianetti;Orhrt;Sheehan;Stuchbury;Behnke;Lush;O'Meara;McNee;While;Giovani;Sindell;Try;Chessor;Fraschetti;Sarch;Bevans;Margetson;McCudden;Walklate;Emmens;Siemens;Gordon;Kendall;Snarie;Canby;McPaik;Bravery;Kinver;Bradshaw;Pedler;Beadman;Honeywood;Zavattero;Couche;Linner;Sycamore;Cosbey;Forkan;Cottu;Mapson;Luckett;Froome;Stockill;Rominov;Caslett;Melrose;Dalton;Cardello;Lacasa;Longbone;Kiledal;Eilhart;Nickell;Bourley;Osgordby;Leachman;McGarvey;Nutbean;Goulbourn;Blunsden;Todeo;Openshaw;Besnard;Skae;Gilliat;Clohisey;Cosins;Kellick;De Bruin;Casaccio;Bhatia;Chat;Hannent;Jeeks;Hamp;Armell;Cleminson;Minot;Montel;Dofty;Gussie;Simeonov;McNiff;Walter;Yexley;Luff;Vose;Martignon;Harlin;Fermer;McLachlan;Fordham;Mollatt;Flips;Basden;Mattaus;Glencorse;Hendrich;Carr;Robbeke;Wemm;Pendlenton;Budgey;MacGowan;Sherlaw;Crisp;Kingsnod;Dever;Couchman;Oury;Grinley;Golden;De Francisci;Inskipp;Fearnall;Kesey;Braden;MacGuffog;Razzell;Henric;Rippin;Thackston;Matuszynski;Clew;Pirie;Bogie;Bunny;Gallego;Kidston;Shulem;Petrulis;Fargher;Tolumello;Finkle;Trenear;O'Dowgaine;Gagin;Shefton;Jorgesen;Grealy;Bisterfeld;Tynan;Gland;Cracoe;Swinyard;Feronet;Kalinke;Doolan;Schnieder;Denisovich;Pirot;Maier;De Bruyn;Nutkin;Kochl;Lowrance;Jellybrand;Comellini;Vaszoly;Lackie;Duhig;Mettricke;Brimmell;Alcorn;Goford;Seater;Uman;Porcas;Duthie;McKeefry;Malcolm;Trimmill;Lyffe;Dillistone;Chrippes;Bettam;Bechley;Basill;Crossgrove;Wessel;Spleving;Stubley;Robilliard;Cerro;Plowes;Hutcheon;Richel;Dobbyn;Danbi;Hune;Timmis".Split(';').ToList();

            var firstName = firstNames[rnd.Next(0,firstNames.Count-1)];
            var surname = surnames[rnd.Next(0, surnames.Count - 1)];

            Regex rgx = new Regex("[^a-zA-Z0-9]");

            var adminString = (isAdmin) ? ".admin" : "";

            var email = string.Format("{0}.{1}{3}@{2}.com", rgx.Replace(firstName, "") , rgx.Replace(surname, ""), companyName.Split(' ').ToList()[0], adminString).Replace(" ", "").Replace(",","").ToLower();
            var userName = string.Format("{0}.{1}", firstName, surname).Replace(" ", "").ToLower();

            var newUser = new User
            {
                TenantId = _tenantId,
                UserName = userName,
                Name = firstName,
                Surname = surname,
                EmailAddress = email,
                Roles = new List<UserRole>(),
                OrganizationUnits = new List<UserOrganizationUnit>(),
                NormalizedEmailAddress = email,
                NormalizedUserName = userName,
                IsEmailConfirmed = true,
                AccessFailedCount = 0,
                IsActive = true,
                IsLockoutEnabled = false,
                IsPhoneNumberConfirmed = true,
                PhoneNumber = string.Format("{0}-{1}-{2}", rnd.Next(55,88).ToString(), rnd.Next(3333, 9999).ToString(), rnd.Next(3333, 9999).ToString()),
                IsTwoFactorEnabled = false,
                ShouldChangePasswordOnNextLogin = false
            };

            newUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(newUser, "123qwe");

            _context.Users.Add(newUser);
            _context.SaveChanges();

            //Assign new user to the correct role
            _context.UserRoles.Add(new UserRole(_tenantId, newUser.Id, roleId));
            _context.SaveChanges();

            //Create a user account for the new user

            _context.UserAccounts.Add(new UserAccount
            {
                TenantId = _tenantId,
                UserId = newUser.Id,
                UserName = userName,
                EmailAddress = newUser.EmailAddress
            });
            _context.SaveChanges();

            //Notification subscription
            _context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, newUser.Id, AppNotificationNames.NewUserRegistered));
            _context.SaveChanges();

        }
    }
}
