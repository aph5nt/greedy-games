using System.Data.SqlClient;
using Persistance;

namespace Web.Services.Impl
{
    public class UserIdentityService : IUserIdentityService
    {


        public string CreateUserIdentity()
        {
            using(SqlConnection connection = new SqlConnection(DataContext.GetConnectionString()))
            {
                connection.Open();
                var command = new SqlCommand("SELECT NEXT VALUE FOR dbo.UserNameSequence", connection);
                var userName = Encoder.Conceal((int)command.ExecuteScalar());
                return userName;
            }
        }
    }
}
