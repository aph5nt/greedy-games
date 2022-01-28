using System.Data.SqlClient;
using WebApi.Configuration;

namespace WebApi.Services.Impl
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly WebSettings _settings;

        public UserIdentityService(WebSettings settings)
        {
            _settings = settings;
        }
        public string CreateUserIdentity()
        {
            using(var connection = new SqlConnection(_settings.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT NEXT VALUE FOR dbo.UserNameSequence", connection);
                var userName = Encoder.Conceal((int)command.ExecuteScalar());
                return userName;
            }
        }
    }
}
