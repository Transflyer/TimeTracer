﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TimeTracker.Data;

namespace TimeTracker.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190108153633_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("TimeTracker.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<int>("Flags");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<string>("Notes");

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TimeTracker.Data.Models.NodeElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<int>("Flags");

                    b.Property<DateTime>("LastModifiedDate");

                    b.Property<string>("Notes");

                    b.Property<int>("ParentId");

                    b.Property<string>("Text");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("UserId");

                    b.ToTable("NodeElements");
                });

            modelBuilder.Entity("TimeTracker.Data.Models.NodeElement", b =>
                {
                    b.HasOne("TimeTracker.Data.Models.NodeElement", "ParentNode")
                        .WithMany("NodeElements")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TimeTracker.Data.Models.ApplicationUser", "User")
                        .WithMany("RootElements")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
