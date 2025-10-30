using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class DatabaseConnectionSys : IDatabaseConnectionSys, IDisposable
    {
        private readonly IDbConnection _connection;

        public DatabaseConnectionSys(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();
        }

        public IDbConnection GetConnection() => _connection;

        public void Dispose() => _connection.Dispose();

    }
}
