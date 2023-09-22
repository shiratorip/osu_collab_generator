using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OCG;

internal static class MainWindowHelpers
{
    public static async Task<SearchResponse?> GetResponse(string url)
    {

        using var client = new HttpClient();

        HttpResponseMessage responseMessage = await client.GetAsync(url);
        if (responseMessage.IsSuccessStatusCode)
        {
            string response = await responseMessage.Content.ReadAsStringAsync();
            try {
                SearchResponse users = JsonConvert.DeserializeObject<SearchResponse>(response);
            Trace.WriteLine($"\n {users} \n"); 
                return users;
            }
            catch(Newtonsoft.Json.JsonSerializationException ex)
            {
                Trace.WriteLine("=======================");
            }
            return null;
           
        }
        else
        {
            return null;
        }


    }
  



    

    public class SearchResponse
    {
        [JsonProperty("user")] public SearchResult<User>? User { get; set; }
        [JsonProperty("wiki_page")] public SearchResult<WikiPage>? WikiPage { get; set; }
    }

    public class SearchResult<T>
    {
        [JsonProperty("data")] public T[] Data { get; set; }
        [JsonProperty("total")] public int Total { get; set; }
    }

    public class User
    {
        [JsonProperty("avatar_url")] public string AvatarUrl { get; set; }
        [JsonProperty("country_code")] public string CountryCode { get; set; }
        [JsonProperty("default_group")] public string DefaultGroup { get; set; }
       
        [JsonProperty("is_active")] public bool IsActive { get; set; }
        [JsonProperty("is_bot")] public bool IsBot { get; set; }
        [JsonProperty("is_deleted")] public bool IsDeleted { get; set; }
        [JsonProperty("is_online")] public bool IsOnline { get; set; }
        [JsonProperty("is_supporter")] public bool IsSupporter { get; set; }
        [JsonProperty("last_visit")] public DateTime? LastVisit { get; set; }
        [JsonProperty("pm_friends_only")] public bool PmFriendOnly { get; set; }
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("username")] public string Username { get; set; }

