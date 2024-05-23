﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WinUi3Test.Datatypes.Serializing;

#nullable disable

namespace WinUi3Test.Migrations
{
    [DbContext(typeof(PasswordContext))]
    [Migration("20240522092618_MyFirstMigration")]
    partial class MyFirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WinUi3Test.Datatypes.Serializing.DbField", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Data")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<long?>("DbPasswordId")
                        .HasColumnType("bigint");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("DbPasswordId");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("WinUi3Test.Datatypes.Serializing.DbPassword", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Password")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("WinUi3Test.Datatypes.Serializing.DbTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("DbPasswordId")
                        .HasColumnType("bigint");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("DbPasswordId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("WinUi3Test.Datatypes.Serializing.DbField", b =>
                {
                    b.HasOne("WinUi3Test.Datatypes.Serializing.DbPassword", null)
                        .WithMany("Fields")
                        .HasForeignKey("DbPasswordId");
                });

            modelBuilder.Entity("WinUi3Test.Datatypes.Serializing.DbTag", b =>
                {
                    b.HasOne("WinUi3Test.Datatypes.Serializing.DbPassword", null)
                        .WithMany("Tags")
                        .HasForeignKey("DbPasswordId");
                });

            modelBuilder.Entity("WinUi3Test.Datatypes.Serializing.DbPassword", b =>
                {
                    b.Navigation("Fields");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
