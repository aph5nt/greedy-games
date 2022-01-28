using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Persistance.Migrations
{
    public partial class AlterMergeTransactionLogProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"

                CREATE OR ALTER PROCEDURE [dbo].[MergeTransactionLog]
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
		                Balance bigint,
										LogEventType INT
	                )

	                INSERT INTO @R1
	                SELECT tl.Id, tl.Network, tl.UserName, tl.Balance FROM (
		                SELECT MAX(Id) as Id, Network, UserName FROM dbo.TransactionLogs
		                GROUP BY Network, UserName ) as g
		                JOIN dbo.TransactionLogs tl 
		                ON tl.Id = g.Id
		                JOIN @TransactionLogs tls ON tls.Network = tl.Network AND tls.UserName = tl.UserName
 
	                INSERT INTO [dbo].[TransactionLogs] (MessageId, Network, UserName, Amount, Balance, LogEventType, CreatedAt)
	                OUTPUT INSERTED.Network, INSERTED.UserName, INSERTED.Balance, INSERTED.LogEventType INTO @O1(Network, UserName, Balance, LogEventType)
		                SELECT tl.MessageId, tl.Network, tl.UserName, tl.Amount, 
		                (COALESCE(r1.Balance, 0) + tl.Amount) as Balance,
		                tl.LogEventType, tl.CreatedAt FROM @TransactionLogs tl
		                FULL OUTER JOIN @R1 r1
		                ON tl.Network = r1.Network AND tl.UserName = r1.UserName

	                SELECT Network, UserName,Balance, LogEventType FROM @O1
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

                GO

			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
