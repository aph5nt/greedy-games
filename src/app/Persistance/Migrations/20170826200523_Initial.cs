using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Persistance.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepositAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Network = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Treshold = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deposits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsGameAccount = table.Column<bool>(type: "bit", nullable: false),
                    Network = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionHeight = table.Column<long>(type: "bigint", nullable: false),
                    TransactionSignature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Balance = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogEventType = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Network = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Withdraws",
                columns: table => new
                {
                    GameName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WithdrawType = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Network = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ToAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TransactionHeight = table.Column<long>(type: "bigint", nullable: false),
                    TransactionSignature = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdraws", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_MessageId",
                table: "TransactionLogs",
                column: "MessageId",
                unique: true);


            migrationBuilder.Sql(@"
                CREATE TYPE [dbo].[TransactionLog] AS TABLE(
	                [MessageId] [nvarchar](255) NOT NULL,
	                [Network] [int] NOT NULL,
	                [UserName] [nvarchar](50) NULL,
	                [Amount] [bigint] NOT NULL,
	                [LogEventType] [int] NOT NULL,
	                [CreatedAt] [datetime] NOT NULL
                )
                GO
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER  PROCEDURE [dbo].[MergeTransactionLog]
                (
	                @TransactionLogs AS [dbo].[TransactionLog] READONLY
                )
                AS

                BEGIN TRY
	                BEGIN TRANSACTION

	                DECLARE @R1 TABLE(
		                Id BIGINT,
		                Network INT,
		                UserName nvarchar(50),
		                Balance bigint
	                )

	                DECLARE @O1 TABLE(
		                Network INT,
		                UserName nvarchar(50),
		                Balance bigint
	                )

	                INSERT INTO @R1
	                SELECT tl.Id, tl.Network, tl.UserName, tl.Balance FROM (
		                SELECT MAX(Id) as Id, Network, UserName FROM dbo.TransactionLogs
		                GROUP BY Network, UserName ) as g
		                JOIN dbo.TransactionLogs tl 
		                ON tl.Id = g.Id
		                JOIN @TransactionLogs tls ON tls.Network = tl.Network AND tls.UserName = tl.UserName
 
	                INSERT INTO [dbo].[TransactionLogs] (MessageId, Network, UserName, Amount, Balance, LogEventType, CreatedAt)
	                OUTPUT INSERTED.Network, INSERTED.UserName, INSERTED.Balance INTO @O1(Network, UserName, Balance)
		                SELECT tl.MessageId, tl.Network, tl.UserName, tl.Amount, 
		                (COALESCE(r1.Balance, 0) + tl.Amount) as Balance,
		                tl.LogEventType, tl.CreatedAt FROM @TransactionLogs tl
		                FULL OUTER JOIN @R1 r1
		                ON tl.Network = r1.Network AND tl.UserName = r1.UserName

	                SELECT Network, UserName,Balance FROM @O1
	                COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
	                ROLLBACK TRANSACTION
	                DECLARE @ErrorMessage NVARCHAR(4000);  
                    DECLARE @ErrorSeverity INT;  
                    DECLARE @ErrorState INT;  
                    SELECT   
                        @ErrorMessage = ERROR_MESSAGE(),  
                        @ErrorSeverity = ERROR_SEVERITY(),  
                        @ErrorState = ERROR_STATE();  
                    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  
                END CATCH
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[LoadTransactionBalances]
                AS
                BEGIN
	                SELECT DISTINCT tl.Id, tl.Network, tl.UserName, tl.Balance FROM (
		                SELECT MAX(Id) as Id, Network, UserName FROM dbo.TransactionLogs
		                GROUP BY Network, UserName ) as g
	                JOIN dbo.TransactionLogs tl 
	                ON tl.Id = g.Id
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[CalculateDividend]
                (
	                @Network INT,
	                @BankUserName NVARCHAR(50),
	                @Rate BIGINT
                )
                AS
                BEGIN
	
	                DECLARE @Treshold BIGINT
	                DECLARE @BankBalance BIGINT -- GameBalance
	                DECLARE @UsersBalance BIGINT
	                DECLARE @Spendable BIGINT
	                DECLARE @Total BIGINT

	 
		                SELECT DISTINCT tl.Id, tl.Network, tl.UserName, tl.Balance 
		                INTO #Balances FROM (
			                SELECT MAX(Id) as Id, Network, UserName FROM dbo.TransactionLogs
			                WHERE Network = @Network
			                GROUP BY Network, UserName ) as g
			                JOIN dbo.TransactionLogs tl 
			                ON tl.Id = g.Id
			                WHERE tl.Network = @Network
	 
	
                    SELECT @Treshold = Treshold FROM dbo.Account WHERE UserName LIKE @BankUserName -- GDZIE NETWORK ???
	                SELECT @BankBalance = Balance FROM #Balances WHERE UserName LIKE @BankUserName
	                SELECT @Total = SUM(Balance)  FROM #Balances

	                SET @Spendable = @BankBalance - @Treshold

	                DECLARE @Dividend BIGINT = @Spendable / @Rate
	                DECLARE @Profit BIGINT = @Spendable - @Dividend

	                SELECT ISNULL(@Spendable,0) AS Spendable, ISNULL(@Profit, 0) AS Profit, ISNULL(@Dividend, 0) AS Dividend, ISNULL(@Total, 0) AS Total

                END
            ");

            migrationBuilder.Sql(@"
                CREATE SEQUENCE [dbo].[GameSequence] 
	                 AS [bigint]
	                 START WITH 1
	                 INCREMENT BY 1
	                 MINVALUE 1
	                 MAXVALUE 9223372036854775807
	                 CACHE  100 
	                GO
 
	                CREATE SEQUENCE [dbo].[UserNameSequence] 
	                 AS [int]
	                 START WITH 1
	                 INCREMENT BY 1
	                 MINVALUE 1
	                 MAXVALUE 2147483647
	                 CACHE  100 
	                GO
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE [dbo].[QRTZ_BLOB_TRIGGERS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [TRIGGER_NAME] [nvarchar](150) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
	                [BLOB_DATA] [image] NULL,
                 CONSTRAINT [PK_QRTZ_BLOB_TRIGGERS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [TRIGGER_NAME] ASC,
	                [TRIGGER_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_CALENDARS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_CALENDARS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [CALENDAR_NAME] [nvarchar](200) NOT NULL,
	                [CALENDAR] [image] NOT NULL,
                 CONSTRAINT [PK_QRTZ_CALENDARS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [CALENDAR_NAME] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_CRON_TRIGGERS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_CRON_TRIGGERS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [TRIGGER_NAME] [nvarchar](150) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
	                [CRON_EXPRESSION] [nvarchar](120) NOT NULL,
	                [TIME_ZONE_ID] [nvarchar](80) NULL,
                 CONSTRAINT [PK_QRTZ_CRON_TRIGGERS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [TRIGGER_NAME] ASC,
	                [TRIGGER_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_FIRED_TRIGGERS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_FIRED_TRIGGERS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [ENTRY_ID] [nvarchar](95) NOT NULL,
	                [TRIGGER_NAME] [nvarchar](150) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
	                [INSTANCE_NAME] [nvarchar](200) NOT NULL,
	                [FIRED_TIME] [bigint] NOT NULL,
	                [SCHED_TIME] [bigint] NOT NULL,
	                [PRIORITY] [int] NOT NULL,
	                [STATE] [nvarchar](16) NOT NULL,
	                [JOB_NAME] [nvarchar](150) NULL,
	                [JOB_GROUP] [nvarchar](150) NULL,
	                [IS_NONCONCURRENT] [bit] NULL,
	                [REQUESTS_RECOVERY] [bit] NULL,
                 CONSTRAINT [PK_QRTZ_FIRED_TRIGGERS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [ENTRY_ID] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_JOB_DETAILS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_JOB_DETAILS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [JOB_NAME] [nvarchar](150) NOT NULL,
	                [JOB_GROUP] [nvarchar](150) NOT NULL,
	                [DESCRIPTION] [nvarchar](250) NULL,
	                [JOB_CLASS_NAME] [nvarchar](250) NOT NULL,
	                [IS_DURABLE] [bit] NOT NULL,
	                [IS_NONCONCURRENT] [bit] NOT NULL,
	                [IS_UPDATE_DATA] [bit] NOT NULL,
	                [REQUESTS_RECOVERY] [bit] NOT NULL,
	                [JOB_DATA] [image] NULL,
                 CONSTRAINT [PK_QRTZ_JOB_DETAILS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [JOB_NAME] ASC,
	                [JOB_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_LOCKS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_LOCKS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [LOCK_NAME] [nvarchar](40) NOT NULL,
                 CONSTRAINT [PK_QRTZ_LOCKS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [LOCK_NAME] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_PAUSED_TRIGGER_GRPS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
                 CONSTRAINT [PK_QRTZ_PAUSED_TRIGGER_GRPS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [TRIGGER_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_SCHEDULER_STATE]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_SCHEDULER_STATE](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [INSTANCE_NAME] [nvarchar](200) NOT NULL,
	                [LAST_CHECKIN_TIME] [bigint] NOT NULL,
	                [CHECKIN_INTERVAL] [bigint] NOT NULL,
                 CONSTRAINT [PK_QRTZ_SCHEDULER_STATE] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [INSTANCE_NAME] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_SIMPLE_TRIGGERS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [TRIGGER_NAME] [nvarchar](150) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
	                [REPEAT_COUNT] [int] NOT NULL,
	                [REPEAT_INTERVAL] [bigint] NOT NULL,
	                [TIMES_TRIGGERED] [int] NOT NULL,
                 CONSTRAINT [PK_QRTZ_SIMPLE_TRIGGERS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [TRIGGER_NAME] ASC,
	                [TRIGGER_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_SIMPROP_TRIGGERS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [TRIGGER_NAME] [nvarchar](150) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
	                [STR_PROP_1] [nvarchar](512) NULL,
	                [STR_PROP_2] [nvarchar](512) NULL,
	                [STR_PROP_3] [nvarchar](512) NULL,
	                [INT_PROP_1] [int] NULL,
	                [INT_PROP_2] [int] NULL,
	                [LONG_PROP_1] [bigint] NULL,
	                [LONG_PROP_2] [bigint] NULL,
	                [DEC_PROP_1] [numeric](13, 4) NULL,
	                [DEC_PROP_2] [numeric](13, 4) NULL,
	                [BOOL_PROP_1] [bit] NULL,
	                [BOOL_PROP_2] [bit] NULL,
                 CONSTRAINT [PK_QRTZ_SIMPROP_TRIGGERS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [TRIGGER_NAME] ASC,
	                [TRIGGER_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]

                GO
                /****** Object:  Table [dbo].[QRTZ_TRIGGERS]    Script Date: 29.12.2016 08:20:29 ******/
                SET ANSI_NULLS ON
                GO
                SET QUOTED_IDENTIFIER ON
                GO
                CREATE TABLE [dbo].[QRTZ_TRIGGERS](
	                [SCHED_NAME] [nvarchar](100) NOT NULL,
	                [TRIGGER_NAME] [nvarchar](150) NOT NULL,
	                [TRIGGER_GROUP] [nvarchar](150) NOT NULL,
	                [JOB_NAME] [nvarchar](150) NOT NULL,
	                [JOB_GROUP] [nvarchar](150) NOT NULL,
	                [DESCRIPTION] [nvarchar](250) NULL,
	                [NEXT_FIRE_TIME] [bigint] NULL,
	                [PREV_FIRE_TIME] [bigint] NULL,
	                [PRIORITY] [int] NULL,
	                [TRIGGER_STATE] [nvarchar](16) NOT NULL,
	                [TRIGGER_TYPE] [nvarchar](8) NOT NULL,
	                [START_TIME] [bigint] NOT NULL,
	                [END_TIME] [bigint] NULL,
	                [CALENDAR_NAME] [nvarchar](200) NULL,
	                [MISFIRE_INSTR] [int] NULL,
	                [JOB_DATA] [image] NULL,
                 CONSTRAINT [PK_QRTZ_TRIGGERS] PRIMARY KEY CLUSTERED 
                (
	                [SCHED_NAME] ASC,
	                [TRIGGER_NAME] ASC,
	                [TRIGGER_GROUP] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                GO

                CREATE TABLE [dbo].[SchemaVersions](
	                [Id] [int] IDENTITY(1,1) NOT NULL,
	                [ScriptName] [nvarchar](255) NOT NULL,
	                [Applied] [datetime] NOT NULL,
                 CONSTRAINT [PK_SchemaVersions_Id] PRIMARY KEY CLUSTERED 
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                )
");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "TransactionLogs");

            migrationBuilder.DropTable(
                name: "Withdraws");
        }
    }
}