    /*
        [JsonProperty("cover_url")] public string CoverUrl { get; set; }
        [JsonProperty("discord")] public string Discord { get; set; }
        [JsonProperty("has_supported")] public bool HasSupported { get; set; }
        [JsonProperty("interests")] public string Interests { get; set; }
        [JsonProperty("join_date")] public DateTime JoinDate { get; set; }
        [JsonProperty("kudosu")] public UserKudosu Kudosu { get; set; }
        [JsonProperty("lastfm")] public string LastFm { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("max_blocks")] public int MaxBlocks { get; set; }
        [JsonProperty("max_friends")] public int MaxFriends { get; set; }
        [JsonProperty("playmode")] public string PlayMode { get; set; }
        [JsonProperty("playstyle")] public string[] PlayStyle { get; set; }
        [JsonProperty("post_count")] public int PostCount { get; set; }
        [JsonProperty("profile_order")] public string[] ProfileOrder { get; set; }
        [JsonProperty("skype")] public string Skype { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("website")] public string Website { get; set; }
        [JsonProperty("country")] public UserCountry Country { get; set; }
        [JsonProperty("cover")] public UserCover Cover { get; set; }
        [JsonProperty("badges")] public UserBadge[] Badges { get; set; }
        [JsonProperty("favourite_beatmapset_count")] public int FavoriteBeatmapSetCount { get; set; }
        [JsonProperty("follower_count")] public int FollowerCount { get; set; }
        [JsonProperty("loved_beatmapset_count")] public int LovedBeatmapSetCount { get; set; }
        [JsonProperty("monthly_playcounts")] public UserPlayCount[] MonthlyPlayCount { get; set; }
        [JsonProperty("page")] public UserPage Page { get; set; }
        [JsonProperty("previous_usernames")] public string[] PreviousUsernames { get; set; }
        [JsonProperty("ranked_and_approved_beatmapset_count")] public int RankedAndApprovedBeatmapSetCount { get; set; }
        [JsonProperty("replays_watched_counts")] public UserWatchedCount[] ReplaysWatchedCounts { get; set; }
        [JsonProperty("scores_first_count")] public int ScoresFirstCount { get; set; }
        [JsonProperty("statistics")] public UserStatistics Statistics { get; set; }
        [JsonProperty("support_level")] public int SupportLevel { get; set; }
        [JsonProperty("unranked_beatmapset_count")] public int UnrankedBeatmapSetCount { get; set; }
        [JsonProperty("user_achievements")] public UserAchievement[] UserAchievements { get; set; }
        [JsonProperty("rankHistory")] public UserRankHistory RankHistory { get; set; }
    */
    }

    public class UserKudosu
    {
        [JsonProperty("total")] public int Total { get; set; }
        [JsonProperty("available")] public int Available { get; set; }
    }

    public class UserCountry
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }

    public class UserCover
    {
        [JsonProperty("custom_url")] public string CustomUrl { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("id")] public int? Id { get; set; }
    }

    public class UserBadge
    {
        [JsonProperty("awarded_at")] public DateTime AwardedAt { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("image_url")] public string ImageUrl { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
    }

    public class UserPlayCount
    {
        [JsonProperty("start_date")] public DateTime StartDate { get; set; }
        [JsonProperty("count")] public int Count { get; set; }
    }

    public class UserPage
    {
        [JsonProperty("html")] public string Html { get; set; }
        [JsonProperty("raw")] public string Raw { get; set; }
    }

    public class UserWatchedCount
    {
        [JsonProperty("start_date")] public DateTime StartDate { get; set; }
        [JsonProperty("count")] public int Count { get; set; }
    }

    public class UserStatistics
    {
        [JsonProperty("level")] public UserStatisticsLevel Level { get; set; }
        [JsonProperty("pp")] public float pp { get; set; }
        [JsonProperty("pp_rank")] public int ppRank { get; set; }
        [JsonProperty("ranked_score")] public long RankedScore { get; set; }
        [JsonProperty("hit_accuracy")] public float HitAccuracy { get; set; }
        [JsonProperty("play_count")] public int PlayCount { get; set; }
        [JsonProperty("play_time")] public int PlayTime { get; set; }
        [JsonProperty("total_score")] public long TotalScore { get; set; }
        [JsonProperty("total_hits")] public int TotalHits { get; set; }
        [JsonProperty("maximum_combo")] public int MaximumCombo { get; set; }
        [JsonProperty("replays_watched_by_others")] public int ReplaysWatchedByOthers { get; set; }
        [JsonProperty("is_ranked")] public bool IsRanked { get; set; }
        [JsonProperty("grade_counts")] public UserStatisticsGradeCounts GradeCounts { get; set; }
        [JsonProperty("rank")] public UserStatisticsRank Rank { get; set; }
    }

    public class UserStatisticsLevel
    {
        [JsonProperty("current")] public int Current { get; set; }
        [JsonProperty("progress")] public int Progress { get; set; }
    }

    public class UserStatisticsGradeCounts
    {
        [JsonProperty("ss")] public int SS { get; set; }
        [JsonProperty("ssh")] public int SSH { get; set; }
        [JsonProperty("s")] public int S { get; set; }
        [JsonProperty("sh")] public int SH { get; set; }
        [JsonProperty("a")] public int A { get; set; }
    }

    public class UserStatisticsRank
    {
        [JsonProperty("global")] public int Global { get; set; }
        [JsonProperty("country")] public int Country { get; set; }
    }

    public class UserAchievement
    {
        [JsonProperty("achieved_at")] public DateTime AchievedAt { get; set; }
        [JsonProperty("achievement_id")] public int AchievementId { get; set; }
    }

    public class UserRankHistory
    {
        [JsonProperty("mode")] public string Mode { get; set; }
        [JsonProperty("data")] public int[] Data { get; set; }
    }

    public class WikiPage
    {
        [JsonProperty("available_locales")] public string[] AvailableLocales { get; set; }
        [JsonProperty("layout")] public string Layout { get; set; }
        [JsonProperty("locale")] public string Locale { get; set; }
        [JsonProperty("markdown")] public string Markdown { get; set; }
        [JsonProperty("path")] public string Path { get; set; }
        [JsonProperty("subtitle")] public string? Subtitle { get; set; }
        [JsonProperty("tags")] public string[] Tags { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
    }
#pragma warning restore CS8618 // Dereference of a possibly null reference.

    public record ClientCredentialsGrant(ulong clientId, string clientSecret);
}