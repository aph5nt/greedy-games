using Akka.Actor;
using Payment.Contracts.Commands.Transactions;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Models;
using Serilog;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Payment.Actors
{
    public class TransactionActor : UntypedActor
    {
        private readonly string _connectionString;
        private SqlConnection _sqlConnection;
        private IActorRef _balanceActor;

        public TransactionActor(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void PreStart()
        {
            _sqlConnection = new SqlConnection(_connectionString);
            _balanceActor = Context.ActorOf(Props.Create<BalanceActor>(_connectionString), "balance");
            base.PreStart();
        }
 
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case TransactionLogMessage cmd:
                    ProcessTransaction(cmd).ForEach(msg =>
                    {
                        _balanceActor.Tell(msg, Context.Self);
                        //Context.System.EventStream.Publish(msg);
                    });

                    //if (!IsDeadletter(Context))
                    //{
                    //    Context.Sender.Tell(new Response());
                    //}
                    
                    break;
                case Object _:
                    Context.Sender.Tell(new Object());
                    break;
            }
        }

        private static bool IsDeadletter(IActorContext context)
        {
            return context.Sender.Path.Elements.Count == 1 && context.Sender.Path.Elements[0].Contains("deadLetters");
        }

        private List<Balance> ProcessTransaction(TransactionLogMessage command)
        {
            if (_sqlConnection.State == ConnectionState.Closed || _sqlConnection.State == ConnectionState.Broken)
                _sqlConnection.Open();

            var results = new List<Balance>();

            var sqlCommand = new SqlCommand("dbo.MergeTransactionLog", _sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            sqlCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "TransactionLogs",
                SqlDbType = SqlDbType.Structured,
                Value = TransactionLogAsDataTable(command.Messages)
            });

            try
            {
                using (var dr = sqlCommand.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var network = (Network)dr["Network"];
                        var userName = (string)dr["UserName"];
                        var balance = (long)dr["Balance"];
                        var logEventType = (LogEventType) dr["LogEventType"];

                        results.Add(new Balance
                        {
                            Amount = balance,
                            Network = network,
                            UserName = userName,
                            LogEventType = logEventType
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Exception occured in dbo.MergeTransactionLog");
            }
            

            return results;
        }

        private DataTable TransactionLogAsDataTable(params TransactionLogDto[] transactionLogs)
        {
            var table = new DataTable("TransactionLog");
            table.Columns.Add("MessageId", typeof(string));
            table.Columns.Add("Network", typeof(int));
            table.Columns.Add("UserName", typeof(string));
            table.Columns.Add("Amount", typeof(long));
            table.Columns.Add("LogEventType", typeof(int));
            table.Columns.Add("CreatedAt", typeof(DateTime));

            foreach (var transactionLog in transactionLogs)
                table.Rows.Add(
                    transactionLog.MessageId,
                    (int)transactionLog.Network,
                    transactionLog.UserName,
                    transactionLog.Amount,
                    (int)transactionLog.LogEventType, transactionLog.CreatedAt);

            return table;
        }
    }
}
