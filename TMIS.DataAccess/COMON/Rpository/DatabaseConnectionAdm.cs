using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

using TMIS.DataAccess.COMON.IRpository;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class DatabaseConnectionAdm : IDatabaseConnectionAdm
    {
        private readonly IDbConnection _connection;
        public DatabaseConnectionAdm(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AdminConnection");
            _connection = new SqlConnection(connectionString);
        }
        public IDbConnection GetConnection() => _connection;
    }
}
