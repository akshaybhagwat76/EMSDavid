using System.Linq;
using Ems.EntityFrameworkCore;
using Ems.Assets;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Ems.Customers;
using System;
using Ems.Organizations;
using Ems.MultiTenancy;
using Ems.Editions;
using Ems.Vendors;
using Ems.Support;
using Abp.Domain.Uow;

namespace Ems.Migrations.Seed.Host
{
    class SeedDataCreator
    {
        private readonly EmsDbContext _context;

        // CUSTOMERS

        private readonly List<string> customerReferences = "SIAEC_PH;".Split(';').ToList();
        private readonly List<string> customerNames = "SIA Engineering (Philippines) Corporation;".Split(';').ToList();
        
        // VENDORS

        private readonly List<string> vendorReferences = "POWERFORCE".Split(';').ToList();
        private readonly List<string> vendorNames = "Power Force Technologies Pte Ltd".Split(';').ToList();

        // ASSET OWNERS

        private readonly List<string> assetOwnerReferences = "AVEL".Split(';').ToList();
        private readonly List<string> assetOwnerNames = "AE Leasing Singapore".Split(';').ToList();

        // CONTACTS

        private readonly List<string> contactIsHeadOfficeContacts = "1".Split(';').ToList();
        private readonly List<string> contactContactNames = "Ma Aileen Puno - Miclat".Split(';').ToList();
        private readonly List<string> contactPhoneOffices = "-".Split(';').ToList();
        private readonly List<string> contactPhoneMobiles = "-".Split(';').ToList();
        private readonly List<string> contactFaxes = "-".Split(';').ToList();
        private readonly List<string> contactEmailAddresses = "-".Split(';').ToList();
        private readonly List<string> contactPositions = "-".Split(';').ToList();
        private readonly List<string> contactDepartments = "-".Split(';').ToList();
        private readonly List<string> contactUserIds = "null".Split(';').ToList();
        private readonly List<string> contactVendorIds = "null".Split(';').ToList();
        private readonly List<string> contactAssetOwnerIds = "null".Split(';').ToList();
        private readonly List<string> contactCustomerIds = "1".Split(';').ToList();

        // ADDRESSES

        private readonly List<string> addressTenantIds = "1".Split(';').ToList();
        private readonly List<string> addressAddressEntryNames = "Head Office".Split(';').ToList();
        private readonly List<string> addressIsHeadOffices = "1".Split(';').ToList();
        private readonly List<string> addressAddressLine1s = "Building 7592 A. Bonifacio Avenue".Split(';').ToList();
        private readonly List<string> addressAddressLine2s = "Civil Aviation Complex".Split(';').ToList();
        private readonly List<string> addressPostalCodes = "2023".Split(';').ToList();
        private readonly List<string> addressCities = "Clark Freeport Zone".Split(';').ToList();
        private readonly List<string> addressStates = "null".Split(';').ToList();
        private readonly List<string> addressCountries = "Philippines".Split(';').ToList();
        private readonly List<string> addressIsDefaultForBillings = "1".Split(';').ToList();
        private readonly List<string> addressIsDefaultForShippings = "1".Split(';').ToList();
        private readonly List<string> addressCustomerIds = "1".Split(';').ToList();
        private readonly List<string> addressAssetOwnerIds = "null".Split(';').ToList();
        private readonly List<string> addressVendorIds = "null".Split(';').ToList();

        // ASSETS (all owned by a single AssetOwner)

