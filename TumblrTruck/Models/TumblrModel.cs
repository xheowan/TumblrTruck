using System;
using System.Collections.Generic;

namespace TumblrTruck.Models
{
    #region json model (generate from http://json2csharp.com/)
    public class Meta
    {
        public int status { get; set; }
        public string msg { get; set; }
    }

    public class Reblog
    {
        public string tree_html { get; set; }
        public string comment { get; set; }
    }

    public class Player
    {
        public int width { get; set; }
        public string embed_code { get; set; }
    }

    public class OriginalSize
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class AltSize
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Exif
    {
        public string Camera { get; set; }
        public string Aperture { get; set; }
        public string Exposure { get; set; }
    }

    public class Photo
    {
        public string caption { get; set; }
        public OriginalSize original_size { get; set; }
        public List<AltSize> alt_sizes { get; set; }

        public Exif exif { get; set; }
    }

    //public class Theme
    //{
    //    public string avatar_shape { get; set; }
    //    public string background_color { get; set; }
    //    public string body_font { get; set; }
    //    public string header_bounds { get; set; }
    //    public string header_image { get; set; }
    //    public string header_image_focused { get; set; }
    //    public string header_image_scaled { get; set; }
    //    public bool header_stretch { get; set; }
    //    public string link_color { get; set; }
    //    public bool show_avatar { get; set; }
    //    public bool show_description { get; set; }
    //    public bool show_header_image { get; set; }
    //    public bool show_title { get; set; }
    //    public string title_color { get; set; }
    //    public string title_font { get; set; }
    //    public string title_font_weight { get; set; }
    //}

    public class Trail
    {
        public TrailBlog blog { get; set; }
        public TrailPost post { get; set; }
        public string content_raw { get; set; }
        public string content { get; set; }
        public bool is_current_item { get; set; }

    }

    public class TrailBlog
    {
        public string name { get; set; }
        public bool active { get; set; }
        public object theme { get; set; }
        public bool share_likes { get; set; }
        public bool share_following { get; set; }
        public bool can_be_followed { get; set; }
    }

    public class TrailPost
    {
        public string id { get; set; }
    }

    public class LikedPost
    {
        public string type { get; set; }
        public string blog_name { get; set; }
        public long id { get; set; }
        public string post_url { get; set; }
        public string slug { get; set; }
        public string date { get; set; }
        public int timestamp { get; set; }
        public string state { get; set; }
        public string format { get; set; }
        public string reblog_key { get; set; }
        public List<object> tags { get; set; }
        public string short_url { get; set; }
        public string summary { get; set; }
        public object recommended_source { get; set; }
        public object recommended_color { get; set; }
        public int note_count { get; set; }
        public string caption { get; set; }
        public Reblog reblog { get; set; }
        public List<Trail> trail { get; set; }
        public string video_url { get; set; }
        public bool html5_capable { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_width { get; set; }
        public int thumbnail_height { get; set; }
        public double duration { get; set; }
        public List<Player> player { get; set; }
        public string video_type { get; set; }
        public int liked_timestamp { get; set; }
        public bool can_like { get; set; }
        public bool can_reblog { get; set; }
        public bool can_send_in_message { get; set; }
        public bool can_reply { get; set; }
        public bool display_avatar { get; set; }
        public string source_url { get; set; }
        public string source_title { get; set; }
        public string image_permalink { get; set; }
        public List<Photo> photos { get; set; }
        public string photoset_layout { get; set; }
    }

    public class Response
    {
        public List<LikedPost> liked_posts { get; set; }
        public int liked_count { get; set; }
    }

    public class RootObject
    {
        public Meta meta { get; set; }
        public Response response { get; set; }
    }
    #endregion
}