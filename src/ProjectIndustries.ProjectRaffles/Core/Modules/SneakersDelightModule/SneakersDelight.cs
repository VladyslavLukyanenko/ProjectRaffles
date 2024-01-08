using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SneakersDelightAccountGenerator;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SneakersDelightModule
{
    [RaffleModuleName("Sneakers Delight")]
    [RaffleModuleVersion(0, 0, 1)]
    [RaffleModuleType(RaffleModuleType.Custom)]
    [RafflePaymentProcessorType(PaymentProcessorType.None)]
    [RaffleRegion(RaffleRegion.Europe)]
    [RaffleReleaseType(RaffleReleaseType.Raffle)]
    [RaffleAccountGenerator(typeof(SneakersDelightAccountGenerator))]
    public class SneakersDelight : AccountBasedRaffleModuleBase<ISneakersDelightClient>
    {
        private readonly DynamicValuesPickerField _sizeValue = new DynamicValuesPickerField("size", "Size", true, null, Pickers.All)
        {
            // SelectedResolver = Pickers.Misc.ListItem
        };

        private readonly AddressFields _addressFields = new AddressFields
        {
            FirstName = null,
            LastName = null
        };

        public SneakersDelight(ISneakersDelightClient client)
            : base(client, @"https:\/\/sneakersdelight\.store\/.*", true)
        {
            AuthenticationConfig =
                new AuthenticationConfigHolder(Client.LoginAsync, AuthenticationConfigHolder.NoopCredentialsConfigurer);
        }

        protected override IEnumerable<object> GetDeclaredFields()
        {
            yield return base.GetDeclaredFields();
            yield return _sizeValue;
            yield return _addressFields;
        }

        protected override async Task<Product> FetchProductAsync(CancellationToken ct)
        {
            var product = await Client.GetProductAsync(RaffleUrl, ct);
            return new Product {Name = product};
        }

        protected override async Task<bool> ExecuteAsync(CancellationToken ct)
        {
            Status = RaffleStatus.GettingRaffleInfo;
            var parsed = await Client.ParseRaffleAsync(RaffleUrl, ct);
            
            Status = RaffleStatus.LoggingIntoAccount;
            await Client.LoginAsync(SelectedAccount, ct);
      
            Status = RaffleStatus.GettingAccountInfo;
            var accountData = await Client.GetAccountInformationAsync(_addressFields, ct);

            parsed.SizeDictionary.TryGetValue(_sizeValue, out string size);
            
            Status = RaffleStatus.Submitting;
            return await Client.SubmitEntryAsync(parsed, size, RaffleUrl, accountData, ct);
        }
    }
}