        private readonly List<string> assetReferences = "WC1535D;WC1929B;WC7935T;H18-002;H18-003;ACU-003;ACU-004;GDC-001;E90-001;WC4193L;WC4539Z;WC4593R;WC7172C;WC7438L;WC7551S;WC7651L;WC7683U;WD235X;WD239K;WD244U;WD259C;WD750A;WD217Z;H18-004;H18-005;H18-006;H18-007;H18-010;H18-013;H18-014;H18-016;H18-017;H18-019;H18-020;H18-021;M18-010;M18-011;M18-012;M18-027;M18-014;M18-020;M18-015;M18-017;M18-018;M18-019;M18-021;M18-023;M18-024;M18-025;PAC-001;PAC-003;EDC-002;GDC-003;GDC-004".Split(';').ToList();
        private readonly List<string> assetVehicleRegistrationNos = "WC1535D;WC1929B;WC7935T;H18-002;H18-003;ACU-003;ACU-004;GDC-001;E90-001;WC4193L;WC4539Z;WC4593R;WC7172C;WC7438L;WC7551S;WC7651L;WC7683U;WD235X;WD239K;WD244U;WD259C;WD750A;WD217Z;H18-004;H18-005;H18-006;H18-007;H18-010;H18-013;H18-014;H18-016;H18-017;H18-019;H18-020;H18-021;M18-010;M18-011;M18-012;M18-027;M18-014;M18-020;M18-015;M18-017;M18-018;M18-019;M18-021;M18-023;M18-024;M18-025;PAC-001;PAC-003;EDC-002;GDC-003;GDC-004".Split(';').ToList();
        private readonly List<string> assetLocations = "Abingdon Workshop / Singapre Changi Airport;Abingdon Workshop / Singapore Changi Airport;Abingdon Workshop;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Abingdon Workshop / Singapre Changi Airport;Seletar Airport;Singapore Changi Airport (SATS Cargo);Seletar Airport;Seletar Airport;Seletar Airport;Singapore Changi Airport;Abingdon Workshop / Singapre Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Seletar Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Marina Barage;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Abingdon Workshop / Singapre Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Abingdon Workshop / Singapre Changi Airport;Abingdon Workshop / Singapre Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Singapore Changi Airport;Philippines, Clark Airport;Philippines, Clark Airport;Seletar Airport;Seletar Airport;Seletar Airport".Split(';').ToList();
        private readonly List<string> assetSerialNumbers = "REC-001;REC-002;NULL;Hobart2;Hobart3;TLD03;TLD04;NULL;NULL;AVL-001;FOK-001;MAJ-001;PAS-001;SJS-001;NULL;AER-035;SJS-002;WOA-001;AER-091;AER-084;AER-084;PUB-001;AER-063;Hobart4;Hobart5;Hobart6;Hobart7;Hobart10;Hobart13;Hobart14;Hobart16;Hobart17;Hobart19;Hobart20;Hobart21;MAK10-18;MAK11-18;MAK12-18;MAK27-18;MAK14-18;MAK20-18;MAK15-18;MAK17-18;MAK18-18;MAK19-18;MAK21-18;MAK23-18;MAK24-18;MAK25-18;NULL;NULL;NULL;NULL;".Split(';').ToList();
        private readonly List<string> assetEngineNos = "2Z0113871;2Z0116558;S6S097580;10722364;10722370;9184744;9184743;NULL;11581985;1DZ0194827;2Z0125553;2Z0125553;2Z0125422;2Z0126303;2Z0126672;2Z0125003;2Z0126041;2Z0127368;2Z0130704;2Z0130720;2Z0130697;2Z0127256;2Z0130556;10722365;10722372;10722373;10850187;10850182;11075015;11075013;11075021;11119994;11075017;11075016;11075014;10433736;10433728;10433729;10433727;10433726;10433732;10492810;10492806;10492812;10492813;10492814;10492809;10492805;10433725;NULL;NULL;NULL;NULL;x".Split(';').ToList();
        private readonly List<string> assetChassisNos = "2TD2532335;2TD2532542;CF19D80074;409PS14850;409PS14851;T24824;T24825;10282;3128/14/01;2TD2517614;2TD2533009;2TD2533009;2TD2526860;2TD2526827;2TD2526864;2TD2526847;2TD2526817;2TD2526800;2TD2545054;2TD2545062;2TD2545051;2TD2526898;2TD2545000;409PS14869;409PS14870;409PS14871;210PS15316;210PS15375;311PS16792;311PS16793;311PS16795;411PS17074;411PS17017;411PS17108;411PS17109;631;632;633;635;637;640;688;690;691;692;693;695;696;697;20541710-00005;20552710-00046;3129/24/01;16480;16483".Split(';').ToList();
        private readonly List<string> assetDescriptions = "Towing Towing Tractor;Towing Towing Tractor;Forklift (Mitsubishi) ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;TLD Air Conditioning Unit;TLD Air Conditioning Unit;Guinault Static Converter;Effetti 90KVA Ground Power Unit;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Towing Towing Tractor;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;Hobart 180KVA Ground Power Unit ;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;MAK 180KVA Ground Power Unit;AC Portable Air Conditioning Unit;AC Portable Air Conditioning Unit;Effetti Static Converter;Guinault Static Converter SC20;Guinault Static Converter SC20".Split(';').ToList(); 
        private readonly List<string> assetPurchaseCosts = "50000;50000;50000;90000;90000;300000;300000;20000;70000;50000;50000;50000;50000;50000;50000;50000;50000;50000;50000;50000;50000;50000;50000;90000;90000;90000;90000;90000;90000;90000;90000;90000;90000;90000;90000;70000;70000;70000;70000;70000;70000;70000;70000;70000;70000;70000;70000;70000;70000;20000;20000;20000;20000;20000".Split(';').ToList();
        private readonly List<string> assetClassIds = "1;1;5;3;3;4;4;10;10;1;1;1;1;1;1;1;1;1;1;1;1;1;1;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;3;4;4;10;10;10".Split(';').ToList();
        private readonly List<string> assetStatusIds = "3;3;1;1;1;1;1;3;1;1;1;1;1;1;3;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;3;1;1;3;3;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1".Split(';').ToList();

