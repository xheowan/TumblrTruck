using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TumblrTruck.DB;
using TumblrTruck.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace TumblrTruck
{
    public class TumblrTruck : IDisposable
    {
        /// <summary>
        /// 是否顯示 console.write
        /// </summary>
		private bool showDebugLog = true;

        //static IConfigurationRoot _config { get; set; }
        private TumblrConfig _config { get; set; }

        
        /// <summary>
        /// base tumbrl url
        /// </summary>
        private string apiBaseUrl = "https://api.tumblr.com/v2/blog/{0}/likes?api_key={1}";

        //static string authHeader = null;

        /// <summary>
        /// Tumblr Context
        /// </summary>
        private readonly TumblrDbContext _db = null;


        private string filePath;
        //static List<Blog> tempBlogList = null;

        public TumblrTruck(TumblrDbContext db, IOptions<TumblrConfig> config)
        {
            _db = db;
            _config = config.Value;
        }
        public void Dispose()
        {
        }

        public void Run()
        {
            if (_config == null)
                throw new ArgumentException("no config");

            if (_db == null)
                throw new ArgumentException("no db context");

            filePath = "";

            Task.Run(async () =>
            {
                await scanLikedPost();
            }).Wait();
        }

        /// <summary>
        /// Https the get JSON
        /// </summary>
        /// <returns>The get JSON.</returns>
        /// <param name="url">URL.</param>
        private async Task<string> httpGetJSON(string url)
        {
            //var uri = new Uri(url);

            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Add("Authorization", authHeader);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();

                //return stringReulst;
            }

            //using(var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            //    var stringTask = await client.GetStringAsync(url);
            //    return stringTask;
            //}
        }

        private string newFileName(string id, string url)
        {
            var fileUrl = new Uri(url);
            var fileName = Path.GetFileName(fileUrl.LocalPath);

            return $"{id}_{fileName}";
        }

        /// <summary>
        /// Scans the liked post.
        /// </summary>
        /// <returns>The liked post.</returns>
        private async Task scanLikedPost()
        {
            //set url
            apiBaseUrl = string.Format(apiBaseUrl, _config.Hostname, _config.ApiKey);

            int totalCount = 0, count = 0, circleCount = 0;
            //liked_timestamp
            long likedTimestamp = 0, lastLikedTimestamp = 0;

            //create db instance
            if (_db == null)
                throw new ArgumentNullException("db instance is null");
            //_db = _db ?? new TumblrDbContext();

            try
            {
                #region get last active log
                var lastActiveLog = _db.LastActiveLog.Where(w => w.Status == 1).FirstOrDefault();
                if (lastActiveLog != null)
                {
                    //count = failActiveLog.LikedCount;
                    likedTimestamp = lastActiveLog.Timestamp;
                    _log($"start from last failed {lastActiveLog.Timestamp} ({lastActiveLog.CreateTime.ToString("yyyy/MM/dd HH:mm")})");
                }
                else
                {
                    lastActiveLog = _db.LastActiveLog.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                    if (lastActiveLog != null)
                    {
                        lastLikedTimestamp = lastActiveLog.Timestamp;
                        _log($"start from newest log {lastActiveLog.Timestamp} failed at {lastActiveLog.CreateTime.ToString("yyyy/MM/dd HH:mm")}");
                    }
                }
                #endregion

                _log("start scan liked posts");

                #region scan
                do
                {
                    circleCount++;

                    if (likedTimestamp != 0 && likedTimestamp < lastLikedTimestamp)
                    {
                        _log($"#stop! ");
                        break;
                    }

                    var apiurl = $"{apiBaseUrl}&before={likedTimestamp}";
                    _debugLog($"circle: {circleCount}, count: {count}, geturl: {apiurl}");

                    var jsonResult = await httpGetJSON(apiurl);
                    var likedPosts = JsonConvert.DeserializeObject<RootObject>(jsonResult);

                    //create active log
                    #region save active log
                    var activeLog = new LastActiveLog
                    {
                        Timestamp = likedTimestamp,
                        LikedCount = totalCount,
                        Status = 1,
                        CreateTime = DateTime.UtcNow
                    };
                    _db.LastActiveLog.Add(activeLog);
                    await _db.SaveChangesAsync();
                    #endregion

                    if (totalCount == 0)
                    {
                        totalCount = likedPosts.response.liked_count;
                        _log($"total posts: {totalCount}");
                    }

                    if (!await ReadObject(likedPosts))
                        break;

                    // if (totalCount == 0)
                    // {
                    //     lastLikedTimestamp = likedPosts.response.liked_posts.First().liked_timestamp;
                    // }

                    //count amount
                    count += likedPosts.response.liked_posts.Count;
                    likedTimestamp = likedPosts.response.liked_posts.Last().liked_timestamp;

                    //update active log
                    activeLog.Status = 2;
                    await _db.SaveChangesAsync();

                    //if (lastActiveLog != null && likedTimestamp <= lastActiveLog.LastTimestamp)
                    //{
                    //    _log($"timestamp <= lastTimestamp break;");
                    //    break;
                    //}
                } while (totalCount >= count);
                #endregion

                // #region create active log
                // if (lastLikedTimestamp != 0)
                // {
                // 	//create ActiveLog
                // 	_db.LastActiveLog.Add(new LastActiveLog
                // 	{
                // 		Timestamp = lastLikedTimestamp,
                // 		LikedCount = totalCount,
                //         Status = 1,
                // 		CreateTime = DateTime.UtcNow
                // 	});
                //     await _db.SaveChangesAsync();
                // }
                // #endregion

                _log("finish");
            }
            catch (HttpRequestException ex)
            {
                _log($"http error: {ex.Message}");
                //_log($"{ex.ToString()}");
            }
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"other error: {ex.Message}");
            //}
        }

        private string getFileName(string url)
        {
            var uri = new Uri(url);
            return Path.GetFileName(uri.LocalPath);
        }

        private string getPlayerEmbedCode(string embedCode)
        {
            //<iframe width=\"400\" height=\"288\" src=\"http://thisvid.com/embed/167700\" frameborder=\"0\" allowfullscreen webkitallowfullscreen mozallowfullscreen oallowfullscreen msallowfullscreen></iframe>
            var group = Regex.Split(embedCode, @"(src)=[""']([^""']*)[""']");
            if (group[2].IndexOf("://") != -1)
                return group[2];

            return null;
        }

        // static Guid getPhotoMediaID(string url)
        // {
        //     Guid mediaId = Guid.Empty;
        //     var path = new Uri(url).AbsolutePath;
        //     var sp = path.Split('/');
        //     if (sp.Length == 3)
        //     {
        //         Guid.TryParse(sp[1], out mediaId);
        //     }

        //     if (mediaId == Guid.Empty)
        //         mediaId = Guid.NewGuid();

        //     return mediaId;

        // 	//throw new ArgumentException("id is not guid");
        // }

        private string getMediaKey(string type, string url)
        {
            var fileName = getFileName(url);

            //var key = fileName.Replace("tumblr_", "");
            var key = fileName.Split('.')[0];
            key = key.Split('_')[1];

            if (type == "photo")
            {
                key = Regex.Split(key, @"(\w+)(o\d+)")[1];
            }

            return key;
            //if (type == "video")
            //    return Regex.Split(fileName, @"(tumblr_)(\w+)(.\w+)")[2];
            //else
            //return Regex.Split(fileName, @"(tumblr_)(\w+)(o.+\.\w+)")[2];
        }

        private byte getPostType(string type)
        {
            switch (type)
            {
                case "photo":
                    return 1;
                case "video":
                    return 2;
                default:
                    return 0;
            }
        }

        private async Task<bool> ReadObject(RootObject root)
        {
            if (root == null || root.meta.status != 200 || root.response == null || root.response.liked_posts.Count == 0)
            {
                _log("! no data available");
                return false;
            }


            foreach (var item in root.response.liked_posts)
            {
                #region get or create MediaSet
                MediaSet dbMediaSet = null;
                MediaSetFlag createFlat = MediaSetFlag.NoData;

                if (item.type == "photo")
                    createFlat = createMediaPhoto(item, ref dbMediaSet);
                else if (item.type == "video")
                    createFlat = createMediaVideo(item, ref dbMediaSet);
                else
                    continue;

                switch (createFlat)
                {
                    case MediaSetFlag.Create:
                        _db.MediaSet.Add(dbMediaSet);
                        await _db.SaveChangesAsync();
                        break;
                    case MediaSetFlag.Exists:
                        _debugLog($"media> {dbMediaSet.Key}:{(MediaType)dbMediaSet.Type} already exists");
                        break;
                    case MediaSetFlag.NoData:
                        _debugLog($"media> post:{item.id} no data!!");
                        continue;
                }
                #endregion

                //get media set id
                Guid _mediaSetID = dbMediaSet.ID;

                #region scan posts & related
                var _type = getPostType(item.type);


                var dbpost = await _db.Post.FindAsync(item.id);
                if (dbpost == null)
                {
                    dbpost = new Post
                    {
                        ID = item.id,
                        BlogName = item.blog_name,
                        Slug = item.slug,
                        Timestamp = item.timestamp,
                        CreateTime = DateTime.UtcNow,
                        Type = _type,
                        MediaSetID = _mediaSetID
                    };

                    if (!string.IsNullOrEmpty(item.source_url))
                    {
                        dbpost.SourceID = 0;
                        dbpost.SourceName = item.source_title;
                        dbpost.SourceUrl = item.source_url;
                    }

                    var trail = item.trail.FirstOrDefault(f => f.post.id == dbpost.ID.ToString());
                    if (trail != null)
                    {
                        dbpost.Content = trail.content_raw;
                    }

                    _db.Post.Add(dbpost);
                }
                else
                {
                    dbpost.Timestamp = item.timestamp;
                    dbpost.Slug = item.slug;
                }

                foreach (var trail in item.trail)
                {
                    //check id exists & only video
                    var postId = Convert.ToInt64(trail.post.id);
                    if (postId == dbpost.ID)
                        continue;

                    if (!_db.Post.Any(a => a.ID == postId))
                    {
                        var newtpost = new Post
                        {
                            ID = postId,
                            BlogName = trail.blog.name,
                            //Slug
                            //Timestamp
                            CreateTime = DateTime.UtcNow,
                            Type = _type,
                            MediaSetID = _mediaSetID,
                            Content = trail.content_raw
                        };

                        _db.Post.Add(newtpost);
                    }
                }

                await _db.SaveChangesAsync();
                #endregion
            }

            return true;
        }

        /// <summary>
        /// Reads the media photo.
        /// </summary>
        /// <param name="item">Item.</param>
        private MediaSetFlag createMediaPhoto(LikedPost item, ref MediaSet dbMediaSet)
        {
            if (item.photos.Count == 0)
                return MediaSetFlag.NoData;

            var fileUrl = item.photos.FirstOrDefault()?.original_size.url;
            var key = getMediaKey(item.type, fileUrl);

            dbMediaSet = _db.MediaSet.FirstOrDefault(s => s.Key == key);
            if (dbMediaSet == null)
            {
                dbMediaSet = new MediaSet
                {
                    ID = Guid.NewGuid(), //getPhotoMediaID(fileUrl),
                    Key = key,
                    Type = getPostType(item.type),
                    Layout = item.photoset_layout
                };
            }
            else
                return MediaSetFlag.Exists;

            foreach (var photo in item.photos)
            {
                var file = photo.original_size;
                dbMediaSet.Media.Add(new Media
                {
                    FileName = getFileName(file.url),
                    Url = file.url,
                    Size = $"{file.width}x{file.height}",
                    Status = 1
                });
            }

            _debugLog($"{item.type} {item.id} {fileUrl}");

            return MediaSetFlag.Create;
        }

        /// <summary>
        /// Reads the media video.
        /// </summary>
        /// <param name="item">LikedPost</param>
        private MediaSetFlag createMediaVideo(LikedPost item, ref MediaSet dbMediaSet)
        {
            if (item.video_type == "unknow" && string.IsNullOrEmpty(item.video_url))
            {
                var getplayer = item.player.FirstOrDefault();
                if (getplayer != null)
                {
                    item.video_url = getPlayerEmbedCode(getplayer.embed_code);
                }
            }

            if (string.IsNullOrEmpty(item.video_url))
            {
                return MediaSetFlag.NoData;
            }

            var key = getMediaKey(item.type, item.video_url);

            dbMediaSet = _db.MediaSet.FirstOrDefault(s => s.Key == key);
            if (dbMediaSet == null)
            {
                dbMediaSet = new MediaSet
                {
                    ID = Guid.NewGuid(),
                    Key = key,
                    Type = getPostType(item.type),
                    Cover = item.thumbnail_url
                };
            }
            else
                return MediaSetFlag.Exists;


            dbMediaSet.Media.Add(new Media
            {
                FileName = getFileName(item.video_url),
                Url = item.video_url,
                Size = $"{item.thumbnail_width}x{item.thumbnail_height}",
                Status = 1
            });

            _debugLog($"{item.type} {item.id} {item.video_url}");

            return MediaSetFlag.Create;
        }

        /// <summary>
        /// Download the specified url.
        /// </summary>
        /// <returns>The download.</returns>
        /// <param name="url">URL.</param>
        private async Task<bool> _download(string url)
        {
            var filename = getFileName(url);
            var savepath = Path.Combine(filePath, filename);

            var client = new HttpClient();
            var response = await client.GetAsync(url);

            using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fs = File.Open(savepath, FileMode.Create))
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                await contentStream.CopyToAsync(fs);

                return true;
            }
        }

        ///
        private Blog getDBBlog(string blogName)
        {
            var dbBlog = _db.Blog.SingleOrDefault(s => s.Name == blogName);
            if (dbBlog == null)
            {
                dbBlog = new Blog
                {
                    Name = blogName
                };
                _db.Blog.Add(dbBlog);
                _db.SaveChanges();
            }

            return dbBlog;
        }


        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="msg">Message.</param>
        private void _debugLog(string msg)
        {
            if (showDebugLog)
                Console.WriteLine(msg);
        }

        static void _log(string msg)
        {
            Console.WriteLine("#" + msg);
        }

    }
}
