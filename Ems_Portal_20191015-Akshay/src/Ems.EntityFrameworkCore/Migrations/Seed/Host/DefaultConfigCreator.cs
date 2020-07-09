using System.Linq;
using Abp.Configuration;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Ems.EntityFrameworkCore;
using Ems.Assets;
using System.Collections.Generic;
using Ems.Billing;
using Ems.Support;
using Ems.Customers;
using Ems.Quotations;
using Ems.Organizations;
using Ems.Metrics;
using Abp.Domain.Uow;

namespace Ems.Migrations.Seed.Host
{
    public class DefaultConfigCreator
    {
        private readonly EmsDbContext _context;

        public DefaultConfigCreator(EmsDbContext context)
        {
            _context = context;
        }

        [UnitOfWork]
        public void Create()
        {

            // AssetTypes

            List<AssetType> assetTypes = _context.AssetTypes.ToList();
            List<AssetType> newAssetTypes = new List<AssetType>()
            {
                new AssetType() { Code="TYP001", Type="Vehicle (motorised)", Description="Vehicle that moves with its own engine", Sort=1},
                new AssetType() { Code="TYP002", Type="Vehicle (Towed)", Description="Vehicle that must be Towed", Sort=2},
                new AssetType() { Code="TYP003", Type="Plant Equipment", Description="Workshop or other stationary Equiptment", Sort=3},
                new AssetType() { Code="TYP004", Type="Location", Description="Building / piece of land", Sort=4},
                new AssetType() { Code="TYP005", Type="IT Equipment", Description="Computers, etc", Sort=5},
                new AssetType() { Code="TYP006", Type="Software", Description="Software", Sort=6},
                new AssetType() { Code="TYP007", Type="Other", Description="Other", Sort=7}

            };
            foreach (var assetType in newAssetTypes)
            {
                if (assetTypes.Where(a => a.Type == assetType.Type).Count() == 0)
                {
                    _context.AssetTypes.Add(assetType);
                }
            }
            _context.SaveChanges();

            // AssetClasses

            var createdAssetTypes = _context.AssetTypes.ToList();

            List<AssetClass> assetClasses = _context.AssetClasses.ToList();
            List<AssetClass> newAssetClasses = new List<AssetClass>()
            {
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Vehicle (motorised)").FirstOrDefault().Id, Class = "Tractor", Manufacturer = "Toyota", Model = "ABC123", Specification = "White Baggage Tractor"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Vehicle (Towed)").FirstOrDefault().Id, Class = "Trolley", Manufacturer = "N/A", Model = "N/A", Specification = "Baggage Trolley"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Vehicle (Towed)").FirstOrDefault().Id, Class = "Ground Power Unit", Manufacturer = "N/A", Model = "N/A", Specification = "GPU"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Vehicle (Towed)").FirstOrDefault().Id, Class = "Aircon Unit", Manufacturer = "N/A", Model = "N/A", Specification = "ACU"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Plant Equipment").FirstOrDefault().Id, Class = "Workshop Forklift", Manufacturer = "Mitsubishi", Model = "N/A", Specification = "Forklift"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Location").FirstOrDefault().Id, Class = "Workshop", Manufacturer = "N/A", Model = "N/A", Specification = "Workshop"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "IT Equipment").FirstOrDefault().Id, Class = "Server Rack", Manufacturer = "IBM", Model = "ABC123", Specification = "IT Server"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Software").FirstOrDefault().Id, Class = "Cloud System", Manufacturer = "EMS", Model = "v1", Specification = "Asset Management"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Other").FirstOrDefault().Id, Class = "Office Furniture", Manufacturer = "N/A", Model = "N/A", Specification = "Furniture"},
                 new AssetClass() { AssetTypeId = createdAssetTypes.Where(a => a.Type == "Vehicle (Towed)").FirstOrDefault().Id, Class = "Static Converter", Manufacturer = "Guinault / Effetti", Model = "N/A", Specification = "N/A"}
            };
            foreach (var assetClass in newAssetClasses)
            {
                if (assetClasses.Where(a => a.Class == assetClass.Class).Count() == 0)
                {
                    _context.AssetClasses.Add(assetClass);
                }
            }
            _context.SaveChanges();


            // AssetStatuses

            List<AssetStatus> assetStatuses = _context.AssetStatuses.ToList();
            List<AssetStatus> newAssetStatuses = new List<AssetStatus>()
            {
                 new AssetStatus() {Status="Deployed", Description="Equipment is deployed to customer"},
                 new AssetStatus() {Status="Unservicable", Description="Asset needs repair before being used again"},
                 new AssetStatus() {Status="Idle", Description="Serviceable Asset not under Lease Contract"}

            };
            foreach (var assetStatus in newAssetStatuses)
            {
                if (assetStatuses.Where(a => a.Status == assetStatus.Status).Count() == 0)
                    if (!assetStatuses.Contains(assetStatus))
                {
                    _context.AssetStatuses.Add(assetStatus);
                }
            }
            _context.SaveChanges();


            // BillingEventTypes

            List<BillingEventType> billingEventTypes = _context.BillingEventTypes.ToList();
            List<BillingEventType> newBillingEventType = new List<BillingEventType>()
            {
                 new BillingEventType() {Type="Calendar", Description = "Bill created based on a certain date driven rules"},
                 new BillingEventType() {Type="Point of Sale", Description = "Bill created when invoice raised"},
                 new BillingEventType() {Type="Single Use Billing", Description = "Billing for single use of Asset"},
                 new BillingEventType() {Type="On UOM Milestone reached", Description = "When an Asset reaches a set UOM milestone (eg 10 litres of fuel)"},
            };
            foreach (var billingEventType in newBillingEventType)
            {
                if (billingEventTypes.Where(a => a.Type == billingEventType.Type).Count() == 0)
                {
                    _context.BillingEventTypes.Add(billingEventType);
                }
            }
            _context.SaveChanges();


            // BillingRuleTypes

            List<BillingRuleType> billingRuleTypes = _context.BillingRuleTypes.ToList();
            List<BillingRuleType> newBillingRuleType = new List<BillingRuleType>()
            {
                 new BillingRuleType() {Type="End on month bill", Description="An invoice is produced at the end of the Calendar month for payment" },
                 new BillingRuleType() {Type="Monthly billing on anniversary date", Description="Monthly bill produced on the day anniversary of the contract start" },
                 new BillingRuleType() {Type="Daily Billing", Description="Bill sent to customer at end of day" },
                 new BillingRuleType() {Type="On Commencement of Asset use", Description="When a user signs onto the asset, bill created" },
                 new BillingRuleType() {Type="On Completion of Asset use", Description="When a user signs out of the asset, bill created" },
                 new BillingRuleType() {Type="Fuel consumption", Description="Fuel consumption" },
                 new BillingRuleType() {Type="Bill created when invoice raised", Description="For sales or one-off services, immeditely raise an invoice for payment" }
            };
            foreach (var billingRuleType in newBillingRuleType)
            {
                if (billingRuleTypes.Where(a => a.Type == billingRuleType.Type).Count() == 0)
                {
                    _context.BillingRuleTypes.Add(billingRuleType);
                }
            }
            _context.SaveChanges();


            // ConsumableTypes

            List<ConsumableType> consumableTypes = _context.ConsumableTypes.ToList();
            List<ConsumableType> newConsumableType = new List<ConsumableType>()
            {
                 new ConsumableType() {Type="Types", Description = "All types of tyres for vehicles"},
                 new ConsumableType() {Type="Oils and Lubricants", Description = "Oils and Lubricants for servicing vehicles"}
            };
            foreach (var consumableType in newConsumableType)
            {
                if (consumableTypes.Where(a => a.Type == consumableType.Type).Count() == 0)
                {
                    _context.ConsumableTypes.Add(consumableType);
                }
            }
            _context.SaveChanges();


            // Currencies

            List<Currency> currencies = _context.Currencies.ToList();
            List<Currency> newCurrencies = new List<Currency>()
            {
                 new Currency() { Code = "AED" , Name = "United Arab Emirates dirham", BaseCountry = "United Arab Emirates", Country= "United Arab Emirates", Symbol="$" },
                 new Currency() { Code = "AFN" , Name = "Afghan afghani", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ALL" , Name = "Albanian lek", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "AMD" , Name = "Armenian dram", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ANG" , Name = "Netherlands Antillean guilder", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "AOA" , Name = "Angolan kwanza", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ARS" , Name = "Argentine peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "AUD" , Name = "Australian dollar", BaseCountry = "Australia", Country= "Australia", Symbol="$" },
                 new Currency() { Code = "AWG" , Name = "Aruban florin", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "AZN" , Name = "Azerbaijani manat", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BAM" , Name = "Bosnia and Herzegovina convertible mark", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BBD" , Name = "Barbados dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BDT" , Name = "Bangladeshi taka", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BGN" , Name = "Bulgarian lev", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BHD" , Name = "Bahraini dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BIF" , Name = "Burundian franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BMD" , Name = "Bermudian dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BND" , Name = "Brunei dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BOB" , Name = "Boliviano", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BOV" , Name = "Bolivian Mvdol (funds code)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BRL" , Name = "Brazilian real", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BSD" , Name = "Bahamian dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BTN" , Name = "Bhutanese ngultrum", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BWP" , Name = "Botswana pula", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BYN" , Name = "Belarusian ruble", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "BZD" , Name = "Belize dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CAD" , Name = "Canadian dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CDF" , Name = "Congolese franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CHE" , Name = "WIR Euro (complementary currency)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CHF" , Name = "Swiss franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CHW" , Name = "WIR Franc (complementary currency)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CLF" , Name = "Unidad de Fomento (funds code)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CLP" , Name = "Chilean peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CNY" , Name = "Renminbi (Chinese) yuan", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "COP" , Name = "Colombian peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "COU" , Name = "Unidad de Valor Real (UVR) (funds code)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CRC" , Name = "Costa Rican colon", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CUC" , Name = "Cuban convertible peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CUP" , Name = "Cuban peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CVE" , Name = "Cape Verde escudo", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "CZK" , Name = "Czech koruna", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "DJF" , Name = "Djiboutian franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "DKK" , Name = "Danish krone", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "DOP" , Name = "Dominican peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "DZD" , Name = "Algerian dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "EGP" , Name = "Egyptian pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ERN" , Name = "Eritrean nakfa", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ETB" , Name = "Ethiopian birr", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "EUR" , Name = "Euro", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "FJD" , Name = "Fiji dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "FKP" , Name = "Falkland Islands pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GBP" , Name = "Pound sterling", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GEL" , Name = "Georgian lari", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GHS" , Name = "Ghanaian cedi", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GIP" , Name = "Gibraltar pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GMD" , Name = "Gambian dalasi", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GNF" , Name = "Guinean franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GTQ" , Name = "Guatemalan quetzal", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "GYD" , Name = "Guyanese dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "HKD" , Name = "Hong Kong dollar", BaseCountry = "Hong Kong", Country= "Hong Kong", Symbol="$"   },
                 new Currency() { Code = "HNL" , Name = "Honduran lempira", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "HRK" , Name = "Croatian kuna", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "HTG" , Name = "Haitian gourde", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "HUF" , Name = "Hungarian forint", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "IDR" , Name = "Indonesian rupiah", BaseCountry = "Indonesia", Country= "Indonesia", Symbol="Rp" },
                 new Currency() { Code = "ILS" , Name = "Israeli new shekel", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "INR" , Name = "Indian rupee", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "IQD" , Name = "Iraqi dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "IRR" , Name = "Iranian rial", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ISK" , Name = "Icelandic króna", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "JMD" , Name = "Jamaican dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "JOD" , Name = "Jordanian dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "JPY" , Name = "Japanese yen", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KES" , Name = "Kenyan shilling", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KGS" , Name = "Kyrgyzstani som", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KHR" , Name = "Cambodian riel", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KMF" , Name = "Comoro franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KPW" , Name = "North Korean won", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KRW" , Name = "South Korean won", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KWD" , Name = "Kuwaiti dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KYD" , Name = "Cayman Islands dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "KZT" , Name = "Kazakhstani tenge", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "LAK" , Name = "Lao kip", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "LBP" , Name = "Lebanese pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "LKR" , Name = "Sri Lankan rupee", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "LRD" , Name = "Liberian dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "LSL" , Name = "Lesotho loti", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "LYD" , Name = "Libyan dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MAD" , Name = "Moroccan dirham", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MDL" , Name = "Moldovan leu", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MGA" , Name = "Malagasy ariary", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MKD" , Name = "Macedonian denar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MMK" , Name = "Myanmar kyat", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MNT" , Name = "Mongolian tögrög", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MOP" , Name = "Macanese pataca", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MRU" , Name = "Mauritanian ouguiya", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MUR" , Name = "Mauritian rupee", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MVR" , Name = "Maldivian rufiyaa", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MWK" , Name = "Malawian kwacha", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MXN" , Name = "Mexican peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MXV" , Name = "Mexican Unidad de Inversion(UDI) (funds code)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "MYR" , Name = "Malaysian ringgit", BaseCountry = "Malaysia", Country= "Malaysia", Symbol="RM"  },
                 new Currency() { Code = "MZN" , Name = "Mozambican metical", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "NAD" , Name = "Namibian dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "NGN" , Name = "Nigerian naira", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "NIO" , Name = "Nicaraguan córdoba", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "NOK" , Name = "Norwegian krone", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "NPR" , Name = "Nepalese rupee", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "NZD" , Name = "New Zealand dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "OMR" , Name = "Omani rial", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "PAB" , Name = "Panamanian balboa", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "PEN" , Name = "Peruvian sol", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "PGK" , Name = "Papua New Guinean kina", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "PHP" , Name = "Philippine piso", BaseCountry = "Philippines", Country= "Philippines", Symbol="₱"  },
                 new Currency() { Code = "PKR" , Name = "Pakistani rupee", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "PLN" , Name = "Polish złoty", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "PYG" , Name = "Paraguayan guaraní", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "QAR" , Name = "Qatari riyal", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "RON" , Name = "Romanian leu", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "RSD" , Name = "Serbian dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "RUB" , Name = "Russian ruble", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "RWF" , Name = "Rwandan franc", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SAR" , Name = "Saudi riyal", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SBD" , Name = "Solomon Islands dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SCR" , Name = "Seychelles rupee", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SDG" , Name = "Sudanese pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SEK" , Name = "Swedish krona/kronor", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SGD" , Name = "Singapore dollar", BaseCountry = "-", Country= "-", Symbol="$"  },
                 new Currency() { Code = "SHP" , Name = "Saint Helena pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SLL" , Name = "Sierra Leonean leone", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SOS" , Name = "Somali shilling", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SRD" , Name = "Surinamese dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SSP" , Name = "South Sudanese pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "STN" , Name = "São Tomé and Príncipe dobra", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SVC" , Name = "Salvadoran colón", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SYP" , Name = "Syrian pound", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "SZL" , Name = "Swazi lilangeni", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "THB" , Name = "Thai baht", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TJS" , Name = "Tajikistani somoni", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TMT" , Name = "Turkmenistan manat", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TND" , Name = "Tunisian dinar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TOP" , Name = "Tongan paʻanga", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TRY" , Name = "Turkish lira", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TTD" , Name = "Trinidad and Tobago dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TWD" , Name = "New Taiwan dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "TZS" , Name = "Tanzanian shilling", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "UAH" , Name = "Ukrainian hryvnia", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "UGX" , Name = "Ugandan shilling", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "USD" , Name = "United States dollar", BaseCountry = "-", Country= "-", Symbol="$"  },
                 new Currency() { Code = "USN" , Name = "United States dollar (next day) (funds code)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "UYI" , Name = "Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "UYU" , Name = "Uruguayan peso", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "UZS" , Name = "Uzbekistan som", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "VEF" , Name = "Venezuelan bolívar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "VND" , Name = "Vietnamese đồng", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "VUV" , Name = "Vanuatu vatu", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "WST" , Name = "Samoan tala", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XAF" , Name = "CFA franc BEAC", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XAG" , Name = "Silver (one troy ounce)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XAU" , Name = "Gold (one troy ounce)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XBA" , Name = "European Composite Unit(EURCO) (bond market unit)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XBB" , Name = "European Monetary Unit(E.M.U.-6) (bond market unit)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XBC" , Name = "European Unit of Account 9(E.U.A.-9) (bond market unit)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XBD" , Name = "European Unit of Account 17(E.U.A.-17) (bond market unit)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XCD" , Name = "East Caribbean dollar", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XDR" , Name = "Special drawing rights", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XOF" , Name = "CFA franc BCEAO", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XPD" , Name = "Palladium (one troy ounce)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XPF" , Name = "CFP franc(franc Pacifique)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XPT" , Name = "Platinum (one troy ounce)", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XSU" , Name = "SUCRE", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XTS" , Name = "Code reserved for testing purposes", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "XUA" , Name = "ADB Unit of Account", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "-" , Name = "No currency", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "YER" , Name = "Yemeni rial", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ZAR" , Name = "South African rand", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ZMW" , Name = "Zambian kwacha", BaseCountry = "-", Country= "-", Symbol="x"  },
                 new Currency() { Code = "ZWL" , Name = "Zimbabwean dollar A/10" , BaseCountry = "-", Country= "-", Symbol="x"  }
            };
            foreach (var currency in newCurrencies)
            {
                if (currencies.Where(a => a.Code == currency.Code).Count() == 0)
                {
                    _context.Currencies.Add(currency);
                }
            }
            _context.SaveChanges();


            // CustomerInvoiceStatuses

            List<CustomerInvoiceStatus> customerInvoiceStatuses = _context.CustomerInvoiceStatuses.ToList();
            List<CustomerInvoiceStatus> newCustomerInvoiceStatuses = new List<CustomerInvoiceStatus>()
            {
                 new CustomerInvoiceStatus() {Status="Created", Description = "Invoice has been Created" },
                 new CustomerInvoiceStatus() {Status="Submitted", Description = "Invoice has been Submitted" },
                 new CustomerInvoiceStatus() {Status="Rejected", Description = "Invoice has been Rejected" },
                 new CustomerInvoiceStatus() {Status="Approved", Description = "Invoice has been Approved" },
                 new CustomerInvoiceStatus() {Status="Cancelled", Description = "Invoice has been Cancelled" }
            };
            foreach (var customerInvoiceStatus in newCustomerInvoiceStatuses)
            {
                if (customerInvoiceStatuses.Where(a => a.Status == customerInvoiceStatus.Status).Count() == 0)
                {
                    _context.CustomerInvoiceStatuses.Add(customerInvoiceStatus);
                }
            }
            _context.SaveChanges();


            // CustomerTypes

            List<CustomerType> customerTypes = _context.CustomerTypes.ToList();
            List<CustomerType> newCustomerTypes = new List<CustomerType>()
            {
                 new CustomerType() {Type = "Leasing", Description = "Customer with Leasing contract"},
                 new CustomerType() {Type = "Service", Description = "Customer that receives mainly services"},
                 new CustomerType() {Type = "Sales", Description = "Customer that purchases Assets"}
            };
            foreach (var customerType in newCustomerTypes)
            {
                if (customerTypes.Where(a => a.Type == customerType.Type).Count() == 0)
                {
                    _context.CustomerTypes.Add(customerType);
                }
            }
            _context.SaveChanges();


            // EstimateStatuss

            List<EstimateStatus> estimateStatuses = _context.EstimateStatuses.ToList();
            List<EstimateStatus> newEstimateStatuses = new List<EstimateStatus>()
            {
                 new EstimateStatus() {Status="Created", Description = "Estimate has been Created"  },
                 new EstimateStatus() {Status="Submitted", Description = "Estimate has been Submitted" },
                 new EstimateStatus() {Status="Approved", Description = "Estimate has been Approved" },
                 new EstimateStatus() {Status="Rejected", Description = "Estimate has been Rejected" },
                 new EstimateStatus() {Status="Cancelled", Description = "Estimate has been Cancelled" }

            };
            foreach (var estimateStatus in newEstimateStatuses)
            {
                if (estimateStatuses.Where(a => a.Status == estimateStatus.Status).Count() == 0)
                {
                    _context.EstimateStatuses.Add(estimateStatus);
                }
            }
            _context.SaveChanges();


            // IncidentTypes

            List<IncidentType> incidentTypes = _context.IncidentTypes.ToList();
            List<IncidentType> newIncidentTypes = new List<IncidentType>()
            {
                 new IncidentType() {Type = "Breakdown", Description = "Asset failed in operation"},
                 new IncidentType() {Type = "Failed Inspection", Description = "Asset failed iInspection and requires rectification"}
            };
            foreach (var incidentType in newIncidentTypes)
            {
                if (incidentTypes.Where(a => a.Type == incidentType.Type).Count() == 0)
                {
                    _context.IncidentTypes.Add(incidentType);
                }
            }
            _context.SaveChanges();


            // IncidentStatuss

            List<IncidentStatus> incidentStatuses = _context.IncidentStatuses.ToList();
            List<IncidentStatus> newIncidentStatuses = new List<IncidentStatus>()
            {
                 new IncidentStatus() {Status = "Raised", Description = "Incident record created"},
                 new IncidentStatus() {Status = "Resolved", Description = "Incident resolved"}

            };
            foreach (var incidentStatus in newIncidentStatuses)
            {
                if (incidentStatuses.Where(a => a.Status == incidentStatus.Status).Count() == 0)
                {
                    _context.IncidentStatuses.Add(incidentStatus);
                }
            }
            _context.SaveChanges();


            // IncidentPriorities

            List<IncidentPriority> incidentPriorities = _context.IncidentPriorities.ToList();
            List<IncidentPriority> newIncidentPriorities = new List<IncidentPriority>() 
            {
                new IncidentPriority() { Priority = "Urgent", Description="Urgent priority",  PriorityLevel = 1 },
                new IncidentPriority() { Priority = "High", Description="High priority", PriorityLevel = 2 },
                new IncidentPriority() { Priority = "Medium", Description="Medium priority", PriorityLevel = 3 },
                new IncidentPriority() { Priority = "Low", Description="Low priority", PriorityLevel = 4 }
            };
            foreach (var incidentPriority in newIncidentPriorities)
            {
                if (incidentPriorities.Where(a => a.Priority == incidentPriority.Priority).Count() == 0)
                {
                    _context.IncidentPriorities.Add(incidentPriority);
                }
            }
            _context.SaveChanges();


            // ItemTypes

            List<ItemType> itemTypes = _context.ItemTypes.ToList();
            List<ItemType> newItemTypes = new List<ItemType>()
            {
                new ItemType() {Type = "12V Battery", Description = "12V Battery"},
                new ItemType() {Type = "Adjuster Cable", Description = "Adjuster Cable"},
                new ItemType() {Type = "Altermator Assy", Description = "Altermator Assy"},
                new ItemType() {Type = "Alternator Drive Belt", Description = "Alternator Drive Belt"},
                new ItemType() {Type = "ATF Dextron III", Description = "ATF Dextron III"},
                new ItemType() {Type = "Bom Repair Kit", Description = "Bom Repair Kit"},
                new ItemType() {Type = "Brake Booster Assy", Description = "Brake Booster Assy"},
                new ItemType() {Type = "Brake Cylinder", Description = "Brake Cylinder"},
                new ItemType() {Type = "Brake Fluid", Description = "Brake Fluid"},
                new ItemType() {Type = "Brake Lining", Description = "Brake Lining"},
                new ItemType() {Type = "Brake Servo Assy", Description = "Brake Servo Assy"},
                new ItemType() {Type = "Centre Bolt", Description = "Centre Bolt"},
                new ItemType() {Type = "Con Rod Bearing set", Description = "Con Rod Bearing set"},
                new ItemType() {Type = "Coolant", Description = "Coolant"},
                new ItemType() {Type = "Crankshaft grinding to specific size", Description = "Crankshaft grinding to specific size"},
                new ItemType() {Type = "Drive Shaft Seal", Description = "Drive Shaft Seal"},
                new ItemType() {Type = "Engine Mounting", Description = "Engine Mounting"},
                new ItemType() {Type = "Fire Extinguisher", Description = "Fire Extinguisher"},
                new ItemType() {Type = "Gasket & oil seals (half set)", Description = "Gasket & oil seals (half set)"},
                new ItemType() {Type = "Gasket set", Description = "Gasket set"},
                new ItemType() {Type = "Gearbox Mounting with Nuts & Bolts", Description = "Gearbox Mounting with Nuts & Bolts"},
                new ItemType() {Type = "Handbrake Cable", Description = "Handbrake Cable"},
                new ItemType() {Type = "Hanger Bushes", Description = "Hanger Bushes"},
                new ItemType() {Type = "Hub Seal", Description = "Hub Seal"},
                new ItemType() {Type = "Ignition Key Set + Wiring", Description = "Ignition Key Set + Wiring"},
                new ItemType() {Type = "King Pin Bearing Bush", Description = "King Pin Bearing Bush"},
                new ItemType() {Type = "King Pin Overhaul Kit", Description = "King Pin Overhaul Kit"},
                new ItemType() {Type = "L/RHF Signal Assembly + Bracketry", Description = "L/RHF Signal Assembly + Bracketry"},
                new ItemType() {Type = "Labour (Manhours)", Description = "Labour (Manhours)"},
                new ItemType() {Type = "Leaf Spring Hanger Bracket", Description = "Leaf Spring Hanger Bracket"},
                new ItemType() {Type = "Main Bearing set", Description = "Main Bearing set"},
                new ItemType() {Type = "Others", Description = "Others"},
                new ItemType() {Type = "PIston Con Rod ", Description = "PIston Con Rod "},
                new ItemType() {Type = "Piston Ring set", Description = "Piston Ring set"},
                new ItemType() {Type = "Propeller Shaft", Description = "Propeller Shaft"},
                new ItemType() {Type = "Radiator Assy (NEW)", Description = "Radiator Assy (NEW)"},
                new ItemType() {Type = "Radiator Assy (Recon)", Description = "Radiator Assy (Recon)"},
                new ItemType() {Type = "Radiator Expansion Tank", Description = "Radiator Expansion Tank"},
                new ItemType() {Type = "RHR Combination Lens", Description = "RHR Combination Lens"},
                new ItemType() {Type = "Solid Tyres", Description = "Solid Tyres Front"},
                new ItemType() {Type = "Solid Tyres", Description = "Solid Tyres Rear"},
                new ItemType() {Type = "Spring Rubber Bush", Description = "Spring Rubber Bush"},
                new ItemType() {Type = "Starter Motor Assy", Description = "Starter Motor Assy"},
                new ItemType() {Type = "Steer Drag Cylinder Assy (Re-con)", Description = "Steer Drag Cylinder Assy (Re-con)"},
                new ItemType() {Type = "Steering Gearbox Assy (Re-con)", Description = "Steering Gearbox Assy (Re-con)"},
                new ItemType() {Type = "Steering Pump Flange Banjo", Description = "Steering Pump Flange Banjo"},
                new ItemType() {Type = "Steering Supply Hose c/w Sleeve Guard", Description = "Steering Supply Hose c/w Sleeve Guard"},
                new ItemType() {Type = "Suspension Bearing & Con Set", Description = "Suspension Bearing & Con Set"},
                new ItemType() {Type = "Thermostat", Description = "Thermostat"},
                new ItemType() {Type = "Thrust Washer (STD)", Description = "Thrust Washer (STD)"},
                new ItemType() {Type = "Tie Rod Ball Joint", Description = "Tie Rod Ball Joint"},
                new ItemType() {Type = "Tow Linkage", Description = "Tow Linkage"},
                new ItemType() {Type = "Tow pin Lifter brackets c/w Handle", Description = "Tow pin Lifter brackets c/w Handle"},
                new ItemType() {Type = "Tow pin stop plate", Description = "Tow pin stop plate"},
                new ItemType() {Type = "Towing (Lowbed) -  one way", Description = "Towing (Lowbed) -  one way"},
                new ItemType() {Type = "Transmission inlet oil cooler hose", Description = "Transmission inlet oil cooler hose"},
                new ItemType() {Type = "Transmission to Engine Bell Housing End Plate Assy", Description = "Transmission to Engine Bell Housing End Plate Assy"},
                new ItemType() {Type = "Vane Pump Drive Belt", Description = "Vane Pump Drive Belt"},
                new ItemType() {Type = "Water Pump Assy", Description = "Water Pump Assy"},
                new ItemType() {Type = "Wheel Hub Inner & Outer Bearing with con c/w oil", Description = "Wheel Hub Inner & Outer Bearing with con c/w oil"}

            };
            foreach (var itemType in newItemTypes)
            {
                if (itemTypes.Where(a => a.Type == itemType.Type).Count() == 0)
                {
                    _context.ItemTypes.Add(itemType);
                }
            }
            _context.SaveChanges();


            // QuotationStatuss

            List<QuotationStatus> quotationStatuses = _context.QuotationStatuses.ToList();
            List<QuotationStatus> newQuotationStatuses = new List<QuotationStatus>()
            {
                new QuotationStatus() {Status="Created" , Description = "Quotation has been Created"},
                new QuotationStatus() {Status="Submitted" , Description = "Quotation has been Submitted"},
                new QuotationStatus() {Status="Approved" , Description = "Quotation Approved"},
                new QuotationStatus() {Status="Rejected" , Description = "Quotation Rejected"},
                new QuotationStatus() {Status="Canceled" , Description = "Quotation Canceled"}

            };
            foreach (var quotationStatus in newQuotationStatuses)
            {
                if (quotationStatuses.Where(a => a.Status == quotationStatus.Status).Count() == 0)
                {
                    _context.QuotationStatuses.Add(quotationStatus);
                }
            }
            _context.SaveChanges();


            // RfqTypes

            List<RfqType> rfqTypes = _context.RfqTypes.ToList();
            List<RfqType> newRfqTypes = new List<RfqType>()
            {
                 new RfqType() { Type="Default", Description="Default"},

            };
            foreach (var rfqType in newRfqTypes)
            {
                if (rfqTypes.Where(a => a.Type == rfqType.Type).Count() == 0)
                    if (!rfqTypes.Contains(rfqType))
                {
                    _context.RfqTypes.Add(rfqType);
                }
            }
            _context.SaveChanges();


            // SupportTypes

            List<SupportType> supportTypes = _context.SupportTypes.ToList();
            List<SupportType> newSupportTypes = new List<SupportType>()
            {
                new SupportType() {Type="Full Service Maintenance " , Description = "Covers Preventative and Reactive Maintenance"},
                new SupportType() {Type="Preventative Maintenance Only" , Description = "Preventative Maintenance Only"},
                new SupportType() {Type="Reactive Maintenance Only" , Description = "Reactive Maintenance Only (Towing and onsite repair)"}

            };
            foreach (var supportType in newSupportTypes)
            {
                if (supportTypes.Where(a => a.Type == supportType.Type).Count() == 0)
                    if (!supportTypes.Contains(supportType))
                {
                    _context.SupportTypes.Add(supportType);
                }
            }
            _context.SaveChanges();


            // SsicCodes

            List<SsicCode> ssicCodes = _context.SsicCodes.ToList();
            List<SsicCode> newSsicCodes = new List<SsicCode>()
            {
                 new SsicCode() { Code = "10001", SSIC = "Commerce - Retail Appliances / Articles / Equipment" },
                 new SsicCode() { Code = "10002", SSIC = "Commerce - Retail Books / Stationery / Gifts" },
                 new SsicCode() { Code = "10003", SSIC = "Commerce - Retail Children Specialty" },
                 new SsicCode() { Code = "10004", SSIC = "Commerce - Retail Computers/IT/Telecom/Office Apparatus" },
                 new SsicCode() { Code = "10005", SSIC = "Commerce - Retail Fabrics / Personal Effects" },
                 new SsicCode() { Code = "10006", SSIC = "Commerce - Retail Food / Beverages / Tobacco" },
                 new SsicCode() { Code = "10007", SSIC = "Commerce - Retail Furniture / Furnishings" },
                 new SsicCode() { Code = "10008", SSIC = "Commerce - Retail Medicinal / Pharmaceutical Products" },
                 new SsicCode() { Code = "10009", SSIC = "Commerce - Retail Non-Specialized Retail Trade in Stores" },
                 new SsicCode() { Code = "10010", SSIC = "Commerce - Retail Others" },
                 new SsicCode() { Code = "10011", SSIC = "Commerce - Retail Transport Equipment / Accessories" },
                 new SsicCode() { Code = "11001", SSIC = "Services Advertising" },
                 new SsicCode() { Code = "11002", SSIC = "Services Agricultural / Horticultural" },
                 new SsicCode() { Code = "11003", SSIC = "Services Consultancy / Business Activities" },
                 new SsicCode() { Code = "11004", SSIC = "Services Design" },
                 new SsicCode() { Code = "11005", SSIC = "Services Education / Training / Development" },
                 new SsicCode() { Code = "11006", SSIC = "Services Electrical / Electronics" },
                 new SsicCode() { Code = "11007", SSIC = "Services Employment Agencies" },
                 new SsicCode() { Code = "11008", SSIC = "Services Entertainment / Leisure" },
                 new SsicCode() { Code = "11009", SSIC = "Services Environmental" },
                 new SsicCode() { Code = "11010", SSIC = "Services Event Management" },
                 new SsicCode() { Code = "11011", SSIC = "Services Fabrics / Personal Effects" },
                 new SsicCode() { Code = "11012", SSIC = "Services Healthcare / Medical" },
                 new SsicCode() { Code = "11013", SSIC = "Services Landscaping" },
                 new SsicCode() { Code = "11014", SSIC = "Services Laundry" },
                 new SsicCode() { Code = "11015", SSIC = "Services Maintenance" },
                 new SsicCode() { Code = "11016", SSIC = "Services Others" },
                 new SsicCode() { Code = "11017", SSIC = "Services Pest Control" },
                 new SsicCode() { Code = "11018", SSIC = "Services Rental / Leasing" },
                 new SsicCode() { Code = "11019", SSIC = "Services Security" },
                 new SsicCode() { Code = "11020", SSIC = "Services Social / Welfare / Associations" },
                 new SsicCode() { Code = "11021", SSIC = "Services Waste Management" },
                 new SsicCode() { Code = "12001", SSIC = "Statutory Boards" },
                 new SsicCode() { Code = "13001", SSIC = "Transport / Storage Transport - Marine" },
                 new SsicCode() { Code = "13002", SSIC = "Transport / Storage Home Delivery" },
                 new SsicCode() { Code = "13003", SSIC = "Transport / Storage Last Mile Delivery" },
                 new SsicCode() { Code = "13004", SSIC = "Transport / Storage Movers" },
                 new SsicCode() { Code = "13005", SSIC = "Transport/Storage Supporting/Auxiliary Transport/Post Marine" },
                 new SsicCode() { Code = "13006", SSIC = "Transport/Storage Supporting/Auxiliary Transport/Post Others" },
                 new SsicCode() { Code = "13007", SSIC = "Transport / Storage Transport - Air" },
                 new SsicCode() { Code = "13008", SSIC = "Transport / Storage Transport - Bus" },
                 new SsicCode() { Code = "13009", SSIC = "Transport / Storage Transport - Container Operations" },
                 new SsicCode() { Code = "13010", SSIC = "Transport / Storage Transport - Heavy Equipment" },
                 new SsicCode() { Code = "13011", SSIC = "Transport / Storage Transport - Land" },
                 new SsicCode() { Code = "13012", SSIC = "Transport / Storage Transport - Oil & Gas" },
                 new SsicCode() { Code = "14001", SSIC = "Individual Person" },
                 new SsicCode() { Code = "20001", SSIC = "Commerce - Wholesale Agricultural / Animal Produce" },
                 new SsicCode() { Code = "20002", SSIC = "Commerce - Wholesale Books / Stationery / Gifts" },
                 new SsicCode() { Code = "20003", SSIC = "Commerce - Wholesale Chemicals / Chemical Products" },
                 new SsicCode() { Code = "20004", SSIC = "Commerce-Wholesale Computers/IT/Telecom/Office Appartus" },
                 new SsicCode() { Code = "20005", SSIC = "Commerce-Wholesale Construction Materials/Hardware/Metals" },
                 new SsicCode() { Code = "20006", SSIC = "Commerce - Wholesale Electrical / Electronics" },
                 new SsicCode() { Code = "20007", SSIC = "Commerce - Wholesale Fabrics / Personal Effects" },
                 new SsicCode() { Code = "20008", SSIC = "Commerce - Wholesale Food / Beverages / Tobacco" },
                 new SsicCode() { Code = "20009", SSIC = "Commerce - Wholesale Furniture / Furnishings" },
                 new SsicCode() { Code = "20010", SSIC = "Commerce - Wholesale General Wholesale Trade" },
                 new SsicCode() { Code = "20011", SSIC = "Commerce - Wholesale Machinery / Equipment NEC" },
                 new SsicCode() { Code = "20012", SSIC = "Commerce - Wholesale Medicinal / Pharmaceutical Products" },
                 new SsicCode() { Code = "20013", SSIC = "Commerce - Wholesale Paper Products" },
                 new SsicCode() { Code = "20014", SSIC = "Commerce-Wholesale Solid/Liquid/Gaseous Fuels and Related" },
                 new SsicCode() { Code = "20015", SSIC = "Commerce - Wholesale Transport Equipment / Accessories" },
                 new SsicCode() { Code = "20016", SSIC = "Commerce - Wholesale Wood Products" },
                 new SsicCode() { Code = "30001", SSIC = "Construction Electricity / Gas / Water" },
                 new SsicCode() { Code = "30002", SSIC = "Construction Fittings / Fixtures" },
                 new SsicCode() { Code = "30003", SSIC = "Construction General Construction Activities" },
                 new SsicCode() { Code = "30004", SSIC = "Construction Structural / Mechanical Engineering" },
                 new SsicCode() { Code = "40001", SSIC = "Finance Banking Services" },
                 new SsicCode() { Code = "40002", SSIC = "Finance Finance Services" },
                 new SsicCode() { Code = "40003", SSIC = "Finance Insurance / Re-Insurance Services" },
                 new SsicCode() { Code = "40004", SSIC = "Finance Insurance Brokers / Agencies" },
                 new SsicCode() { Code = "40005", SSIC = "Finance Investment / Stock Broking" },
                 new SsicCode() { Code = "40006", SSIC = "Finance Others" },
                 new SsicCode() { Code = "50001", SSIC = "Holdings Investment / Holding Activities" },
                 new SsicCode() { Code = "50002", SSIC = "Holdings Multi-Industry Activities" },
                 new SsicCode() { Code = "60001", SSIC = "Hospitality / F & B Food Establishments" },
                 new SsicCode() { Code = "60002", SSIC = "Hospitality / F & B Hotels" },
                 new SsicCode() { Code = "60003", SSIC = "Hospitality / F & B Others" },
                 new SsicCode() { Code = "70001", SSIC = "Information&Communications Computer/E-Commerce/IT related" },
                 new SsicCode() { Code = "70002", SSIC = "Information & Communications Infocomm" },
                 new SsicCode() { Code = "70003", SSIC = "Information & Communications Media" },
                 new SsicCode() { Code = "70004", SSIC = "Information & Communications Printing / Publishing" },
                 new SsicCode() { Code = "70005", SSIC = "Information & Communications Supporting Infocomm" },
                 new SsicCode() { Code = "80001", SSIC = "Manufacturing Chemical / Chemical Products" },
                 new SsicCode() { Code = "80002", SSIC = "Manufacturing Electrical / Electronics" },
                 new SsicCode() { Code = "80003", SSIC = "Manufacturing Fabrics / Personal Effects" },
                 new SsicCode() { Code = "80004", SSIC = "Manufacturing Food / Beverages / Tobacco" },
                 new SsicCode() { Code = "80005", SSIC = "Manufacturing Furniture / Furnishings" },
                 new SsicCode() { Code = "80006", SSIC = "Manufacturing Healthcare Related Products" },
                 new SsicCode() { Code = "80007", SSIC = "Manufacturing Machinery / Equipment" },
                 new SsicCode() { Code = "80008", SSIC = "Manufacturing Manufacturing - NEC" },
                 new SsicCode() { Code = "80009", SSIC = "Manufacturing Metal / Fabricated Metal" },
                 new SsicCode() { Code = "80010", SSIC = "Manufacturing Non Metallic Fabricated Products" },
                 new SsicCode() { Code = "80011", SSIC = "Manufacturing Paper Products" },
                 new SsicCode() { Code = "80012", SSIC = "Manufacturing Precision Instruments" },
                 new SsicCode() { Code = "80013", SSIC = "Manufacturing Rubber / Plastic" },
                 new SsicCode() { Code = "80014", SSIC = "Manufacturing Transport Equipment / Accessories" },
                 new SsicCode() { Code = "80015", SSIC = "Manufacturing Wood Products" },
                 new SsicCode() { Code = "90001", SSIC = "Property Real Estate Activities" },
                 new SsicCode() { Code = "90002", SSIC = "Property Real Estate Investment / Developers" }
            };
            foreach (var ssicCode in newSsicCodes)
            {
                if (ssicCodes.Where(a => a.Code == ssicCode.Code).Count() == 0)
                {
                    _context.SsicCodes.Add(ssicCode);
                }
            }
            _context.SaveChanges();


            // Uoms

            List<Uom> uoms = _context.Uoms.ToList();
            List<Uom> newUoms = new List<Uom>()
            {
                 new Uom() { UnitOfMeasurement ="Engine Revolutions"},
                 new Uom() { UnitOfMeasurement ="Hour"},
                 new Uom() { UnitOfMeasurement ="Kilometer"},
                 new Uom() { UnitOfMeasurement ="Litre"},
                 new Uom() { UnitOfMeasurement ="Month"},
                 new Uom() { UnitOfMeasurement ="Per Activation"}

            };
            foreach (var uom in newUoms)
            {
                if (uoms.Where(a => a.UnitOfMeasurement == uom.UnitOfMeasurement).Count() == 0)
                {
                    _context.Uoms.Add(uom);
                }
            }
            _context.SaveChanges();

            // VendorChargeStatuses

            List<VendorChargeStatus> vendorChargeStatuses = _context.VendorChargeStatuses.ToList();
            List<VendorChargeStatus> newVendorChargeStatuses = new List<VendorChargeStatus>()
            {
                 new VendorChargeStatus() {Status="Created", Description = "Vendor Charge has been Created" },
                 new VendorChargeStatus() {Status="Submitted", Description = "Vendor Charge has been Submitted" },
                 new VendorChargeStatus() {Status="Rejected", Description = "Vendor Charge has been Rejected" },
                 new VendorChargeStatus() {Status="Approved", Description = "Vendor Charge has been Approved" },
                 new VendorChargeStatus() {Status="Cancelled", Description = "Vendor Charge has been Cancelled" }
            };
            foreach (var vendorChargeStatus in newVendorChargeStatuses)
            {
                if (vendorChargeStatuses.Where(a => a.Status == vendorChargeStatus.Status).Count() == 0)
                {
                    _context.VendorChargeStatuses.Add(vendorChargeStatus);
                }
            }
            _context.SaveChanges();


            // WorkOrderPriorities

            List<WorkOrderPriority> workOrderPriorities = _context.WorkOrderPriorities.ToList();
            List<WorkOrderPriority> newWorkOrderPriorities = new List<WorkOrderPriority>()
            {
                 new WorkOrderPriority() { Priority="Urgent", PriorityLevel = 1},
                 new WorkOrderPriority() { Priority="High", PriorityLevel = 2},
                 new WorkOrderPriority() { Priority="Medium", PriorityLevel = 3},
                 new WorkOrderPriority() { Priority="Low", PriorityLevel = 4}
            };
            foreach (var workOrderPriority in newWorkOrderPriorities)
            {
                if (workOrderPriorities.Where(a => a.Priority == workOrderPriority.Priority).Count() == 0)
                {
                    _context.WorkOrderPriorities.Add(workOrderPriority);
                }
            }
            _context.SaveChanges();

            // WorkOrderStatuss

            List<WorkOrderStatus> workOrderStatuses = _context.WorkOrderStatuses.ToList();
            List<WorkOrderStatus> newWorkOrderStatuses = new List<WorkOrderStatus>()
            {
                 new WorkOrderStatus() {Status="Created", Description = "WorkOrder has been Created"  },
                 new WorkOrderStatus() {Status="Assigned", Description = "WorkOrder has been Assigned" },
                 new WorkOrderStatus() {Status="On Hold", Description = "WorkOrderis on hold" },
                 new WorkOrderStatus() {Status="Completed", Description = "WorkOrder has been Completed" },
                 new WorkOrderStatus() {Status="Cancelled", Description = "WorkOrder has been Cancelled" }

            };
            foreach (var workOrderStatus in newWorkOrderStatuses)
            {
                if (workOrderStatuses.Where(a => a.Status == workOrderStatus.Status).Count() == 0)
                {
                    _context.WorkOrderStatuses.Add(workOrderStatus);
                }
            }
            _context.SaveChanges();

            
            // WorkOrderTypes

            List<WorkOrderType> workOrderTypes = _context.WorkOrderTypes.ToList();
            List<WorkOrderType> newWorkOrderTypes = new List<WorkOrderType>()
            {
                 new WorkOrderType() { Type = "Preventative Maintenance", Description ="Vehicle checks and servicing as per the Manufacturers guidelines"},
                 new WorkOrderType() { Type = "Corrective Maintenance", Description ="Work needed after an Asset has broken down / been damaged"},
                 new WorkOrderType() { Type = "Inspection", Description ="Inspection only"},
                 new WorkOrderType() { Type = "Breakdown", Description ="Collection and/or repair of broken down vehicle"}
            };
            foreach (var workOrderType in newWorkOrderTypes)
            {
                if (workOrderTypes.Where(a => a.Type == workOrderType.Type).Count() == 0)
                {
                    _context.WorkOrderTypes.Add(workOrderType);
                }
            }
            _context.SaveChanges();

        }

    }
}