        // LEASE AGREEMENTS

        private readonly List<string> leaseAgreementReferences = "HD373874F87".Split(';').ToList();
        private readonly List<string> leaseAgreementDescriptions = "2 ACUs for Clark Airport, Philippines".Split(';').ToList();
        private readonly List<string> leaseAgreementTitles = "Lease 2 ACUs for SIAEC (PH)".Split(';').ToList();
        private readonly List<string> leaseAgreementStartDates = "2019-11-12".Split(';').ToList();
        private readonly List<string> leaseAgreementEndDates = "2020-12-20".Split(';').ToList();
        private readonly List<string> leaseAgreementTerms = "Terms go here".Split(';').ToList();
        private readonly List<string> leaseAgreementContactIds = "1".Split(';').ToList();
        private readonly List<string> leaseAgreementAssetOwnerIds = "1".Split(';').ToList();
        private readonly List<string> leaseAgreementCustomerIds = "1".Split(';').ToList();

        private readonly List<string> leaseItemDateAllocateds = "2019-25-11".Split(';').ToList();
        private readonly List<string> leaseItemTerms = "-".Split(';').ToList();
        private readonly List<string> leaseItemUnitRentalRates = "2000".Split(';').ToList();
        private readonly List<string> leaseItemUnitDepositRates = "2000".Split(';').ToList();
        private readonly List<string> leaseItemStartDates = "2019-11-12".Split(';').ToList();
        private readonly List<string> leaseItemEndDates = "2020-12-20".Split(';').ToList();
        private readonly List<string> leaseItemRentalUomRefIds = "5".Split(';').ToList();
        private readonly List<string> leaseItemDepositUomRefIds = "null".Split(';').ToList();
        private readonly List<string> leaseItemItems = "WC1535D".Split(';').ToList();
        private readonly List<string> leaseItemDescriptions = "WC1535D".Split(';').ToList();
        private readonly List<string> leaseItemAssetClassIds = "null".Split(';').ToList();
        private readonly List<string> leaseItemAssetIds = "1".Split(';').ToList();
        private readonly List<string> leaseItemLeaseAgreementIds = "1".Split(';').ToList();

        // SUPPORT CONTRACTS

        private readonly List<string> supportContractReferences = "-".Split(';').ToList(); 
        private readonly List<string> supportContractTitles = "Singapore Full Service".Split(';').ToList();
        private readonly List<string> supportContractDescriptions = "PowerForce Maint Contract".Split(';').ToList();
        private readonly List<string> supportContractStartDates = "2019-11-26".Split(';').ToList();
        private readonly List<string> supportContractEndDates = "2020-11-26".Split(';').ToList();
        private readonly List<string> supportContractAcknowledgedBys = "somebody".Split(';').ToList();
        private readonly List<string> supportContractAcknowledgedAts = "2019-11-26".Split(';').ToList();
        private readonly List<string> supportContractVendorIds = "1".Split(';').ToList();

