using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SneakersDelightModule
{
    public class SneakersDelightUserData
    {
        public SneakersDelightUserData(SneakersDelightDeserializeUserData userData, List<object> street)
        {
            UserData = userData;
            streetAddress = street;
        }
        public SneakersDelightDeserializeUserData UserData { get; set; }
        public List<object> streetAddress { get; set; }
    }
    
    public class SneakersDelightDeserializeUserData
    {
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
        public List<object> addresses { get; set; } 
        public int disable_auto_group_change { get; set; } 
        public SneakersDelightExtensionAttributes extension_attributes { get; set; } 
    }
    
}