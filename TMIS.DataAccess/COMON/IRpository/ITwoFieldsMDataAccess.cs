using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMIS.Models.SMIS;


namespace TMIS.DataAccess.COMON.IRpository
{
    public interface ITwoFieldsMDataAccess
    {
        IEnumerable<TwoFieldsMData> GetList(string tbleName);

        string[] InsertRecord(TwoFieldsMData twoFieldsMData, string tblName);

        string[] UpdateRecord(TwoFieldsMData twoFieldsMData, string tblName);

        bool DeleteRecord(int? id, string tblName);

    }
}
