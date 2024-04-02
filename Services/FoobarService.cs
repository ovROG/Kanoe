using Kanoe.Data.Models;
using Kanoe.Hubs;
using Kanoe.Shared;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using TwitchLib.Api.ThirdParty.ModLookup;
using TwitchLib.PubSub.Models.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kanoe.Services
{
    public class FoobarService : IObserver<ObservationEvent>
    {
        public struct Playlist
        {
            public string id { set; get; }
            public int index { set; get; }
            public string title { set; get; }
            public bool isCurrent { set; get; }
            public int itemCount { set; get; }
            public float totalTime { set; get; }
        }
        struct PlaylistsResponce
        {
            public Playlist[] playlists { get; set; }
        }

        struct YouTubeResponceItemStat
        {
            public string viewCount { get; set; }
            public string likeCount { get; set; }
            public string favoriteCount { get; set; }
            public string commentCount { get; set; }
        }
        struct YouTubeResponceItem
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public YouTubeResponceItemStat statistics { get; set; }
        }
        struct YouTubeResponce
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public YouTubeResponceItem[] items { get; set; }
            public object pageInfo { get; set; }
        }

        //--------------

        private readonly Config config;
        private readonly ActionsService actionsService;
        private readonly IHubContext<Actions, IActionsClient> actionsHub;

        HttpClient client = new HttpClient();

        string port = "8880";

        string? currentTrack;

        public FoobarService(Config configService, ActionsService aService, IHubContext<Actions, IActionsClient> aHub)
        {
            config = configService;
            actionsHub = aHub;
            Task.Run(ListenToEvents);
            actionsService = aService;
            aService.Subscribe(this);
        }

        private Task ListenToEvents()
        {
            HttpClient eventClient = new HttpClient();

            var stream = eventClient.GetStreamAsync($"http://localhost:{port}/api/query/updates?player=true&trcolumns=%artist%%20-%20%title%").Result;

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        dynamic? data = JsonConvert.DeserializeObject($"{{{reader.ReadLine()}}}" ?? "");
                        currentTrack = data.data.player.activeItem.columns[0];
                        actionsService.SetVarible("{nowplaying}",currentTrack);
                        actionsHub.Clients.All.NowPlaying(currentTrack);
                    }
                    catch
                    {
                        //proably just tick
                    }

                }
            }

            return Task.CompletedTask;
        }

        public async Task<List<Playlist>> GetPlaylists()
        {
            var responce = await client.GetAsync($"http://localhost:{port}/api/playlists");
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                return (await responce.Content.ReadFromJsonAsync<PlaylistsResponce>()).playlists.ToList();
            }
            else
            {
                Logger.Error("Unable To get Playlists");
                return new List<Playlist>();
            }
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
        }

        public virtual void OnNext(ObservationEvent e)
        {
            if (e.Event is FoobarAddYoutube fay)
            {
                _ = AddYoutube(e.Varibles["{text}"], fay.ViewsThreshold, fay.PlaylistId);
            }
        }

        public async Task AddYoutube(string url, int threshold, string playlistId)
        {
            Uri uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            string? videoId;

            if (query.AllKeys.Contains("v"))
            {
                videoId = query["v"];
            }
            else
            {
                videoId = uri.Segments.Last();
            }

            try
            {
                var stats = await client.GetFromJsonAsync<YouTubeResponce>($"https://youtube.googleapis.com/youtube/v3/videos?part=statistics&id={videoId}&key={config.GetYoutubeApiKey()}");
                if (Int32.Parse(stats.items[0].statistics.viewCount) > threshold)
                {
                    var postBody = new
                    {
                        async = true,
                        items = new[] { url },
                    };
                    var responce = await client.PostAsJsonAsync($"http://localhost:{port}/api/playlists/{playlistId}/items/add", postBody);
                }
                else
                {
                    Logger.Log($"{videoId} has low viewcount: {stats.items[0].statistics.viewCount}");
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to add video {url} to {playlistId}");
                Logger.Log($"With error: {e.Message}");
            }
        }

        public string? GetCurrentTrack()
        {
            return currentTrack;
        }
    }
}
