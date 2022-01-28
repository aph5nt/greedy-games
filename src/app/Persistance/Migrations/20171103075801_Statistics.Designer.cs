﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Persistance;
using Shared.Model;
using System;

namespace Persistance.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20171103075801_Statistics")]
    partial class Statistics
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Persistance.Model.Accounts.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DepositAddress")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("Network");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Accounts");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Account");
                });

            modelBuilder.Entity("Persistance.Model.Payments.Deposit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Amount");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("IsGameAccount");

                    b.Property<int>("Network");

                    b.Property<int>("Status");

                    b.Property<long>("TransactionHeight");

                    b.Property<string>("TransactionSignature")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Deposits");
                });

            modelBuilder.Entity("Persistance.Model.Payments.TransactionLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Amount");

                    b.Property<long>("Balance");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("LogEventType");

                    b.Property<string>("MessageId")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Network");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("TransactionLogs");
                });

            modelBuilder.Entity("Persistance.Model.Payments.Withdraw", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Amount");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("Network");

                    b.Property<int>("Status");

                    b.Property<string>("ToAddress")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<long>("TransactionHeight");

                    b.Property<string>("TransactionSignature")
                        .HasMaxLength(255);

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Withdraws");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Withdraw");
                });

            modelBuilder.Entity("Persistance.Model.Statistics.MinefieldStat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Bet");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("GameId")
                        .IsRequired();

                    b.Property<long>("Loss");

                    b.Property<int>("Network");

                    b.Property<string>("Size")
                        .IsRequired();

                    b.Property<int>("Turn");

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.Property<long>("Win");

                    b.HasKey("Id");

                    b.HasIndex("Type", "Network", "CreatedAt")
                        .HasName("IDX_AllUsersQuery");

                    b.HasIndex("Type", "Network", "UserName", "CreatedAt")
                        .HasName("IDX_UserQuery");

                    b.ToTable("MinefieldStats","stats");
                });

            modelBuilder.Entity("Persistance.Model.Accounts.GameAccount", b =>
                {
                    b.HasBaseType("Persistance.Model.Accounts.Account");

                    b.Property<long>("Treshold");

                    b.ToTable("GameAccount");

                    b.HasDiscriminator().HasValue("GameAccount");
                });

            modelBuilder.Entity("Persistance.Model.Accounts.UserAccount", b =>
                {
                    b.HasBaseType("Persistance.Model.Accounts.Account");

                    b.Property<bool>("IsActive");

                    b.ToTable("UserAccount");

                    b.HasDiscriminator().HasValue("UserAccount");
                });

            modelBuilder.Entity("Persistance.Model.Payments.DividendWithdraw", b =>
                {
                    b.HasBaseType("Persistance.Model.Payments.Withdraw");

                    b.Property<string>("GameName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("WithdrawType");

                    b.ToTable("DividendWithdraw");

                    b.HasDiscriminator().HasValue("DividendWithdraw");
                });

            modelBuilder.Entity("Persistance.Model.Payments.UserWithdraw", b =>
                {
                    b.HasBaseType("Persistance.Model.Payments.Withdraw");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.ToTable("UserWithdraw");

                    b.HasDiscriminator().HasValue("UserWithdraw");
                });
#pragma warning restore 612, 618
        }
    }
}
