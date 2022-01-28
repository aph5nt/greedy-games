//using System;
//using System.Data.SqlClient;
//using Shared.Configuration;

//namespace Shared.Infrastructure
//{
//    public class LockingObject
//    {
//        public const long DefaultLockTimeout = 25;

//        private const string SqlMutextCommandText = @"

//        declare @result int
//        exec @result =sp_getapplock @Resource=@ResourceName, @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout = @LockTimeout

//    ";

//        private LockingObject(string uniqueId, TimeSpan lockTimeout)
//        {
//            GlobalUniqueId = uniqueId;
//            LockTimeout = lockTimeout;
//        }

//        public string GlobalUniqueId { get; }
//        public TimeSpan LockTimeout { get; }

//        private SqlCommand GetMutexCommand(SqlConnection sqlConnection, SqlTransaction transaction)
//        {
//            var sqlCommand = new SqlCommand(SqlMutextCommandText, sqlConnection, transaction);
//            sqlCommand.Parameters.AddWithValue("@ResourceName", GlobalUniqueId);
//            sqlCommand.Parameters.AddWithValue("@LockTimeout", LockTimeout.TotalMilliseconds);
//            return sqlCommand;
//        }

//        public class Lock : IDisposable
//        {
//            private static readonly object SingleAccessVerifier = new object();
//            private readonly SqlConnection _sqlConnection;

//            private readonly SqlTransaction _sqlTransaction;

//            public Lock(string uniqueId)
//            {
//                var defaultLockTimeout = TimeSpan.FromSeconds(DefaultLockTimeout);

//                lock (SingleAccessVerifier)
//                {
//                    var lockingObject = new LockingObject(uniqueId, defaultLockTimeout);
//                    var connectionString = ConfigurationUtil.GetValue(CfgKey.ConnectionStringSql);
//                    _sqlConnection = new SqlConnection(connectionString);
//                    _sqlConnection.Open();
//                    _sqlTransaction = _sqlConnection.BeginTransaction();

//                    using (var command = lockingObject.GetMutexCommand(_sqlConnection, _sqlTransaction))
//                    {
//                        command.ExecuteNonQuery();
//                    }
//                }
//            }

//            public void Dispose()
//            {
//                Dispose(true);
//                GC.SuppressFinalize(this);
//            }

//            protected virtual void Dispose(bool disposing)
//            {
//                if (disposing)
//                    _sqlTransaction?.Commit();

//                _sqlTransaction?.Dispose();
//                _sqlConnection?.Dispose();
//            }

//            ~Lock()
//            {
//                Dispose(false);
//            }
//        }
//    }
//}