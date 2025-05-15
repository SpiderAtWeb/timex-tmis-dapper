using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;
using TMIS.Models.ITIS.VM;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class DeviceRepository(IDatabaseConnectionSys dbConnection,
                            ICommonList iCommonList,
                            IITISLogdb iITISLogdb): IDeviceRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly ICommonList _icommonList = iCommonList;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            string sql = @"select DeviceID, DeviceName, SerialNumber, FixedAssetCode, Location from ITIS_Devices";

            return await _dbConnection.GetConnection().QueryAsync<Device>(sql);
        }
    }
}
