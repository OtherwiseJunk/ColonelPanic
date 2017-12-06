using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Utilities.JSONClasses
{

    public class MediaEmbed
    {
        public string content { get; set; }
        public int? width { get; set; }
        public bool? scrolling { get; set; }
        public int? height { get; set; }
    }

    public class Oembed
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_height { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
    }

    public class SecureMedia
    {
        public Oembed oembed { get; set; }
        public string type { get; set; }
    }

    public class SecureMediaEmbed
    {
        public string content { get; set; }
        public int? width { get; set; }
        public bool? scrolling { get; set; }
        public string media_domain_url { get; set; }
        public int? height { get; set; }
    }

    public class RedditVideo
    {
        public string fallback_url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string scrubber_media_url { get; set; }
        public string dash_url { get; set; }
        public int duration { get; set; }
        public string hls_url { get; set; }
        public bool is_gif { get; set; }
        public string transcoding_status { get; set; }
    }

    public class Oembed2
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_height { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
    }

    public class Media
    {
        public RedditVideo reddit_video { get; set; }
        public Oembed2 oembed { get; set; }
        public string type { get; set; }
    }

    public class MediaEmbed2
    {
        public string content { get; set; }
        public int width { get; set; }
        public bool scrolling { get; set; }
        public int height { get; set; }
    }

    public class Oembed3
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_height { get; set; }
    }

    public class SecureMedia2
    {
        public Oembed3 oembed { get; set; }
        public string type { get; set; }
    }

    public class SecureMediaEmbed2
    {
        public string content { get; set; }
        public int width { get; set; }
        public bool scrolling { get; set; }
        public string media_domain_url { get; set; }
        public int height { get; set; }
    }

    public class Oembed4
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_height { get; set; }
    }

    public class Media2
    {
        public Oembed4 oembed { get; set; }
        public string type { get; set; }
    }

    public class CrosspostParentList
    {
        public string domain { get; set; }
        public object approved_at_utc { get; set; }
        public object banned_by { get; set; }
        public MediaEmbed2 media_embed { get; set; }
        public string subreddit { get; set; }
        public object selftext_html { get; set; }
        public string selftext { get; set; }
        public object likes { get; set; }
        public object suggested_sort { get; set; }
        public List<object> user_reports { get; set; }
        public SecureMedia2 secure_media { get; set; }
        public bool is_reddit_media_domain { get; set; }
        public bool saved { get; set; }
        public string id { get; set; }
        public object banned_at_utc { get; set; }
        public object view_count { get; set; }
        public bool archived { get; set; }
        public bool clicked { get; set; }
        public object report_reasons { get; set; }
        public string title { get; set; }
        public int num_crossposts { get; set; }
        public object link_flair_text { get; set; }
        public List<object> mod_reports { get; set; }
        public bool can_mod_post { get; set; }
        public bool is_crosspostable { get; set; }
        public bool pinned { get; set; }
        public int score { get; set; }
        public object approved_by { get; set; }
        public bool over_18 { get; set; }
        public bool hidden { get; set; }
        public string thumbnail { get; set; }
        public string subreddit_id { get; set; }
        public bool edited { get; set; }
        public object link_flair_css_class { get; set; }
        public object author_flair_css_class { get; set; }
        public bool contest_mode { get; set; }
        public int gilded { get; set; }
        public int downs { get; set; }
        public bool brand_safe { get; set; }
        public SecureMediaEmbed2 secure_media_embed { get; set; }
        public object removal_reason { get; set; }
        public object author_flair_text { get; set; }
        public bool stickied { get; set; }
        public bool can_gild { get; set; }
        public bool is_self { get; set; }
        public string parent_whitelist_status { get; set; }
        public string name { get; set; }
        public bool spoiler { get; set; }
        public string permalink { get; set; }
        public string subreddit_type { get; set; }
        public bool locked { get; set; }
        public bool hide_score { get; set; }
        public double created { get; set; }
        public string url { get; set; }
        public string whitelist_status { get; set; }
        public bool quarantine { get; set; }
        public string author { get; set; }
        public double created_utc { get; set; }
        public string subreddit_name_prefixed { get; set; }
        public int ups { get; set; }
        public Media2 media { get; set; }
        public int num_comments { get; set; }
        public bool visited { get; set; }
        public object num_reports { get; set; }
        public bool is_video { get; set; }
        public object distinguished { get; set; }
    }

    public class Data2
    {
        public string domain { get; set; }
        public object approved_at_utc { get; set; }
        public object banned_by { get; set; }
        public MediaEmbed media_embed { get; set; }
        public string subreddit { get; set; }
        public string selftext_html { get; set; }
        public string selftext { get; set; }
        public object likes { get; set; }
        public object suggested_sort { get; set; }
        public List<object> user_reports { get; set; }
        public SecureMedia secure_media { get; set; }
        public bool is_reddit_media_domain { get; set; }
        public bool saved { get; set; }
        public string id { get; set; }
        public object banned_at_utc { get; set; }
        public object view_count { get; set; }
        public bool archived { get; set; }
        public bool clicked { get; set; }
        public object report_reasons { get; set; }
        public string title { get; set; }
        public int num_crossposts { get; set; }
        public string link_flair_text { get; set; }
        public List<object> mod_reports { get; set; }
        public bool can_mod_post { get; set; }
        public bool is_crosspostable { get; set; }
        public bool pinned { get; set; }
        public int score { get; set; }
        public object approved_by { get; set; }
        public bool over_18 { get; set; }
        public bool hidden { get; set; }
        public string thumbnail { get; set; }
        public string subreddit_id { get; set; }
        public object edited { get; set; }
        public string link_flair_css_class { get; set; }
        public string author_flair_css_class { get; set; }
        public bool contest_mode { get; set; }
        public int gilded { get; set; }
        public int downs { get; set; }
        public bool brand_safe { get; set; }
        public SecureMediaEmbed secure_media_embed { get; set; }
        public object removal_reason { get; set; }
        public string author_flair_text { get; set; }
        public bool stickied { get; set; }
        public bool can_gild { get; set; }
        public bool is_self { get; set; }
        public string parent_whitelist_status { get; set; }
        public string name { get; set; }
        public bool spoiler { get; set; }
        public string permalink { get; set; }
        public string subreddit_type { get; set; }
        public bool locked { get; set; }
        public bool hide_score { get; set; }
        public double created { get; set; }
        public string url { get; set; }
        public string whitelist_status { get; set; }
        public bool quarantine { get; set; }
        public string author { get; set; }
        public double created_utc { get; set; }
        public string subreddit_name_prefixed { get; set; }
        public int ups { get; set; }
        public Media media { get; set; }
        public int num_comments { get; set; }
        public bool visited { get; set; }
        public object num_reports { get; set; }
        public bool is_video { get; set; }
        public string distinguished { get; set; }
        public string crosspost_parent { get; set; }
        public List<CrosspostParentList> crosspost_parent_list { get; set; }
    }

    public class Child
    {
        public string kind { get; set; }
        public Data2 data { get; set; }
    }

    public class Data
    {
        public string modhash { get; set; }
        public string whitelist_status { get; set; }
        public List<Child> children { get; set; }
        public string after { get; set; }
        public string before { get; set; }
    }

    public class RedditTopTwenty
    {
        public string kind { get; set; }
        public Data data { get; set; }
    }
}
