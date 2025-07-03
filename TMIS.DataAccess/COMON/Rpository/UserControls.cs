using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using TMIS.DataAccess.COMON.IRpository;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class UserControls(IDatabaseConnectionSys dbConnection) : IUserControls
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;

        public async Task<IEnumerable<SelectListItem>> LoadDropDownsAsync(string tableName)
        {
            string query = $@"SELECT CAST(Id AS NVARCHAR) AS Value, 
            PropName AS Text FROM {tableName} WHERE IsDelete = 0 ORDER BY Text";
            var results = await _dbConnection.GetConnection().QueryAsync<SelectListItem>(query);
            return results;
        }

        public async Task<string> GenerateRefAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string codeType)
        {
            int currentYear = DateTime.Now.Year;

            // 1. Try to get the generator for the current year
            var selectSql = @"SELECT TOP 1 [Id], [GenYear], [GenNo], [LastGeneratedDate]
            FROM [dbo]." + tableName + " WHERE GenYear = @Year AND CodeType=@CodeType";

            var generator = await connection.QuerySingleOrDefaultAsync<dynamic>(
                selectSql, new { CodeType = codeType, Year = currentYear }, transaction);

            int genNo;
            int id;

            if (generator == null)
            {
                // 2. No record for this year — insert new
                genNo = 1;

                var insertSql = @"INSERT INTO [dbo]." + tableName + " (GenYear, GenNo, LastGeneratedDate, CodeType) VALUES (@GenYear, @GenNo, GETDATE(), @CodeType ); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await connection.ExecuteScalarAsync<int>(
                    insertSql,
                    new { CodeType = codeType, GenYear = currentYear, GenNo = genNo + 1 },
                    transaction
                );
            }
            else
            {
                // 3. Record exists — increment and update
                genNo = generator.GenNo;
                id = generator.Id;

                var updateSql = @"
                UPDATE [dbo]." + tableName + " SET GenNo = @NewGenNo, LastGeneratedDate = GETDATE() WHERE Id = @Id AND CodeType=@CodeType;";

                await connection.ExecuteAsync(
                    updateSql,
                    new { CodeType = codeType, NewGenNo = genNo + 1, Id = id },
                    transaction
                );
            }

            // 4. Format final reference number
            string reference = $"{codeType}/{currentYear}/{genNo:D5}";
            return reference;
        }

        public async Task<string> GenerateSeqRefAsync(IDbConnection connection, IDbTransaction transaction, string tableName, string codeType)
        {
            // 1. Try to get the generator for the current year
            var selectSql = @"SELECT TOP 1 [Id], [GenNo], [LastGeneratedDate]
            FROM [dbo]." + tableName + " WHERE CodeType=@CodeType";

            var generator = await connection.QuerySingleOrDefaultAsync<dynamic>(
                selectSql, new { CodeType = codeType }, transaction);

            int genNo;
            int id;

            if (generator == null)
            {
                // 2. No record for this year — insert new
                genNo = 1;

                var insertSql = @"INSERT INTO [dbo]." + tableName + " (GenYear, GenNo, LastGeneratedDate, CodeType) VALUES (NULL, @GenNo, GETDATE(), @CodeType); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                await connection.ExecuteScalarAsync<int>(
                    insertSql,
                    new { CodeType = codeType, GenNo = genNo + 1 },
                    transaction
                );
            }
            else
            {
                // 3. Record exists — increment and update
                genNo = generator.GenNo;
                id = generator.Id;

                var updateSql = @"
                UPDATE [dbo]." + tableName + " SET GenNo = @NewGenNo, LastGeneratedDate = GETDATE() WHERE Id = @Id AND CodeType=@CodeType;";

                await connection.ExecuteAsync(
                    updateSql,
                    new { CodeType = codeType, NewGenNo = genNo + 1, Id = id },
                    transaction
                );
            }

            // 4. Format final reference number
            string reference = $"{codeType}{genNo:D5}";
            return reference;
        }

    }
}
