﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Taskmony.Data;

#nullable disable

namespace Taskmony.Migrations
{
    [DbContext(typeof(TaskmonyDbContext))]
    [Migration("20230203213926_RenameTaskFields")]
    partial class RenameTaskFields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Taskmony.Models.Comments.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("Comments");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Taskmony.Models.Direction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Details")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("Directions");
                });

            modelBuilder.Entity("Taskmony.Models.Idea", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Details")
                        .HasColumnType("text");

                    b.Property<Guid?>("DirectionId")
                        .HasColumnType("uuid");

                    b.Property<byte>("Generation")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("DirectionId");

                    b.ToTable("Ideas");
                });

            modelBuilder.Entity("Taskmony.Models.Membership", b =>
                {
                    b.Property<Guid>("DirectionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.HasKey("DirectionId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Memberships");
                });

            modelBuilder.Entity("Taskmony.Models.Notifications.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ActionItemId")
                        .HasColumnType("uuid");

                    b.Property<byte?>("ActionItemType")
                        .HasColumnType("smallint");

                    b.Property<byte>("ActionType")
                        .HasColumnType("smallint");

                    b.Property<Guid>("ActorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Field")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ModifiedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("NewValue")
                        .HasColumnType("text");

                    b.Property<Guid>("NotifiableId")
                        .HasColumnType("uuid");

                    b.Property<byte>("NotifiableType")
                        .HasColumnType("smallint");

                    b.Property<string>("OldValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ActorId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Taskmony.Models.Subscriptions.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Subscriptions");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Taskmony.Models.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AssigneeId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid");

                    b.Property<string>("Details")
                        .HasColumnType("text");

                    b.Property<Guid?>("DirectionId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<int?>("RepeatEvery")
                        .HasColumnType("integer");

                    b.Property<byte?>("RepeatMode")
                        .HasColumnType("smallint");

                    b.Property<DateTime?>("StartAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<byte?>("WeekDays")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("DirectionId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Taskmony.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("NotificationReadTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("PasswordHash");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Taskmony.Models.Comments.IdeaComment", b =>
                {
                    b.HasBaseType("Taskmony.Models.Comments.Comment");

                    b.Property<Guid>("IdeaId")
                        .HasColumnType("uuid");

                    b.HasIndex("IdeaId");

                    b.ToTable("IdeaComments");
                });

            modelBuilder.Entity("Taskmony.Models.Comments.TaskComment", b =>
                {
                    b.HasBaseType("Taskmony.Models.Comments.Comment");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskComments");
                });

            modelBuilder.Entity("Taskmony.Models.Subscriptions.IdeaSubscription", b =>
                {
                    b.HasBaseType("Taskmony.Models.Subscriptions.Subscription");

                    b.Property<Guid>("IdeaId")
                        .HasColumnType("uuid");

                    b.HasIndex("IdeaId");

                    b.ToTable("IdeaSubscriptions");
                });

            modelBuilder.Entity("Taskmony.Models.Subscriptions.TaskSubscription", b =>
                {
                    b.HasBaseType("Taskmony.Models.Subscriptions.Subscription");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskSubscriptions");
                });

            modelBuilder.Entity("Taskmony.Models.Comments.Comment", b =>
                {
                    b.HasOne("Taskmony.Models.User", "CreatedBy")
                        .WithMany("Comments")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Taskmony.ValueObjects.DeletedAt", "DeletedAt", b1 =>
                        {
                            b1.Property<Guid>("CommentId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("DeletedAt");

                            b1.HasKey("CommentId");

                            b1.ToTable("Comments");

                            b1.WithOwner()
                                .HasForeignKey("CommentId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.CommentText", "Text", b1 =>
                        {
                            b1.Property<Guid>("CommentId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Text");

                            b1.HasKey("CommentId");

                            b1.ToTable("Comments");

                            b1.WithOwner()
                                .HasForeignKey("CommentId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedAt");

                    b.Navigation("Text")
                        .IsRequired();
                });

            modelBuilder.Entity("Taskmony.Models.Direction", b =>
                {
                    b.HasOne("Taskmony.Models.User", "CreatedBy")
                        .WithMany("OwnDirections")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Taskmony.ValueObjects.DeletedAt", "DeletedAt", b1 =>
                        {
                            b1.Property<Guid>("DirectionId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("DeletedAt");

                            b1.HasKey("DirectionId");

                            b1.ToTable("Directions");

                            b1.WithOwner()
                                .HasForeignKey("DirectionId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.DirectionName", "Name", b1 =>
                        {
                            b1.Property<Guid>("DirectionId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Name");

                            b1.HasKey("DirectionId");

                            b1.ToTable("Directions");

                            b1.WithOwner()
                                .HasForeignKey("DirectionId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedAt");

                    b.Navigation("Name")
                        .IsRequired();
                });

            modelBuilder.Entity("Taskmony.Models.Idea", b =>
                {
                    b.HasOne("Taskmony.Models.User", "CreatedBy")
                        .WithMany("Ideas")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.Direction", "Direction")
                        .WithMany("Ideas")
                        .HasForeignKey("DirectionId");

                    b.OwnsOne("Taskmony.ValueObjects.DeletedAt", "DeletedAt", b1 =>
                        {
                            b1.Property<Guid>("IdeaId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("DeletedAt");

                            b1.HasKey("IdeaId");

                            b1.ToTable("Ideas");

                            b1.WithOwner()
                                .HasForeignKey("IdeaId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.Description", "Description", b1 =>
                        {
                            b1.Property<Guid>("IdeaId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Description");

                            b1.HasKey("IdeaId");

                            b1.ToTable("Ideas");

                            b1.WithOwner()
                                .HasForeignKey("IdeaId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.ReviewedAt", "ReviewedAt", b1 =>
                        {
                            b1.Property<Guid>("IdeaId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("ReviewedAt");

                            b1.HasKey("IdeaId");

                            b1.ToTable("Ideas");

                            b1.WithOwner()
                                .HasForeignKey("IdeaId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedAt");

                    b.Navigation("Description")
                        .IsRequired();

                    b.Navigation("Direction");

                    b.Navigation("ReviewedAt");
                });

            modelBuilder.Entity("Taskmony.Models.Membership", b =>
                {
                    b.HasOne("Taskmony.Models.Direction", null)
                        .WithMany()
                        .HasForeignKey("DirectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Taskmony.Models.Notifications.Notification", b =>
                {
                    b.HasOne("Taskmony.Models.User", "Actor")
                        .WithMany()
                        .HasForeignKey("ActorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Actor");
                });

            modelBuilder.Entity("Taskmony.Models.Subscriptions.Subscription", b =>
                {
                    b.HasOne("Taskmony.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Taskmony.Models.Task", b =>
                {
                    b.HasOne("Taskmony.Models.User", "Assignee")
                        .WithMany("AssignedTasks")
                        .HasForeignKey("AssigneeId");

                    b.HasOne("Taskmony.Models.User", "CreatedBy")
                        .WithMany("Tasks")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.Direction", "Direction")
                        .WithMany("Tasks")
                        .HasForeignKey("DirectionId");

                    b.OwnsOne("Taskmony.ValueObjects.DeletedAt", "DeletedAt", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("DeletedAt");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.Description", "Description", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Description");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.CompletedAt", "CompletedAt", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("CompletedAt");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.RepeatUntil", "RepeatUntil", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("RepeatUntil");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.Navigation("Assignee");

                    b.Navigation("CompletedAt");

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedAt");

                    b.Navigation("Description")
                        .IsRequired();

                    b.Navigation("Direction");

                    b.Navigation("RepeatUntil");
                });

            modelBuilder.Entity("Taskmony.Models.User", b =>
                {
                    b.OwnsOne("Taskmony.ValueObjects.DisplayName", "DisplayName", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("DisplayName");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Email");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Taskmony.ValueObjects.Login", "Login", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("Login");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("DisplayName")
                        .IsRequired();

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("Login")
                        .IsRequired();
                });

            modelBuilder.Entity("Taskmony.Models.Comments.IdeaComment", b =>
                {
                    b.HasOne("Taskmony.Models.Comments.Comment", null)
                        .WithOne()
                        .HasForeignKey("Taskmony.Models.Comments.IdeaComment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.Idea", "Idea")
                        .WithMany("Comments")
                        .HasForeignKey("IdeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Idea");
                });

            modelBuilder.Entity("Taskmony.Models.Comments.TaskComment", b =>
                {
                    b.HasOne("Taskmony.Models.Comments.Comment", null)
                        .WithOne()
                        .HasForeignKey("Taskmony.Models.Comments.TaskComment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.Task", "Task")
                        .WithMany("Comments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Taskmony.Models.Subscriptions.IdeaSubscription", b =>
                {
                    b.HasOne("Taskmony.Models.Subscriptions.Subscription", null)
                        .WithOne()
                        .HasForeignKey("Taskmony.Models.Subscriptions.IdeaSubscription", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.Idea", "Idea")
                        .WithMany("Subscriptions")
                        .HasForeignKey("IdeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Idea");
                });

            modelBuilder.Entity("Taskmony.Models.Subscriptions.TaskSubscription", b =>
                {
                    b.HasOne("Taskmony.Models.Subscriptions.Subscription", null)
                        .WithOne()
                        .HasForeignKey("Taskmony.Models.Subscriptions.TaskSubscription", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.Task", "Task")
                        .WithMany("Subscriptions")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Taskmony.Models.Direction", b =>
                {
                    b.Navigation("Ideas");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("Taskmony.Models.Idea", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Taskmony.Models.Task", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("Taskmony.Models.User", b =>
                {
                    b.Navigation("AssignedTasks");

                    b.Navigation("Comments");

                    b.Navigation("Ideas");

                    b.Navigation("OwnDirections");

                    b.Navigation("Subscriptions");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
