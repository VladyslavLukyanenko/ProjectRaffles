using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;
using AutoMapperProfile = AutoMapper.Profile;

namespace ProjectIndustries.ProjectRaffles.Core
{
  public class AutoMapperGenericProfile : AutoMapperProfile
  {
    public AutoMapperGenericProfile()
    {
      CreateMap<Profile, CsvProfileData>()
        .ReverseMap()
        .ForMember(_ => _.Id, _ => _.Ignore())
        .ForMember(_ => _.Changed, _ => _.Ignore())
        .ForMember(_ => _.Changing, _ => _.Ignore())
        .ForMember(_ => _.ThrownExceptions, _ => _.Ignore());

      CreateMap<Account, CsvAccountData>()
        .ForMember(_ => _.GroupName, _ => _.Ignore())
        .ReverseMap()
        .ForMember(_ => _.Id, _ => _.Ignore());

      CreateMap<Proxy, CsvProxyData>()
        .ForMember(_ => _.GroupName, _ => _.Ignore())
        .ReverseMap()
        .ForMember(_ => _.Id, _ => _.Ignore())
        .ForMember(_ => _.IsAvailable, _ => _.Ignore());
    }
  }
}