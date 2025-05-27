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
            string query = @"SELECT d.DeviceID, d.SerialNumber, d.FixedAssetCode, d.DeviceName, d.PurchasedDate, vt.Name as Vendor,
                               ISNULL(locAssign.LocationName, locDevice.LocationName) AS LocationName,
                               ISNULL(da.AssignLocation, d.Location) AS LocationID,
                               d.DeviceStatusID, ds.PropName AS Status, d.DeviceTypeID, dt.DeviceType
                                FROM ITIS_Devices d
                                LEFT JOIN ITIS_DeviceAssignments da ON da.DeviceID = d.DeviceID AND da.AssignStatusID IN (2, 3)
                                LEFT JOIN COMN_MasterTwoLocations locAssign ON locAssign.Id = da.AssignLocation
                                LEFT JOIN COMN_MasterTwoLocations locDevice ON locDevice.Id = d.Location
                                INNER JOIN ITIS_DeviceStatus ds ON ds.Id = d.DeviceStatusID
                                INNER JOIN ITIS_DeviceTypes dt ON dt.DeviceTypeID = d.DeviceTypeID
                                INNER JOIN ITIS_VendorTemp vt ON vt.ID = d.VendorID
                                ORDER BY LocationName;";

            var result = await _dbConnection.GetConnection().QueryAsync<SummaryReportData>(query);
            return result;

        }
    }
}