        private readonly List<string> supportItemDescriptions = "WC1535D - Full service Contract;WC1929B - FSM".Split(';').ToList();
        private readonly List<string> supportItemUnitPrices = "1000;1000".Split(';').ToList();
        private readonly List<string> supportItemFrequencies = "1;1".Split(';').ToList();
        private readonly List<string> supportItemIsAdHocs = "0;0".Split(';').ToList();
        private readonly List<string> supportItemIsChargeables = "0;0".Split(';').ToList();
        private readonly List<string> supportItemIsStandbys = "0;0".Split(';').ToList();
        private readonly List<string> supportItemAssetIds = "1;2".Split(';').ToList();
        private readonly List<string> supportItemAssetClassIds = "1;1".Split(';').ToList();
        private readonly List<string> supportItemUomIds = "5;5".Split(';').ToList();
        private readonly List<string> supportItemSupportContractIds = "1;1".Split(';').ToList();
        private readonly List<string> supportItemConsumableTypeIds = "null;null".Split(';').ToList();
        private readonly List<string> supportItemSupportTypeIds = "1;1".Split(';').ToList();

        public SeedDataCreator(EmsDbContext context)
        {
            _context = context;

        }

        [UnitOfWork]
        public void CreateTenants()
        {

            Random rnd = new Random();
            int counter = 0;

            var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            var defaultCurrenyId = _context.Currencies.Where(c => c.Code == "SGD").FirstOrDefault().Id;
            var defaultCustomerTypeId = _context.CustomerTypes.FirstOrDefault().Id;

            // Customers

            bool createCustomers = true;

            var customers = _context.Customers.ToList();
            if (customers.Count > 0)
            {
                var lastCustomer = customers.Last();
                if (customerNames.Contains(lastCustomer.Name))
                {
                    createCustomers = false;
                };
            }

            if (createCustomers)
            {
                do
                {
                    string tenancyName = customerReferences[counter];

                    Tenant newTenant = new Tenant()
                    {
                        IsActive = true,
                        TenancyName = tenancyName,
                        EditionId = defaultEdition.Id,
                        TenantType = "C",
                        IsInTrialPeriod = false,
                        Name = customerNames[counter]
                    };

                    Tenant createdTenant = _context.Tenants.Add(newTenant).Entity;
                    _context.SaveChanges();

                    Customer newCustomer = new Customer()
                    {
                        CurrencyId = defaultCurrenyId,
                        CustomerTypeId = defaultCustomerTypeId,
                        Identifier = tenancyName,
                        Name = customerNames[counter],
                        Reference = tenancyName,
                        TenantId = createdTenant.Id,
                        CustomerLoc8UUID = "",
                        LogoUrl = "",
                        Website = ""
                    };

                    Customer createdCustomer = _context.Customers.Add(newCustomer).Entity;
                    _context.SaveChanges();

                    counter = counter + 1;
                } while (counter < customerNames.Count());
            }


            // Vendors

            counter = 0;
            bool createVendors = true;

            var vendors = _context.Vendors.ToList();
            if (vendors.Count > 0)
            {
                var lastVendor = vendors.Last();
                if (vendorNames.Contains(lastVendor.Name))
                {
                    createVendors = false;
                };
            }

            if (createVendors)
            {
                do
                {
                    string tenancyName = vendorReferences[counter];

                    Tenant newTenant = new Tenant()
                    {
                        IsActive = true,
                        TenancyName = tenancyName,
                        EditionId = defaultEdition.Id,
                        TenantType = "V",
                        IsInTrialPeriod = false,
                        Name = vendorNames[counter]
                    };

                    Tenant createdTenant = _context.Tenants.Add(newTenant).Entity;
                    _context.SaveChanges();

                    Vendor newVendor = new Vendor()
                    {
                        CurrencyId = defaultCurrenyId,
                        Identifier = tenancyName,
                        Name = vendorNames[counter],
                        Reference = tenancyName,
                        TenantId = createdTenant.Id,
                        VendorLoc8GUID = "",
                        LogoUrl = "",
                        Website = "",
                    };

                    Vendor createdVendor = _context.Vendors.Add(newVendor).Entity;
                    _context.SaveChanges();

                    counter = counter + 1;
                } while (counter < vendorNames.Count());
            }

            // Asset Owners

            counter = 0;
            bool createAssetOwners = true;

            var assetOwners = _context.AssetOwners.ToList();
            if (assetOwners.Count > 0)
            {
                var lastAssetOwner = assetOwners.Last();
                if (assetOwnerNames.Contains(lastAssetOwner.Name))
                {
                    createAssetOwners = false;
                };
            }

            if (createAssetOwners)
            {
                do
                {
                    string tenancyName = assetOwnerNames[counter];

                    Tenant newTenant = new Tenant()
                    {
                        IsActive = true,
                        TenancyName = tenancyName,
                        EditionId = defaultEdition.Id,
                        TenantType = "A",
                        IsInTrialPeriod = false,
                        Name = assetOwnerNames[counter]
                    };

                    Tenant createdTenant = _context.Tenants.Add(newTenant).Entity;
                    _context.SaveChanges();

                    AssetOwner newAssetOwner = new AssetOwner()
                    {
                        CurrencyId = defaultCurrenyId,
                        Identifier = tenancyName,
                        Name = assetOwnerNames[counter],
                        Reference = tenancyName,
                        TenantId = createdTenant.Id,
                        LogoUrl = "",
                        Website = ""
                    };

                    AssetOwner createdAssetOwner = _context.AssetOwners.Add(newAssetOwner).Entity;
                    _context.SaveChanges();

                    counter = counter + 1;
                } while (counter < assetOwnerNames.Count());
            }

        }


