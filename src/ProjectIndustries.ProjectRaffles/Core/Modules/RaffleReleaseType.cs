using System.ComponentModel.DataAnnotations;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public enum RaffleReleaseType
  {
    [Display(Name = "FCFS")] Fcfs,
    [Display(Name = "Raffle")] Raffle,
    [Display(Name = "Raffle -> FCFS")] RaffleFcfs
  }
}