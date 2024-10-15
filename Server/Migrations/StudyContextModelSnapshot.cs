﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Projekt.Server.Helpers;

namespace Projekt.Server.Migrations
{
    [DbContext(typeof(StudyContext))]
    partial class StudyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Projekt.Shared.Entities.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Correct")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<int?>("QuestionId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SentQuestionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AnswerId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("SentQuestionId");

                    b.ToTable("Answer");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("CategoryId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(160)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.HasKey("CourseId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<int>("ContentPoints")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CourseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(160)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("ItemId");

                    b.HasIndex("CourseId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(160)
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Open")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.HasKey("QuestionId");

                    b.HasIndex("ItemId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            Name = "Admin"
                        });
                });

            modelBuilder.Entity("Projekt.Shared.Entities.SentItem", b =>
                {
                    b.Property<int>("SentItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SentItemId");

                    b.HasIndex("ItemId");

                    b.HasIndex("UserId");

                    b.ToTable("SentItem");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.SentQuestion", b =>
                {
                    b.Property<int>("SentQuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SentItemId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SentQuestionId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("SentItemId");

                    b.ToTable("SentQuestion");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("TEXT");

                    b.Property<int?>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            CreatedDate = new DateTime(2022, 12, 23, 14, 21, 59, 572, DateTimeKind.Utc).AddTicks(4299),
                            Login = "admin",
                            Password = "$2a$11$psUkisXSiG1WxjSLV.VknugbjYq30zEF22R/0uclTy27PSp3R37uu",
                            RoleId = 1
                        },
                        new
                        {
                            UserId = 2,
                            CreatedDate = new DateTime(2022, 12, 23, 14, 21, 59, 849, DateTimeKind.Utc).AddTicks(5143),
                            Login = "student",
                            Password = "$2a$11$7QAxm1hOgF4rmDtRAo3ed.q2odc/ia6Egz7GKgC9XbuJUtbCRO.DS"
                        });
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Answer", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Question", null)
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId");

                    b.HasOne("Projekt.Shared.Entities.SentQuestion", null)
                        .WithMany("Answers")
                        .HasForeignKey("SentQuestionId");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Course", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Item", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Course", "Course")
                        .WithMany("Items")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Question", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Item", "Item")
                        .WithMany("Questions")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.SentItem", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Item", "Item")
                        .WithMany("FilledItems")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Projekt.Shared.Entities.User", "User")
                        .WithMany("FilledItems")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.SentQuestion", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Question", "Question")
                        .WithMany("SentQuestions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Projekt.Shared.Entities.SentItem", "SentItem")
                        .WithMany("SentQuestions")
                        .HasForeignKey("SentItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("SentItem");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.User", b =>
                {
                    b.HasOne("Projekt.Shared.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Course", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Item", b =>
                {
                    b.Navigation("FilledItems");

                    b.Navigation("Questions");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.Question", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("SentQuestions");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.SentItem", b =>
                {
                    b.Navigation("SentQuestions");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.SentQuestion", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("Projekt.Shared.Entities.User", b =>
                {
                    b.Navigation("FilledItems");
                });
#pragma warning restore 612, 618
        }
    }
}
