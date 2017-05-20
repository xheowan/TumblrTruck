using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TumblrTruck.DB;

namespace TumblrTruck.DB.Migrations
{
    [DbContext(typeof(TumblrDbContext))]
    partial class TumblrContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("TumblrTruck.DB.Blog", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("ID");

                    b.ToTable("Blog");
                });

            modelBuilder.Entity("TumblrTruck.DB.LastActiveLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime");

                    b.Property<int>("LikedCount");

                    b.Property<byte>("Status");

                    b.Property<long>("Timestamp");

                    b.HasKey("ID");

                    b.ToTable("LastActiveLog");
                });

            modelBuilder.Entity("TumblrTruck.DB.Media", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<Guid>("MediaSetID");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte>("Status");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("ID");

                    b.HasIndex("MediaSetID");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("TumblrTruck.DB.MediaSet", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Cover")
                        .HasMaxLength(200);

                    b.Property<string>("Key")
                        .HasMaxLength(50);

                    b.Property<string>("Layout")
                        .HasMaxLength(50);

                    b.Property<byte>("Type");

                    b.HasKey("ID");

                    b.ToTable("MediaSet");
                });

            modelBuilder.Entity("TumblrTruck.DB.Post", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BlogName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime");

                    b.Property<Guid>("MediaSetID");

                    b.Property<string>("Slug")
                        .HasMaxLength(500);

                    b.Property<long>("SourceID");

                    b.Property<string>("SourceName");

                    b.Property<string>("SourceUrl")
                        .HasMaxLength(50);

                    b.Property<long>("Timestamp");

                    b.Property<byte>("Type");

                    b.HasKey("ID");

                    b.HasIndex("MediaSetID");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("TumblrTruck.DB.Media", b =>
                {
                    b.HasOne("TumblrTruck.DB.MediaSet", "MediaSet")
                        .WithMany("Media")
                        .HasForeignKey("MediaSetID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TumblrTruck.DB.Post", b =>
                {
                    b.HasOne("TumblrTruck.DB.MediaSet", "MediaSet")
                        .WithMany("Posts")
                        .HasForeignKey("MediaSetID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
