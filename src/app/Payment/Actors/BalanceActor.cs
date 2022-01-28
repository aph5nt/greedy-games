using Akka.Actor;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Events.Balances;
using Payment.Contracts.Models;
using Shared.Contracts;
using Shared.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Payment.Actors
{
    public class BalanceActor : ReceiveActor
    {
        private readonly string _connectionString;
        private SqlConnection _sqlConnection;
        private readonly Dictionary<string, Balance> _balances = new Dictionary<string, Balance>();

        public BalanceActor(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void PreStart()
        {
            _sqlConnection = new SqlConnection(_connectionString);

            Initialize();
 
            base.PreStart();
        }

        public void Initialized()
        {
            Receive<BalanceCommand>(command =>
            {
                HandleBalanceValidation(command);
            });

            Receive<Balance>(data =>
            {
                if (_balances.ContainsKey(Key(data.Network, data.UserName)))
                {
                    _balances[Key(data.Network, data.UserName)] = data;
                }
                else
                {
                    _balances.Add(Key(data.Network, data.UserName), data);
                }

                Context.System.EventStream.Publish(data);
            });

            Receive<GetBalance>(commad =>
            {
                var key = Key(commad.Network, commad.UserName);
                if (_balances.ContainsKey(key))
                {
                    var balance = _balances[key];
                    Context.Sender.Forward(balance);
                }
                else
                {
                    Context.Sender.Tell(new Balance
                    {
                        Network = commad.Network,
                        UserName = commad.UserName,
                        Amount = 0
                    });
                }
            });
        }

        private void HandleBalanceValidation(BalanceCommand command)
        {
            var key = Key(command.Network, command.UserName);
            
            if (_balances.ContainsKey(key))
            {
                if (_balances[key].Amount >= command.Amount + command.Fee)
                {
                    command.Target.Forward(new BalanceVerified(command.Network, command.UserName, command.Payload));
                }
                else
                {
                    Context.Sender.Tell(new Response("No funds."));
                }
            }
            else
            {
                Context.Sender.Tell(new Response("Balance not present."));
            }
        }

        private void Initialize()
        {
            if (_sqlConnection.State == ConnectionState.Closed || _sqlConnection.State == ConnectionState.Broken)
                _sqlConnection.Open();

            var sqlCommand = new SqlCommand("dbo.LoadTransactionBalances", _sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var buffer = new List<Balance>();
            
            using (var dr = sqlCommand.ExecuteReader())
            {
                while (dr.Read())
                {
                    var network = (Network)dr["Network"];
                    var userName = (string)dr["UserName"];
                    var balance = (long)dr["Balance"];

                    buffer.Add(new Balance
                    {
                        Amount = balance,
                        Network = network,
                        UserName = userName
                    });
                }
            }
            
            buffer.ForEach(balance => Context.Self.Tell(balance));
            
            Become(Initialized);
        }

        private string Key(Network network, string userName)
        {
            return $"{network}.{userName}";
        }
    }
}