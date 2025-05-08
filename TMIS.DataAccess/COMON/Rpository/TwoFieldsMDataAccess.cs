using Dapper;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.Models.SMIS;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class TwoFieldsMDataAccess : ITwoFieldsMDataAccess
    {
        private readonly IDatabaseConnectionSys _dbConnection;

        public TwoFieldsMDataAccess(IDatabaseConnectionSys dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IEnumerable<TwoFieldsMData> GetList(string tblName)
        {
            string query = "SELECT Id ,PropName, PropDesc FROM " + tblName + " WHERE IsDelete = 0";
            return _dbConnection.GetConnection().Query<TwoFieldsMData>(query);
        }

        public string[] InsertRecord(TwoFieldsMData twoFieldsMData, string tblName)
        {
            string[] array = new string[2];

            try
            {
                if (twoFieldsMData == null)
                {
                    array[0] = "0";
                    array[1] = "Record cannot be Null";
                    return array;
                }

                // Check if the record with the same PropName already exists
                string checkQuery = $"SELECT COUNT(*) FROM {tblName} WHERE PropName = @PropName";
                int existingRecordCount = _dbConnection.GetConnection().ExecuteScalar<int>(checkQuery, new { twoFieldsMData.PropName });

                if (existingRecordCount > 0)
                {
                    array[0] = "0"; // Indicating failure
                    array[1] = "Record already exists !!";
                    return array;
                }

                // Insert the new record
                string query = $"INSERT INTO {tblName} (PropName, PropDesc, DateCreate, IsDelete) VALUES (@PropName, @PropDesc, @NowDT, 0)";
                int rowsAffected = _dbConnection.GetConnection().Execute(query, new { twoFieldsMData.PropName, twoFieldsMData.PropDesc, NowDT = DateTime.Now });

                if (rowsAffected > 0)
                {
                    array[0] = "1"; // Indicating success
                    array[1] = "Inserted Successfully";
                }
            }
            catch (Exception ex)
            {
                array[0] = "0"; // Indicating failure
                array[1] = ex.Message;
            }

            return array;
        }
        public string[] UpdateRecord(TwoFieldsMData twoFieldsMData, string tblName)
        {
            string[] result = new string[2];

            // Validate the input
            if (twoFieldsMData == null)
            {
                result[0] = "0"; // Indicating failure
                result[1] = "Record cannot be null.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(twoFieldsMData.PropName))
            {
                result[0] = "0"; // Indicating failure
                result[1] = "PropName cannot be null or empty.";
                return result;
            }

            if (twoFieldsMData.Id <= 0)
            {
                result[0] = "0"; // Indicating failure
                result[1] = "Invalid Id.";
                return result;
            }

            try
            {
                string query = $"UPDATE {tblName} SET PropName = @PropName, PropDesc = @PropDesc, DateUpdate = @NowDT WHERE Id = @Id";
                int rowsAffected = _dbConnection.GetConnection().Execute(query, new { twoFieldsMData.PropName, twoFieldsMData.PropDesc, NowDT = DateTime.Now, twoFieldsMData.Id });

                if (rowsAffected > 0)
                {
                    result[0] = "1"; // Indicating success
                    result[1] = "Update successful.";
                }
                else
                {
                    result[0] = "0"; // Indicating failure
                    result[1] = "No record was updated. The Id may not exist.";
                }
            }
            catch (Exception ex)
            {
                result[0] = "0"; // Indicating failure
                result[1] = ex.Message; // Return the exception message
            }

            return result;
        }

        public bool DeleteRecord(int? id, string tblName)
        {
            try
            {
                string query = $"UPDATE {tblName} SET IsDelete = 1, DateDelete = @NowDT WHERE Id = @Id";
                int rowsAffected = _dbConnection.GetConnection().Execute(query, new { NowDT = DateTime.Now, Id = id });
                return rowsAffected > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
