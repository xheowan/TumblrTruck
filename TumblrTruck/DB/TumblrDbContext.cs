using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TumblrTruck.DB
{
    public class TumblrDbContext : DbContext
    {
        //public TumblrDbContext()
        //{

        //}

        public TumblrDbContext(DbContextOptions<TumblrDbContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Data Source=TumblrLocalDB.db");
        //    //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=TumblrLocalDB;");
        //}

        public DbSet<LastActiveLog> LastActiveLog { get; set; }

        public DbSet<Blog> Blog { get; set; }

        public DbSet<Post> Post { get; set; }

        public DbSet<MediaSet> MediaSet { get; set; }

        public DbSet<Media> Media { get; set; }
    }

    public class LastActiveLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public long Timestamp { get; set; }

        public int LikedCount { get; set; }

        public byte Status { get; set; }

        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// blog
    /// </summary>
    public class Blog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        //public ICollection<Post> Posts { get; set; }
    }

    /// <summary>
    /// blog post
    /// </summary>
    public class Post
    {
        public long ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string BlogName { get; set; }

        [MaxLength(500)]
        public string Slug { get; set;}

        /// <summary>
        /// 1:photo, 2:video
        /// </summary>
        /// <returns></returns>
        public byte Type { get; set; }

        public string Content { get; set; }

        [Required]
        public long Timestamp { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        #region source post
        public string SourceName { get; set;}

        [MaxLength(50)]
        public string SourceUrl { get; set; }

        public long SourceID { get; set;}

        #endregion

        // [ForeignKey("BlogName")]
        // public Blog Blog { get; set;}

        public Guid MediaSetID { get; set; }

        [ForeignKey("MediaSetID")]
        public MediaSet MediaSet { get; set; }
    }

    /// <summary>
    /// media set
    /// </summary>
    public class MediaSet
    {
        public MediaSet()
        {
            Media = new HashSet<Media>();
        }

        /// <summary>
        /// from fileName url path
        /// </summary>
        /// <returns></returns>
        public Guid ID { get; set; }

        /// <summary>
        /// media set key
        /// </summary>
        [MaxLength(50)]
        public string Key { get; set; }

        /// <summary>
        /// meida type
        /// </summary>
        /// <returns></returns>
        public byte Type { get; set; }

        /// <summary>
        /// layout
        /// </summary>
        [MaxLength(50)]
        public string Layout { get; set; }

        /// <summary>
        /// cover
        /// </summary>
        [MaxLength(200)]
        public string Cover { get; set; }

        /// <summary>
        /// post.source_url
        /// </summary>
        public string SourceUrl { get; set; }
        
        /// <summary>
        /// source post id
        /// </summary>
        public long SourceID { get; set; }
        
        /// <summary>
        /// post list
        /// </summary>
        public ICollection<Post> Posts { get; set; }

        /// <summary>
        /// media list
        /// </summary>
        public ICollection<Media> Media { get; set; }
    }

    /// <summary>
    /// media
    /// </summary>
    public class Media
    {
        public Media() {}

        public Media(string fileName, string size, string url)
        {
            FileName = fileName;
            Size = size;
            Url = url;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public Guid MediaSetID { get; set; }

        /// <summary>
        /// local file name
        /// </summary>
        /// <returns></returns>
        [Required]
        [MaxLength(200)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Size { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Url { get; set;}

        /// <summary>
        /// 0: delete, 1: no-yea, 2: success
        /// </summary>
        /// <returns></returns>
        public byte Status { get; set; }

        [ForeignKey("MediaSetID")]
        public MediaSet MediaSet { get; set; }
    }
}