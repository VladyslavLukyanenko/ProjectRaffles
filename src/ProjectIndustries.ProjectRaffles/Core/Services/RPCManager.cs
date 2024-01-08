using DiscordRPC;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class RPCManager : IRPCManager
    {
        private DiscordRpcClient _rpcClient;

        public void UpdateState(string state)
        {
            if (_rpcClient?.IsInitialized ?? false)
            {
                return;
            }
      
            _rpcClient = new DiscordRpcClient("765985520056008794");
            _rpcClient.Initialize();
            _rpcClient.SetPresence(new RichPresence
            {
                Details = $"version: {AppConstants.CurrentAppVersion}",
                State = state,
                Timestamps = Timestamps.Now,
                Assets = new Assets
                {
                    LargeImageKey = "project_raffles_logo",
                    LargeImageText = "Project Raffles",
                }
            });
        }
    }
}