        public void CreateData()
        {
            CreateAssets();
            CreateContacts();
            CreateLeaseAgeements();
            CreateContracts();
        }

        [UnitOfWork]
        private void CreateAssets()
        {
            // Assets

            int counter = 0;
            var assetOwner = _context.AssetOwners.FirstOrDefault();
            bool createAssets = true;

            var assets = _context.Assets.ToList();
            if (assets.Count > 0)
            {
                var lastAsset = assets.Last();
                if (assetReferences.Contains(lastAsset.Reference))
                {
                    createAssets = false;
                };
            }

            if (createAssets)
            {
                do
                {
                    int? assetClassId = (assetClassIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(assetClassIds[counter]) : null;
                    int? assetStatusId = (assetStatusIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(assetStatusIds[counter]) : null; 
                    decimal? purchaseCost = (assetPurchaseCosts[counter].ToUpper() != "NULL") ? (decimal?)Convert.ToInt32(assetPurchaseCosts[counter]) : null; 

                    Asset newAsset = new Asset()
                    {
                        AssetClassId = assetClassId,
                        AssetLoc8GUID = "",
                        AssetStatusId = assetStatusId,
                        ChassisNo = assetChassisNos[counter],
                        Description = assetDescriptions[counter],
                        EngineNo = assetEngineNos[counter],
                        IsExternalAsset = false,
                        Location = assetLocations[counter],
                        TenantId = null,
                        PurchaseCost = purchaseCost,
                        PurchaseDate = null,
                        PurchaseOrderNo =null,
                        Reference = assetReferences[counter],
                        SerialNumber = assetSerialNumbers[counter],
                        VehicleRegistrationNo = assetVehicleRegistrationNos[counter]
                    };

                    var createdAsset = _context.Assets.Add(newAsset).Entity;
                    _context.SaveChanges();

                    AssetOwnership newAssetOwnership = new AssetOwnership()
                    {
                        AssetId = createdAsset.Id,
                        AssetOwnerId = assetOwner.Id,
                        FinishDate = DateTime.UtcNow.AddYears(100),
                        PercentageOwnership = 100,
                        StartDate = DateTime.UtcNow.AddDays(-100),
                        TenantId = assetOwner.TenantId
                    };

                    _context.AssetOwnerships.Add(newAssetOwnership);
                    _context.SaveChanges();

                    counter = counter + 1;
                } while (counter < assetReferences.Count());
            }
        }

        [UnitOfWork]
        private void CreateContacts()
        {
            // Contacts

            int counter = 0;
            bool createContacts = true;

            var contacts = _context.Contacts.ToList();
            if (contacts.Count > 0)
            {
                var lastContact = contacts.Last();
                if (contactContactNames.Contains(lastContact.ContactName))
                {
                    createContacts = false;
                };
            }

            if (createContacts)
            {
                //var assetOwner = _context.AssetOwners.FirstOrDefault();
                do
                {
                    int? assetOwnerId = (contactAssetOwnerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(contactAssetOwnerIds[counter]) : null;
                    int? customerId = (contactCustomerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(contactCustomerIds[counter]) : null;
                    int? vendorId = (contactVendorIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(contactVendorIds[counter]) : null;
                    int? tenantId = _context.Customers.Where(c => c.Id == customerId).FirstOrDefault().TenantId;

                    Contact contact = new Contact()
                    {
                        AssetOwnerId = assetOwnerId,
                        CustomerId = customerId,
                        VendorId = vendorId,
                        TenantId = tenantId,
                        ContactLoc8GUID = "",
                        ContactName = contactContactNames[counter],
                        Department = contactDepartments[counter],
                        EmailAddress = contactEmailAddresses[counter],
                        Fax = contactFaxes[counter],
                        HeadOfficeContact = (contactIsHeadOfficeContacts[counter] != "0") ? true : false,
                        PhoneMobile = contactPhoneMobiles[counter],
                        PhoneOffice = contactPhoneOffices[counter],
                        Position = contactPositions[counter],
                        CreationTime = DateTime.Now,
                        IsDeleted = false
                    };

                    var createdContact = _context.Contacts.Add(contact).Entity;
                    _context.SaveChanges();
                    counter = counter + 1;
                } while (counter < contactContactNames.Count());
            }
        }

        [UnitOfWork]
        private void CreateAddresses()
        {
            // Addresses

            int counter = 0;
            bool createAddresses = true;

            var addresss = _context.Addresses.ToList();
            if (addresss.Count > 0)
            {
                var lastContact = addresss.Last();
                if (addressAddressEntryNames.Contains(lastContact.AddressEntryName))
                {
                    createAddresses = false;
                };
            }

            if (createAddresses)
            {
                //var assetOwner = _context.AssetOwners.FirstOrDefault();
                do
                {
                    int? vendorId = (addressAssetOwnerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(addressAssetOwnerIds[counter]) : null;
                    int? customerId = (addressCustomerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(addressCustomerIds[counter]) : null;
                    int? assetOwnerId = (addressAssetOwnerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(addressAssetOwnerIds[counter]) : null;
                    int? tenantId = (addressTenantIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(addressTenantIds[counter]) : null;

                    Address Address = new Address()
                    {
                        VendorId = vendorId,
                        CustomerId = customerId,
                        AssetOwnerId = assetOwnerId,
                        AddressEntryName = addressAddressEntryNames[counter],
                        AddressLine1 = addressAddressLine1s[counter],
                        AddressLine2 = addressAddressLine2s[counter],
                        AddressLoc8GUID = "",
                        City = addressCities[counter],
                        Country = addressCountries[counter],
                        IsDefaultForBilling = (addressIsDefaultForBillings[counter] != "0") ? true : false,
                        IsDefaultForShipping = (addressIsDefaultForShippings[counter] != "0") ? true : false,
                        IsHeadOffice = (addressIsHeadOffices[counter] != "0") ? true : false,
                        PostalCode = addressPostalCodes[counter],
                        State = addressStates[counter],
                        TenantId = tenantId,
                        CreationTime = DateTime.Now,
                        IsDeleted = false
                    };

                    Address createdAddress = _context.Addresses.Add(Address).Entity;
                    _context.SaveChanges();
                    counter = counter + 1;
                } while (counter < addressAddressEntryNames.Count());
            }
        }

        [UnitOfWork]
        private void CreateLeaseAgeements()
        {
            // Lease Agreements

            int counter = 0;
            bool createLeaseAgreements = true;

            var leaseAgreements = _context.LeaseAgreements.ToList();
            if (leaseAgreements.Count > 0)
            {
                var lastLeaseAgreement = leaseAgreements.Last();
                if (leaseAgreementReferences.Contains(lastLeaseAgreement.Reference))
                {
                    createLeaseAgreements = false;
                };
            }

            if (createLeaseAgreements)
            {
                var assetOwner = _context.AssetOwners.FirstOrDefault();
                do
                {
                    int? assetOwnerId = (leaseAgreementAssetOwnerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseAgreementAssetOwnerIds[counter]) : null;
                    int? customerId = (leaseAgreementCustomerIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseAgreementCustomerIds[counter]) : null;
                    DateTime? endDate = (leaseAgreementEndDates[counter].ToUpper() != "NULL") ? (DateTime?)DateTime.ParseExact(leaseAgreementEndDates[counter], "yyyy-mm-dd", null) : null;
                    DateTime? startDate = (leaseAgreementStartDates[counter].ToUpper() != "NULL") ? (DateTime?)DateTime.ParseExact(leaseAgreementStartDates[counter], "yyyy-mm-dd", null) : null;
                    int? contactId = (leaseAgreementContactIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseAgreementContactIds[counter]) : null;

                    LeaseAgreement leaseAgreement = new LeaseAgreement()
                    {
                        AssetOwnerId = assetOwnerId,
                        CustomerId = customerId,
                        Description = leaseAgreementDescriptions[counter],
                        EndDate = endDate,
                        StartDate = startDate,
                        Reference = leaseAgreementReferences[counter],
                        TenantId = assetOwner.TenantId,
                        Terms = leaseAgreementTerms[counter],
                        Title = leaseAgreementTitles[counter],
                        ContactId = contactId,
                        CreationTime = DateTime.Now,
                        IsDeleted = false
                    };

                    var createdLeaseAgreement = _context.LeaseAgreements.Add(leaseAgreement).Entity;
                    _context.SaveChanges();
                    counter = counter + 1;
                } while (counter < leaseAgreementReferences.Count());
            }

            // Add Assets

            counter = 0;
            bool createLeaseItems = true;

            var leaseItems = _context.LeaseItems.ToList();
            if (leaseItems.Count > 0)
            {
                var lastLeaseItem = leaseItems.Last();
                if (leaseItemItems.Contains(lastLeaseItem.Item))
                {
                    createLeaseItems = false;
                };
            }

            if (createLeaseItems)
            {
                var assetOwner = _context.AssetOwners.FirstOrDefault();
                do
                {
                    int? assetClassId = (leaseItemAssetClassIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseItemAssetClassIds[counter]) : null; 
                    int? assetId = (leaseItemAssetIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseItemAssetIds[counter]) : null;
                    DateTime? dateAllocated = (leaseItemDateAllocateds[counter].ToUpper() != "NULL") ? (DateTime?)DateTime.ParseExact(leaseItemDateAllocateds[counter], "yyyy-mm-dd", null) : null;
                    int? depositUomRefId = (leaseItemDepositUomRefIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseItemDepositUomRefIds[counter]) : null;
                    DateTime ? endDate = (leaseItemEndDates[counter].ToUpper() != "NULL") ? (DateTime?)DateTime.ParseExact(leaseItemEndDates[counter], "yyyy-mm-dd", null) : null;
                    DateTime?  startDate = (leaseItemStartDates[counter].ToUpper() != "NULL") ? (DateTime?)DateTime.ParseExact(leaseItemStartDates[counter], "yyyy-mm-dd", null) : null;
                    int? leaseAgreementId = (leaseItemLeaseAgreementIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseItemLeaseAgreementIds[counter]) : null;
                    int? rentalUomRefId = (leaseItemRentalUomRefIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(leaseItemRentalUomRefIds[counter]) : null;
                    decimal? unitDepositRate = (leaseItemUnitDepositRates[counter].ToUpper() != "NULL") ? (decimal?)Convert.ToInt32(leaseItemUnitDepositRates[counter]) : null; 
                    decimal? unitRentalRate = (leaseItemUnitRentalRates[counter].ToUpper() != "NULL") ? (decimal?)Convert.ToInt32(leaseItemUnitRentalRates[counter]) : null;

                    LeaseItem leaseItem = new LeaseItem()
                    {
                        AllocationPercentage = 100,
                        AssetClassId = assetClassId,
                        AssetId = assetId,
                        DateAllocated = dateAllocated,
                        DepositUomRefId =depositUomRefId,
                        Description = leaseItemDescriptions[counter],
                        EndDate = endDate,
                        StartDate = startDate,
                        Item = leaseItemItems[counter],
                        LeaseAgreementId = leaseAgreementId,
                        RentalUomRefId = rentalUomRefId,
                        Terms = leaseItemTerms[counter],
                        TenantId = assetOwner.TenantId,
                        UnitDepositRate = unitDepositRate,
                        UnitRentalRate = unitDepositRate,
                        CreationTime = DateTime.Now,
                        IsDeleted = false
                    };

                    var createdLeaseItem = _context.LeaseItems.Add(leaseItem).Entity;
                    _context.SaveChanges();
                    counter = counter + 1;
                } while (counter < leaseItemItems.Count());
            }
        }

        [UnitOfWork]
        private void CreateContracts()
        {

            // Support Contracts

            int counter = 0;
            bool createSupportContracts = true;

            var supportContracts = _context.SupportContracts.ToList();
            if (supportContracts.Count > 0)
            {
                var lastSupportContract = supportContracts.Last();
                if (supportContractDescriptions.Contains(lastSupportContract.Description))
                {
                    createSupportContracts = false;
                };
            }

            if (createSupportContracts)
            {
                var assetOwner = _context.AssetOwners.FirstOrDefault();
                do
                {

                    int? vendorId = (supportContractVendorIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(supportContractVendorIds[counter]) : null;

                    SupportContract supportContract = new SupportContract()
                    {
                        AssetOwnerId = assetOwner.Id,
                        VendorId = vendorId,
                        Description = supportContractDescriptions[counter],
                        EndDate = DateTime.ParseExact(supportContractEndDates[counter], "yyyy-mm-dd", null),
                        StartDate = DateTime.ParseExact(supportContractStartDates[counter], "yyyy-mm-dd", null) ,
                        Reference = supportContractReferences[counter],
                        TenantId = assetOwner.TenantId,
                        Title = supportContractTitles[counter],
                        AcknowledgedAt = DateTime.ParseExact(supportContractAcknowledgedAts[counter], "yyyy-mm-dd", null),
                        AcknowledgedBy = supportContractAcknowledgedBys[counter],
                        IsAcknowledged = (supportContractAcknowledgedBys[counter] != "0") ? true : false,
                        IsRFQTemplate = false,
                        CreationTime = DateTime.Now,
                        IsDeleted = false
                    };

                    var createdSupportContract = _context.SupportContracts.Add(supportContract).Entity;
                    _context.SaveChanges();
                    counter = counter + 1;
                } while (counter < supportContractReferences.Count());
            }

            // Add Assets

            counter = 0;
            bool createSupportItems = true;

            var supportItems = _context.SupportItems.ToList();
            if (supportItems.Count > 0)
            {
                var lastSupportItem = supportItems.Last();
                if (supportItemDescriptions.Contains(lastSupportItem.Description))
                {
                    createSupportItems = false;
                };
            }

            if (createSupportItems)
            {
                var assetOwner = _context.AssetOwners.FirstOrDefault();
                do
                {
                    int? assetClassId = (supportItemAssetClassIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(supportItemAssetClassIds[counter]) : null;
                    int? consumableTypeId = (supportItemConsumableTypeIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(supportItemConsumableTypeIds[counter]) : null;
                    int? frequency= (supportItemFrequencies[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(supportItemFrequencies[counter]) : null;
                    int? supportTypeId = (supportItemSupportTypeIds[counter].ToUpper() != "NULL") ? (int?)Convert.ToInt32(supportItemSupportTypeIds[counter]) : null;

                    SupportItem supportItem = new SupportItem()
                    {
                        AssetClassId = assetClassId,
                        AssetId = Convert.ToInt32(supportItemAssetIds[counter]),
                        Description = supportItemDescriptions[counter],
                        TenantId = assetOwner.TenantId,
                        ConsumableTypeId = consumableTypeId,
                        Frequency = frequency,
                        IsAdHoc = (supportItemIsAdHocs[counter] != "0") ? true : false,
                        IsChargeable = (supportItemIsChargeables[counter] != "0") ? true : false,
                        IsStandbyReplacementUnit = (supportItemIsStandbys[counter] != "0") ? true : false,
                        SupportContractId = Convert.ToInt32(supportItemSupportContractIds[counter]),
                        UomId = Convert.ToInt32(supportItemUomIds[counter]),
                        SupportTypeId = supportTypeId,
                        UnitPrice = Convert.ToInt32(supportItemUnitPrices[counter]),
                        CreationTime = DateTime.Now,
                        IsDeleted = false
                    };

                    var createdSupportItem = _context.SupportItems.Add(supportItem).Entity;
                    _context.SaveChanges();
                    counter = counter + 1;
                } while (counter < supportItemDescriptions.Count());
            }
        }
    }
}
