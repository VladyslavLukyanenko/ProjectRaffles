using System.Reflection;

[assembly: Obfuscation(
  Feature = "4. encrypt symbol names with password YGtu9NQfZWoATcAlVrCtnHrHesIVZXSDpXwCDLxor2guuPwL6KVi1gBD785LShqs",
  Exclude = false)]
[assembly: Obfuscation(Feature = "5. apply to type ProjectIndustries.ProjectRaffles.Core.*: all", Exclude = false,
  ApplyToMembers = true)]
[assembly:
  Obfuscation(
    Feature = "4. apply to type ProjectIndustries.ProjectRaffles.Core.Modules.*: type renaming pattern 'x177c52e2f2948bd94179395f413be31'.*",
    Exclude = false)]
[assembly:
  Obfuscation(
    Feature =
      "4. apply to type ProjectIndustries.ProjectRaffles.Core.EventHandlers.*: type renaming pattern 'bb03f0343da547c58b078baca55e19f8'.*",
    Exclude = false)]
[assembly:
  Obfuscation(
    Feature =
      "4. apply to type ProjectIndustries.ProjectRaffles.Core.Validators.*: type renaming pattern 'ea698727491448139f116001226861eb'.*",
    Exclude = false)]
[assembly:
  Obfuscation(
    Feature = "4. apply to type ProjectIndustries.ProjectRaffles.Core.Clients.*: type renaming pattern 'qeb1d5dca7f14a8b8d12efc62241e2cb'.*",
    Exclude = false)]
[assembly:
  Obfuscation(
    Feature = "4. apply to type ProjectIndustries.ProjectRaffles.Core.Services.*: type renaming pattern 'e4ab726b704f4eb984a469cdbe33bed7'.*",
    Exclude = false)]
[assembly: Obfuscation(Feature = "2. apply to type ProjectIndustries.ProjectRaffles.WpfUI.*: renaming", Exclude = true,
  ApplyToMembers = true)]
[assembly: Obfuscation(Feature = "2. apply to type * when enum: renaming", Exclude = true, ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectIndustries.ProjectRaffles.Core.Domain.*: renaming", Exclude = true,
    ApplyToMembers = true)]
[assembly:
  Obfuscation(Feature = "2. apply to type ProjectIndustries.ProjectRaffles.Core.ViewModels.*: renaming", Exclude = true,
    ApplyToMembers = true)]

[assembly: Obfuscation(Feature = "1. apply to type CompiledAvaloniaXaml.*: apply to member *: renaming", Exclude = true,
  ApplyToMembers = true)]
// [assembly: Obfuscation(Feature = "debug", Exclude = false)]