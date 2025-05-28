using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class ReportRepository(IDatabaseConnectionSys dbConnection) : IReportRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<SummaryReportData>> GetAllDeviceData()
        {          
            string query = "select * from ITIS_VwSummary;";

            var result = await _dbConnection.GetConnection().QueryAsync<SummaryReportData>(query);
            return result;

        }
    }
}
