using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SneakersDelightModule
{
    public class SneakersDelightAccountInformation
    {
        public SneakersDelightCustomerInformation customer { get; set; }
    }
    
    public class SneakersDelightAddressInformation {
        public string firstname { get; set; } 
        public string lastname { get; set; } 
        public string vat_id { get; set; } 
        public string telephone { get; set; } 
        public List<string> street { get; set; } 
        public string city { get; set; } 
        public string postcode { get; set; } 
        public string country_id { get; set; } 
        public string region { get; set; } 
        public object region_id { get; set; } 
        public int customer_id { get; set; } 
    }

    public class SneakersDelightExtensionAttributes{
        public bool is_subscribed { get; set; } 
    }

    public class SneakersDelightCustomerInformation{
        public int id { get; set; } 
        public int group_id { get; set; } 
        public string created_at { get; set; } 
        public string updated_at { get; set; } 
        public string created_in { get; set; } 
        public string dob { get; set; } 
        public string email { get; set; } 
        public string firstname { get; set; } 
        public string lastname { get; set; } 
        public int gender { get; set; } 
        public int store_id { get; set; } 
        public int website_id { get; set; } 
        public List<SneakersDelightAddressInformation> addresses { get; set; } 
        public int disable_auto_group_change { get; set; } 
        public SneakersDelightExtensionAttributes extension_attributes { get; set; } 
    }
}