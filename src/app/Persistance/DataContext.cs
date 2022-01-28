using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using Persistance.Model.Statistics;

namespace Persistance
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            
            var keyVault = configuration.GetSection("KeyVault");
            builder.AddAzureKeyVault(
                $"https://{keyVault["Vault"]}.vault.azure.net/",
                keyVault["ClientId"],
                keyVault["ClientSecret"], new DefaultKeyVaultSecretManager());

            configuration = builder.Build();
            
            var contextBuilder = new DbContextOptionsBuilder<DataContext>();
            contextBuilder.UseSqlServer(configuration["ConnectionString:Sql"]);
            return new DataContext(contextBuilder.Options);
        }
    }


    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<GameStatistic> GameStatistics { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MapGameAccount(modelBuilder);
            MapUserAccount(modelBuilder);
            MapDeposits(modelBuilder);
            MapUserWithdraw(modelBuilder);
            MapDividendWithdraw(modelBuilder);
            MapTransactionLogs(modelBuilder);
            MapGameResults(modelBuilder);
        }

        private static void MapGameAccount(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GameAccount>();
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DepositAddress).HasMaxLength(255).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();
            entity.Property(x => x.Treshold).IsRequired();
        }

        private static void MapUserAccount(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<UserAccount>();
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.DepositAddress).HasMaxLength(255).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();
        }

        private static void MapDeposits(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Deposit>();
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Amount).IsRequired();
            entity.Property(x => x.UpdatedAt).IsRequired();
            entity.Property(x => x.TransactionId).IsRequired().HasMaxLength(255);
            entity.Property(x => x.Status).IsRequired();
        }

        private static void MapDividendWithdraw(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<DividendWithdraw>();
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.GameName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Amount).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.ToAddress).HasMaxLength(255).IsRequired();
            entity.Property(x => x.TransactionId).HasMaxLength(255);
            entity.Property(x => x.UpdatedAt).IsRequired();
        }

        private static void MapUserWithdraw(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<UserWithdraw>();
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.Amount).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.ToAddress).HasMaxLength(255).IsRequired();
            entity.Property(x => x.TransactionId).HasMaxLength(255);
            entity.Property(x => x.UpdatedAt).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
        }

        private static void MapTransactionLogs(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<TransactionLog>();
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MessageId).IsRequired().HasMaxLength(255);
            entity.HasIndex(b => b.MessageId).IsUnique();
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Amount).IsRequired();
            entity.Property(x => x.Balance).IsRequired();
            entity.Property(x => x.LogEventType).IsRequired();
        }

        private static void MapGameResults(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GameStatistic>();
            
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.Type, x.Network, x.UserName, x.CreatedAt}).HasName("IDX_UserQuery");
            entity.HasIndex(x => new { x.Type, x.Network, x.CreatedAt }).HasName("IDX_AllUsersQuery");

            entity.Property(x => x.GameId).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Size).HasMaxLength(6).IsRequired();
            entity.Property(x => x.Network).IsRequired();
            entity.Property(x => x.UserName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Type).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.Turn).IsRequired();
            entity.Property(x => x.Bet).IsRequired();
            entity.Property(x => x.Win).IsRequired();
            entity.Property(x => x.Loss).IsRequired();
        }
    }
}
