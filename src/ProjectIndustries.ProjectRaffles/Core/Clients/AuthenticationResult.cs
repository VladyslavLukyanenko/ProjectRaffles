using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
    public class AuthenticationResult
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
            
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("discordId")]
        public long DiscordId { get; set; }
        
        [JsonProperty("discriminator")]
        public long Discriminator { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("username")]
        public string UserName { get; set; }
        
        [JsonProperty("version")]
        public string SoftwareVersion { get; set; }
        
        [JsonProperty("expiry")] public long? Expiry { get; set; }

        public static AuthenticationResult CreateUnkownError()
        {
            return new AuthenticationResult
            {
                IsSuccess = false,
                Message = "Unknown error occurred on authentication attempt"
            };
        }
    }
}