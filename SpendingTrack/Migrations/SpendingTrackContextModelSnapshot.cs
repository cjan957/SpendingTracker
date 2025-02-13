﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpendingTrack.Models;

namespace SpendingTrack.Migrations
{
    [DbContext(typeof(SpendingTrackContext))]
    partial class SpendingTrackContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("SpendingTrack.Models.SpendingItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Category");

                    b.Property<double>("Cost");

                    b.Property<string>("CreatedAt");

                    b.Property<string>("Currency");

                    b.Property<string>("Heading");

                    b.Property<string>("Note");

                    b.Property<string>("ReceiptID");

                    b.Property<int>("TripID");

                    b.HasKey("ID");

                    b.ToTable("SpendingItem");
                });
#pragma warning restore 612, 618
        }
    }
}
