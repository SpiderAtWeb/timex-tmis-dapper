using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.TAPS.IRepository;

namespace TMIS.DataAccess.TAPS.Repository
{
    public class AdminRepository(IDatabaseConnectionAdm dbConnection, IITISLogdb iITISLogdb) : IAdminRepository
    {
        private readonly IDatabaseConnectionAdm _dbConnection = dbConnection;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
    }
}
