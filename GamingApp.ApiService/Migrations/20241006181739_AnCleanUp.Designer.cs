﻿// <auto-generated />
using System;
using GamingApp.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GamingApp.ApiService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241006181739_AnCleanUp")]
    partial class AnCleanUp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.1.24451.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AchievementUser", b =>
                {
                    b.Property<int>("AchievementsUnlockedId")
                        .HasColumnType("integer");

                    b.Property<int>("UnlockedById")
                        .HasColumnType("integer");

                    b.HasKey("AchievementsUnlockedId", "UnlockedById");

                    b.HasIndex("UnlockedById");

                    b.ToTable("UserAchievements", (string)null);
                });

            modelBuilder.Entity("GameUser", b =>
                {
                    b.Property<int>("PlayedGamesId")
                        .HasColumnType("integer");

                    b.Property<int>("PlayersId")
                        .HasColumnType("integer");

                    b.HasKey("PlayedGamesId", "PlayersId");

                    b.HasIndex("PlayersId");

                    b.ToTable("UserGames", (string)null);
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UnlockedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Achievements", (string)null);
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int>("GameCount")
                        .HasColumnType("integer");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Developer")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("GenreId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("GenreId");

                    b.ToTable("Games", (string)null);
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.GameSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("GameSessions", (string)null);
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Bio")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("FavoriteGame")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("IdentityServerSid")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("InGameUserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("AchievementUser", b =>
                {
                    b.HasOne("GamingApp.ApiService.Data.Models.Achievement", null)
                        .WithMany()
                        .HasForeignKey("AchievementsUnlockedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GamingApp.ApiService.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UnlockedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameUser", b =>
                {
                    b.HasOne("GamingApp.ApiService.Data.Models.Game", null)
                        .WithMany()
                        .HasForeignKey("PlayedGamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GamingApp.ApiService.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("PlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Category", b =>
                {
                    b.HasOne("GamingApp.ApiService.Data.Models.Category", null)
                        .WithMany("Categories")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Game", b =>
                {
                    b.HasOne("GamingApp.ApiService.Data.Models.Category", "Genre")
                        .WithMany("Games")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.GameSession", b =>
                {
                    b.HasOne("GamingApp.ApiService.Data.Models.Game", "Game")
                        .WithMany("GameSessions")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GamingApp.ApiService.Data.Models.User", "User")
                        .WithMany("GameSessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Category", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Games");
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.Game", b =>
                {
                    b.Navigation("GameSessions");
                });

            modelBuilder.Entity("GamingApp.ApiService.Data.Models.User", b =>
                {
                    b.Navigation("GameSessions");
                });
#pragma warning restore 612, 618
        }
    }
}