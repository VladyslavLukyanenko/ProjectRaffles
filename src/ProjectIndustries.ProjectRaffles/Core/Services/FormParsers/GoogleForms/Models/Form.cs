using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models
{
    public class Form
    {
        public string Title { get; set; } 
        public string Description { get; set; } 
        public bool Login { get; set; }
        public List<GoogleFormField> Fields { get; set; }  = new List<GoogleFormField>();
    }
}