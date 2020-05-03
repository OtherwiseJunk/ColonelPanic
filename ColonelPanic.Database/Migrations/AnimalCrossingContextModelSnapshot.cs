﻿// <auto-generated />
using System;
using ColonelPanic.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ColonelPanic.Database.Migrations
{
    [DbContext(typeof(AnimalCrossingContext))]
    partial class AnimalCrossingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DartsDiscordBots.Models.Fruit", b =>
                {
                    b.Property<int>("FruitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FruitName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TownId")
                        .HasColumnType("int");

                    b.HasKey("FruitId");

                    b.HasIndex("TownId");

                    b.ToTable("AC_Fruits");
                });

            modelBuilder.Entity("DartsDiscordBots.Models.Item", b =>
                {
                    b.Property<int>("ItemNum")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ItemName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TownId")
                        .HasColumnType("int");

                    b.HasKey("ItemNum");

                    b.HasIndex("TownId");

                    b.ToTable("AC_Wishlist_Items");
                });

            modelBuilder.Entity("DartsDiscordBots.Models.Town", b =>
                {
                    b.Property<int>("TownId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("BorderOpen")
                        .HasColumnType("bit");

                    b.Property<string>("DodoCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MayorDiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("NativeFruit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NorthernHempisphere")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TownName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TownId");

                    b.ToTable("AC_Towns");
                });

            modelBuilder.Entity("DartsDiscordBots.Modules.AnimalCrossing.Models.TurnipBuyPrice", b =>
                {
                    b.Property<int>("BuyPriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TownId")
                        .HasColumnType("int");

                    b.HasKey("BuyPriceId");

                    b.HasIndex("TownId");

                    b.ToTable("AC_BuyPrices");
                });

            modelBuilder.Entity("DartsDiscordBots.Modules.AnimalCrossing.Models.TurnipSellPrice", b =>
                {
                    b.Property<int>("SellPriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsMorningPrice")
                        .HasColumnType("bit");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TownId")
                        .HasColumnType("int");

                    b.HasKey("SellPriceId");

                    b.HasIndex("TownId");

                    b.ToTable("AC_SellPrices");
                });

            modelBuilder.Entity("DartsDiscordBots.Models.Fruit", b =>
                {
                    b.HasOne("DartsDiscordBots.Models.Town", null)
                        .WithMany("Fruits")
                        .HasForeignKey("TownId");
                });

            modelBuilder.Entity("DartsDiscordBots.Models.Item", b =>
                {
                    b.HasOne("DartsDiscordBots.Models.Town", null)
                        .WithMany("Wishlist")
                        .HasForeignKey("TownId");
                });

            modelBuilder.Entity("DartsDiscordBots.Modules.AnimalCrossing.Models.TurnipBuyPrice", b =>
                {
                    b.HasOne("DartsDiscordBots.Models.Town", null)
                        .WithMany("BuyPrices")
                        .HasForeignKey("TownId");
                });

            modelBuilder.Entity("DartsDiscordBots.Modules.AnimalCrossing.Models.TurnipSellPrice", b =>
                {
                    b.HasOne("DartsDiscordBots.Models.Town", null)
                        .WithMany("SellPrices")
                        .HasForeignKey("TownId");
                });
#pragma warning restore 612, 618
        }
    }
}
