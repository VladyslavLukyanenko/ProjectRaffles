using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models
{
    public class GoogleFormField
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public GoogleFormFieldType Type { get; set; }
        
        public IList<Option> Options { get; set; } = new List<Option>();
    }
}