using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [Flags]
  public enum RaffleRegion
  {
    [Display(Name = "Europe")] Europe = 0x0000_0001,
    [Display(Name = "NA")] NorthAmerica = 0x0000_0010,
    [Display(Name = "SA")] SouthAmerica = 0x0000_0100,
    [Display(Name = "Oceania")] Oceania = 0x0000_1000,
    [Display(Name = "Asia")] Asia = 0x0001_0000,
    [Display(Name = "Africa")] Africa = 0x0010_0000
  }
}