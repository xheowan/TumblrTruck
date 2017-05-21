using System;
using System.Net.Http;  
using System.Threading.Tasks; 
using TumblrTruck.Models;
using Newtonsoft.Json;

namespace TumblrTruck
{
    public class TumblrClient
    {
        
        private readonly TumblrConfig _config;

        public readonly string _baseApiUrl = "https://api.tumblr.com/v2/{0}";
        
        public TumblrClient(TumblrConfig config)
        {
            this._config = config;
        }

        #region core
        
        private string GetApiUrl(string path, bool apiKey = true)
        {
            return string.Format(_baseApiUrl, path) + (apiKey ? $"?api_key={_config.ApiKey}" : "");
        }
        
        /// <summary>
        /// Https the get JSON
        /// </summary>
        /// <returns>The get JSON.</returns>
        /// <param name="url">URL.</param>
        private async Task<string> HttpGetJSON(string url)
        {
            //var uri = new Uri(url);
            _debugLog($"client:get: {url}");
            
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
        
        #endregion

        /// <summary>
        /// get blog likes
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public async Task<LikedJson> GetBlogLikes(int limit = 0, int offset = 0, long before = 0, long after = 0)
        {
            var url = GetApiUrl($"blog/{_config.Hostname}/likes") + _getRequest(limit, offset, before, after);
            

            var result = await HttpGetJSON(url);
            return JsonConvert.DeserializeObject<LikedJson>(result);
        }

        /// <summary>
        /// get post by id
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<PostJson> GetPost(long postId)
        {
            var url = GetApiUrl($"blog/{_config.Hostname}/posts") + $"&id={postId}";
            
            var result = await HttpGetJSON(url);
            return JsonConvert.DeserializeObject<PostJson>(result);
        }

        private string _getRequest(int limit = 0, int offset = 0, long before = 0, long after = 0)
        {
            string parameters = "";

            parameters += limit != 0 ? $"&limit={limit}" : ""; 
            
            parameters += offset != 0 ? $"&offset={offset}" : "";
            
            parameters += before != 0 ? $"&before={before}" : "";
            
            parameters += after != 0 ? $"&after={after}" : "";

            //if (parameters.Length > 0)
            //    parameters = parameters.Remove(parameters.Length - 1, 1);
            
            return parameters;

            //var a = new { data1 = "test1", data2 = "sam", data3 = "bob" };
            //var type = a.GetType();
            //var props = type.GetProperties();
            //var pairs = props.Select(x => x.Name + "=" + x.GetValue(a, null)).ToArray();
            //var result = string.Join("&", pairs);

        }
        
        //public  async
        
        /// <summary>                         
        /// Writes the log.                   
        /// </summary>                        
        /// <param name="msg">Message.</param>
        private void _debugLog(string msg)    
        {                                     
            if (_config.ShowDebugLog)                 
                Console.WriteLine(msg);     
        }                                     
    }

    //public class ClientRequest
    //{
    //    public int limit { get; set; }
        
    //    public int offset { get; set; }
        
    //    public long before { get; set; }
        
    //    public long after { get; set; }
    //}
}