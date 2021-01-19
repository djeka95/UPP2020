﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using lit_udr.EntityFramework;

namespace lit_udr.Migrations
{
    [DbContext(typeof(LitUdrContext))]
    [Migration("20201213200553_BoardMembersFIxV3")]
    partial class BoardMembersFIxV3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("lit_udr.EntityFramework.Model.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.NewUserData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Hash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewUserEmmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("processDefinitionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("processInstanceId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NewUserData");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserRetryCount")
                        .HasColumnType("int");

                    b.Property<bool>("UserVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("Writer")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.UserGenre", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("UserGenres");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.UserReview", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WorkApplicationDataId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "WorkApplicationDataId");

                    b.HasIndex("WorkApplicationDataId");

                    b.ToTable("UserReview");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.WorkApplicationData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("BoardMembeNeedsMoreData")
                        .HasColumnType("bit");

                    b.Property<bool>("BoardMembersDeclined")
                        .HasColumnType("bit");

                    b.Property<string>("processDefinitionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("processInstanceId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("workApplicationData");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.UserGenre", b =>
                {
                    b.HasOne("lit_udr.EntityFramework.Model.Genre", "Genre")
                        .WithMany("UserGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("lit_udr.EntityFramework.Model.User", "User")
                        .WithMany("UserGenres")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("User");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.UserReview", b =>
                {
                    b.HasOne("lit_udr.EntityFramework.Model.User", "User")
                        .WithMany("WorkApplicationData")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("lit_udr.EntityFramework.Model.WorkApplicationData", "WorkApplicationData")
                        .WithMany("BoardMembers")
                        .HasForeignKey("WorkApplicationDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WorkApplicationData");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.Genre", b =>
                {
                    b.Navigation("UserGenres");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.User", b =>
                {
                    b.Navigation("UserGenres");

                    b.Navigation("WorkApplicationData");
                });

            modelBuilder.Entity("lit_udr.EntityFramework.Model.WorkApplicationData", b =>
                {
                    b.Navigation("BoardMembers");
                });
#pragma warning restore 612, 618
        }
    }
}
