﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using pastemyst.DbContexts;

#nullable disable

namespace pastemyst.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230320081257_UserSettings")]
    partial class UserSettings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "citext");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PasteUser", b =>
                {
                    b.Property<string>("PasteId")
                        .HasColumnType("text")
                        .HasColumnName("paste_id");

                    b.Property<string>("StarsId")
                        .HasColumnType("text")
                        .HasColumnName("stars_id");

                    b.HasKey("PasteId", "StarsId")
                        .HasName("pk_stars");

                    b.HasIndex("StarsId")
                        .HasDatabaseName("ix_stars_stars_id");

                    b.ToTable("stars", (string)null);
                });

            modelBuilder.Entity("pastemyst.Models.Image", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<byte[]>("Bytes")
                        .HasColumnType("bytea")
                        .HasColumnName("bytes");

                    b.Property<string>("ContentType")
                        .HasColumnType("text")
                        .HasColumnName("content_type");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.HasKey("Id")
                        .HasName("pk_images");

                    b.ToTable("images", (string)null);
                });

            modelBuilder.Entity("pastemyst.Models.Paste", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DeletesAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deletes_at");

                    b.Property<int>("ExpiresIn")
                        .HasColumnType("integer")
                        .HasColumnName("expires_in");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text")
                        .HasColumnName("owner_id");

                    b.Property<bool>("Pinned")
                        .HasColumnType("boolean")
                        .HasColumnName("pinned");

                    b.Property<bool>("Private")
                        .HasColumnType("boolean")
                        .HasColumnName("private");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_pastes");

                    b.HasIndex("OwnerId")
                        .HasDatabaseName("ix_pastes_owner_id");

                    b.ToTable("pastes", (string)null);
                });

            modelBuilder.Entity("pastemyst.Models.Pasty", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<string>("Language")
                        .HasColumnType("text")
                        .HasColumnName("language");

                    b.Property<string>("PasteId")
                        .HasColumnType("text")
                        .HasColumnName("paste_id");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_pasties");

                    b.HasIndex("PasteId")
                        .HasDatabaseName("ix_pasties_paste_id");

                    b.ToTable("pasties", (string)null);
                });

            modelBuilder.Entity("pastemyst.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("AvatarId")
                        .HasColumnType("text")
                        .HasColumnName("avatar_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsContributor")
                        .HasColumnType("boolean")
                        .HasColumnName("is_contributor");

                    b.Property<bool>("IsSupporter")
                        .HasColumnType("boolean")
                        .HasColumnName("is_supporter");

                    b.Property<string>("ProviderId")
                        .HasColumnType("text")
                        .HasColumnName("provider_id");

                    b.Property<string>("ProviderName")
                        .HasColumnType("text")
                        .HasColumnName("provider_name");

                    b.Property<string>("Username")
                        .HasColumnType("citext")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("AvatarId")
                        .HasDatabaseName("ix_users_avatar_id");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("ix_users_username");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("pastemyst.Models.UserSettings", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<bool>("ShowAllPastesOnProfile")
                        .HasColumnType("boolean")
                        .HasColumnName("show_all_pastes_on_profile");

                    b.HasKey("UserId")
                        .HasName("pk_user_settings");

                    b.ToTable("user_settings", (string)null);
                });

            modelBuilder.Entity("PasteUser", b =>
                {
                    b.HasOne("pastemyst.Models.Paste", null)
                        .WithMany()
                        .HasForeignKey("PasteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_stars_pastes_paste_id");

                    b.HasOne("pastemyst.Models.User", null)
                        .WithMany()
                        .HasForeignKey("StarsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_stars_users_stars_id");
                });

            modelBuilder.Entity("pastemyst.Models.Paste", b =>
                {
                    b.HasOne("pastemyst.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .HasConstraintName("fk_pastes_users_owner_id");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("pastemyst.Models.Pasty", b =>
                {
                    b.HasOne("pastemyst.Models.Paste", null)
                        .WithMany("Pasties")
                        .HasForeignKey("PasteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_pasties_pastes_paste_id");
                });

            modelBuilder.Entity("pastemyst.Models.User", b =>
                {
                    b.HasOne("pastemyst.Models.Image", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarId")
                        .HasConstraintName("fk_users_images_avatar_id");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("pastemyst.Models.UserSettings", b =>
                {
                    b.HasOne("pastemyst.Models.User", "User")
                        .WithOne("Settings")
                        .HasForeignKey("pastemyst.Models.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_settings_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("pastemyst.Models.Paste", b =>
                {
                    b.Navigation("Pasties");
                });

            modelBuilder.Entity("pastemyst.Models.User", b =>
                {
                    b.Navigation("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
