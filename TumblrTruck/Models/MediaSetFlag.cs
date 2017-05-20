
namespace TumblrTruck.Models
{
    public enum MediaSetFlag : int
    {
        /// <summary>
        /// Already exists.
        /// </summary>
        Exists = 0,

        /// <summary>
        /// Create new.
        /// </summary>
        Create = 1,

        /// <summary>
        /// no url data
        /// </summary>
        NoData = -1
    }
}
