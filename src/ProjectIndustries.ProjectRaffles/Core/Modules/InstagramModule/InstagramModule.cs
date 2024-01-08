using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.InstagramModule
{
    [RaffleModuleName("Instagram")]
    [RaffleModuleVersion(0, 0, 1)]
    [RaffleModuleType(RaffleModuleType.Custom)]
    public class Instagram : AccountBasedRaffleModuleBase<IInstagramModuleClient>
    {
        
        private readonly OptionsField _moduleType = new OptionsField("instagramSelection", "Module", true, new[] 
        {
         "Like",
         "Follow",
         "Comment",
         "Repost to story",
         "DM"
         });

        private readonly TextField _uploaderName = new TextField(displayName: "Users tag (@)", isRequired: false);
        
        private readonly TemplatedMultilineField _commentField = new TemplatedMultilineField(displayName: "Comment/DM", isRequired: false);
    
        public Instagram(IInstagramModuleClient client) : base(client, @"https:\/\/www\.instagram\.com\/.*")
        {
        }
        
        protected override IEnumerable<object> GetDeclaredFields()
        {
            yield return _moduleType;
            yield return _uploaderName;
            yield return _commentField;
            yield return base.GetDeclaredFields();
        }

        protected override Task<Product> FetchProductAsync(CancellationToken ct)
        {
            return Task.FromResult(new Product {Name = "Instagram"});
        }

        protected override async Task<bool> ExecuteAsync(CancellationToken ct)
        {
            switch (_moduleType.Value)
            {
                case "Like":
                {
                    Status = RaffleStatus.LoggingIntoAccount;
                    await Client.LoginAsync(SelectedAccount, ct);
 
                    Status = RaffleStatus.Submitting;
                    return await Client.PostLike(RaffleUrl, ct);
                }
                case "Comment":
                {
                    Status = RaffleStatus.LoggingIntoAccount;
                    await Client.LoginAsync(SelectedAccount, ct);

                    Status = RaffleStatus.Submitting;
                    return await Client.PostComment(RaffleUrl,  _commentField.Value, ct);
                }
                case "Follow":
                {
                    Status = RaffleStatus.LoggingIntoAccount;
                    await Client.LoginAsync(SelectedAccount, ct);

                    Status = RaffleStatus.Submitting;
                    return await Client.PostFollow(_uploaderName.Value, ct);
                }
                case "Repost to story":
                {
                    Status = RaffleStatus.GettingRaffleInfo;
                    var mediaInfo = await Client.GetMediaPk(RaffleUrl, ct);

                    var mediaPk = long.Parse(mediaInfo);
       
                    Status = RaffleStatus.LoggingIntoAccount;
                    await Client.LoginAsync(SelectedAccount, ct);
 
                    Status = RaffleStatus.Submitting;
                    return await Client.PostStoryAsync(mediaPk, ct);
                }
                case "DM":
                {
                    Status = RaffleStatus.LoggingIntoAccount;
                    await Client.LoginAsync(SelectedAccount, ct);
                    
                    Status = RaffleStatus.Submitting;
                    return await Client.PostDirectMessage(_uploaderName.Value, _commentField.Value);
                }
                default:
                    return false;
            }
        }
    }
}