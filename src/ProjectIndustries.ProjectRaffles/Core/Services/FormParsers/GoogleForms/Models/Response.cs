using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
    public class Response
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public Form Form { get; set; }
    }
}