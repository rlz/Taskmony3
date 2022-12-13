﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Taskmony.Data;

#nullable disable

namespace Taskmony.Migrations
{
    [DbContext(typeof(TaskmonyDbContext))]
    partial class TaskmonyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DirectionUser", b =>
                {
                    b.Property<Guid>("DirectionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MembersId")
                        .HasColumnType("uuid");

                    b.HasKey("DirectionsId", "MembersId");

                    b.HasIndex("MembersId");

                    b.ToTable("DirectionUser");
                });

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

                    b.Property<Guid?>("CreatedById")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

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

                    b.Property<Guid?>("CreatedById")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Details")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
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

                    b.Property<Guid?>("CreatedById")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Details")
                        .HasColumnType("text");

                    b.Property<Guid?>("DirectionId")
                        .HasColumnType("uuid");

                    b.Property<byte>("Generation")
                        .HasColumnType("smallint");

                    b.Property<DateTime?>("ReviewedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("DirectionId");

                    b.ToTable("Ideas");
                });

            modelBuilder.Entity("Taskmony.Models.Notification", b =>
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

                    b.Property<Guid?>("ActorId")
                        .IsRequired()
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

                    b.Property<Guid?>("NotifiableId")
                        .IsRequired()
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

                    b.Property<DateTime?>("SubscribedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid?>("UserId")
                        .IsRequired()
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

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid?>("CreatedById")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

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
                        .HasColumnType("timestamp with time zone");

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

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

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

            modelBuilder.Entity("DirectionUser", b =>
                {
                    b.HasOne("Taskmony.Models.Direction", null)
                        .WithMany()
                        .HasForeignKey("DirectionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Taskmony.Models.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Taskmony.Models.Comments.Comment", b =>
                {
                    b.HasOne("Taskmony.Models.User", "CreatedBy")
                        .WithMany("Comments")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Taskmony.Models.Direction", b =>
                {
                    b.HasOne("Taskmony.Models.User", "CreatedBy")
                        .WithMany("OwnDirections")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
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

                    b.Navigation("CreatedBy");

                    b.Navigation("Direction");
                });

            modelBuilder.Entity("Taskmony.Models.Notification", b =>
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

                    b.Navigation("Assignee");

                    b.Navigation("CreatedBy");

                    b.Navigation("Direction");
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
