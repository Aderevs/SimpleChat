﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SimpleChat.DbLogic;

#nullable disable

namespace SimpleChat.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20240712141450_ConfigureCascadeDelete")]
    partial class ConfigureCascadeDelete
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.Property<int>("ChatsConnectedToChatId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersInvitedUserId")
                        .HasColumnType("integer");

                    b.HasKey("ChatsConnectedToChatId", "UsersInvitedUserId");

                    b.HasIndex("UsersInvitedUserId");

                    b.ToTable("UsersInChats", (string)null);
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.Chat", b =>
                {
                    b.Property<int>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ChatId"));

                    b.Property<int>("HostUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ChatId");

                    b.HasIndex("HostUserId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MessageId"));

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("MessageId");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.HasOne("SimpleChat.DbLogic.Entities.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsConnectedToChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SimpleChat.DbLogic.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersInvitedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.Chat", b =>
                {
                    b.HasOne("SimpleChat.DbLogic.Entities.User", "HostUser")
                        .WithMany("ChatsCreated")
                        .HasForeignKey("HostUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HostUser");
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.Message", b =>
                {
                    b.HasOne("SimpleChat.DbLogic.Entities.Chat", "Chat")
                        .WithMany("AllMessages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SimpleChat.DbLogic.Entities.User", "User")
                        .WithMany("MessagesWrote")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.Chat", b =>
                {
                    b.Navigation("AllMessages");
                });

            modelBuilder.Entity("SimpleChat.DbLogic.Entities.User", b =>
                {
                    b.Navigation("ChatsCreated");

                    b.Navigation("MessagesWrote");
                });
#pragma warning restore 612, 618
        }
    }
}
