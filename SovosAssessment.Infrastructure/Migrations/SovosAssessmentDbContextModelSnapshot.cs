﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SovosAssessment.Infrastructure.Persistence.Context;

#nullable disable

namespace SovosAssessment.Infrastructure.Migrations
{
    [DbContext(typeof(SovosAssessmentDbContext))]
    partial class SovosAssessmentDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SovosAssessment.Domain.Entities.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ExternalInvoiceId")
                        .HasColumnType("longtext");

                    b.Property<string>("ReceiverTitle")
                        .HasColumnType("longtext");

                    b.Property<string>("SenderTitle")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Invoices", (string)null);
                });

            modelBuilder.Entity("SovosAssessment.Domain.Entities.InvoiceLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("UnitCode")
                        .HasColumnType("longtext");

                    b.Property<int>("UnitPrice")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceLines", (string)null);
                });

            modelBuilder.Entity("SovosAssessment.Domain.Entities.InvoiceLine", b =>
                {
                    b.HasOne("SovosAssessment.Domain.Entities.Invoice", "InvoiceFk")
                        .WithMany("InvoiceLines")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InvoiceFk");
                });

            modelBuilder.Entity("SovosAssessment.Domain.Entities.Invoice", b =>
                {
                    b.Navigation("InvoiceLines");
                });
#pragma warning restore 612, 618
        }
    }
}
