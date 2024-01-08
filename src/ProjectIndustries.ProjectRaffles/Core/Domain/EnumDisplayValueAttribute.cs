using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
    public class EnumDisplayValueAttribute
        : Attribute
    {
        public EnumDisplayValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}