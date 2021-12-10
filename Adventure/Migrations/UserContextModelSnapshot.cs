﻿// <auto-generated />
using System;
using Adventure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Adventure.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("Adventure.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("TradeValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("Adventure.ResourceCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ResourceCategories");
                });

            modelBuilder.Entity("Adventure.User", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Money")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Reputation")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Adventure.UserResource", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Amount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ResourceId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ResourceId");

                    b.HasIndex("UserId");

                    b.ToTable("UserResources");
                });

            modelBuilder.Entity("Adventure.Resource", b =>
                {
                    b.HasOne("Adventure.ResourceCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Adventure.UserResource", b =>
                {
                    b.HasOne("Adventure.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Adventure.User", null)
                        .WithMany("UserResources")
                        .HasForeignKey("UserId");

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("Adventure.User", b =>
                {
                    b.Navigation("UserResources");
                });
#pragma warning restore 612, 618
        }
    }
}
