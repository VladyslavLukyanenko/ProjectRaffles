using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.InstagramModule
{
    public class InstagramModuleClient : InstaHttpClientBase, IInstagramModuleClient
    {
        private IInstaApi _instaApi;

        public async Task<string> GetMediaPk(string url, CancellationToken ct)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
            httpClient.DefaultRequestHeaders.Add("accept-language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
            var getPage = await httpClient.GetAsync(url, ct);
            var pageContent = await getPage.ReadStringResultOrFailAsync("Can't access image", ct);
            
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(pageContent);

            var imageIdFull = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='al:ios:url']").GetAttributeValue("content","");
            var imageId = imageIdFull.Replace("instagram://media?id=", "");

            return imageId;
        }

        public async Task LoginAsync(Account account, CancellationToken ct)
        {
            var userSession = new UserSessionData
            {
                UserName = account.Email,
                Password = account.Password
            };
            
            _instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseHttpClientHandler(HttpClientHandler)
                .Build();
            
            var logInResult = await _instaApi.LoginAsync();
            if (!logInResult.Succeeded)
            {
                throw new RaffleFailedException("cannot login", "Cannot login to account: " + account.Email);
            }
        }

        public async Task<bool> PostComment(string mediaUrl, string comment, CancellationToken ct)
        {
            var uri = new Uri(mediaUrl);
            var mediaId = await _instaApi.MediaProcessor.GetMediaIdFromUrlAsync(uri);
            if(!mediaId.Succeeded) throw new RaffleFailedException($"Getting media ID failed for URL: {mediaUrl}, error: {mediaId.Info}", "Failed on getting media ID");
            
            var sendComment = await _instaApi.CommentProcessor.CommentMediaAsync(mediaId.Value, comment);
            
            if(!sendComment.Succeeded) throw new RaffleFailedException("Cannot add comment","Cannot add comment: " + sendComment.Info);
            
            return sendComment.Succeeded;
        }
        
        public async Task<bool> PostFollow(string username, CancellationToken ct)
        {
            var user = await _instaApi.UserProcessor.GetUserAsync(username);
            if(!user.Succeeded) throw new RaffleFailedException("Error on getting User PK: " +user.Info, "Can't get user ID");
            var userId = user.Value.Pk;
            
            var followUser = await _instaApi.UserProcessor.FollowUserAsync(userId);
            
            if(!followUser.Succeeded) throw new RaffleFailedException("Error on following: "+ userId + " Message: " + followUser.Info, "Error on following");

            return followUser.Succeeded;
        }
        
        public async Task<bool> PostLike(string mediaUrl, CancellationToken ct)
        {
            var uri = new Uri(mediaUrl);
            var mediaId = await _instaApi.MediaProcessor.GetMediaIdFromUrlAsync(uri);
            if(!mediaId.Succeeded) throw new RaffleFailedException($"Getting media ID failed for URL: {mediaUrl}, error: {mediaId.Info}", "Failed on getting media ID");
            
            var likeResult = await _instaApi.MediaProcessor.LikeMediaAsync(mediaId.Value);
            
            if(!likeResult.Succeeded) throw new RaffleFailedException("Error on like, "+likeResult.Info, "Error on like");

            return likeResult.Succeeded;
        }
        
        public async Task<bool> PostStoryAsync( long mediaPk, CancellationToken ct)
        {
            var mediaStory = new InstaMediaStoryUpload
            {
                X = 0.5, // center of photo
                Y = 0.5, // center of photo
                Width = 0.5, // height of clickable media, it's an square in center of photo
                Height = 0.5, // width of clickable media, it's an square in center of photo
                Rotation = 0, // don't change this
                MediaPk = mediaPk
            };

            var storageLocation =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ProjectRaffles");
            
            if (!File.Exists(storageLocation + @"\igstorybackground.jpg"))
            {
                var httpClient = new HttpClient();
                var downloadImage = await httpClient.GetAsync(
                    "https://buffer.com/resources/content/images/resources/wp-content/uploads/2019/12/luke-chesser-B_oL3jEt5L4-unsplash.jpg",
                    ct);

                var imageStream = await downloadImage.Content.ReadAsStreamAsync(ct);

                var imageName = storageLocation + @"\igstorybackground.jpg";
                using (var fs = new FileStream(imageName, FileMode.CreateNew))
                {
                    await imageStream.CopyToAsync(fs, ct);
                }
            }
            
            var image = new InstaImage {Uri = storageLocation + @"\igstorybackground.jpg"};
            
            var result = await _instaApi.StoryProcessor.ShareMediaAsStoryAsync(image, mediaStory);

            if(!result.Succeeded) throw new RaffleFailedException("Error on repost" + result.Info,"Error on repost");

            return result.Succeeded;
        }

        public async Task<bool> PostDirectMessage(string userToDm, string message)
        {
            var user = await _instaApi.UserProcessor.GetUserAsync(userToDm);
            if(!user.Succeeded) throw new RaffleFailedException("Error on getting User PK: " +user.Info, "Can't get user ID");
            var userId = user.Value.Pk.ToString();
            
            var directText = await _instaApi.MessagingProcessor.SendDirectTextAsync(userId, null, message);
            if(!directText.Succeeded) throw new RaffleFailedException("Can't DM: " + userId + " Error: " + directText.Info, "Error on sending DM");

            return directText.Succeeded;
        }
    }
}