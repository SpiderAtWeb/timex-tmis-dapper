using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class LdapServiceRepository(IDatabaseConnectionSys dbConnection, IITISLogdb iITISLogdb) : ILdapServiceRepository
    {
        private readonly IDatabaseConnectionSys _dbConnection = dbConnection;
        private readonly IITISLogdb _iITISLogdb = iITISLogdb;
        public async Task<bool> ButtonStatus(string buttonName)
        {            
            bool isButtonActive = false;
            const string sql = @"select RunStatus from ITIS_ButtonStatus where ButtonName=@ButtonName";

            isButtonActive = await _dbConnection.GetConnection().QueryFirstOrDefaultAsync<bool>(sql, new { ButtonName = buttonName });

            return isButtonActive;

        }

        public async Task<bool> SetButtonStatus(string buttonName, bool status)
        {
            const string sql = @"UPDATE ITIS_ButtonStatus SET RunStatus = @RunStatus WHERE ButtonName = @ButtonName";
            var parameters = new
            {
                ButtonName = buttonName,
                RunStatus = status
            };
            int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> InsertADEMPLOYEES(List<ADEMPLOYEE> employeeList)
        {
            const string deleteSql = @"DELETE FROM ITIS_MasterADEMPLOYEES";

            const string sql = @"INSERT INTO ITIS_MasterADEMPLOYEES (EmpEmail, EmpName, EmpDesignation, EmpDepartment, EmpLocation, EmpUserName)
                VALUES (@mail, @displayName, @jobTitle, @department, @office, @username)";

            try
            {
                // Clear existing records before inserting new ones
                await _dbConnection.GetConnection().ExecuteAsync(deleteSql);
                // Insert new employee records
                int rowsAffected = await _dbConnection.GetConnection().ExecuteAsync(sql, employeeList);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting employees: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> GetEmployeesFromAD()
        {
            bool updateBtnStatus = await SetButtonStatus("SYNCBTN", true);
            bool isSuccess = false;
            var employeeList = new List<ADEMPLOYEE>();
            var credentials = new NetworkCredential("servicedesk", "T9#vLp@72k!QzM$w");

            var ldapIdentifier = "timex.local";
            var baseDn = "DC=timex,DC=local";

            var connection = new LdapConnection(ldapIdentifier)
            {
                Credential = credentials,
                AuthType = AuthType.Negotiate,
                SessionOptions =
                {
                    ProtocolVersion = 3, // Ensure LDAPv3 for paging
                    ReferralChasing = ReferralChasingOptions.None
                }
            };

            try
            {
                connection.Bind();
                Console.WriteLine("LDAP connection successful");
            }
            catch (LdapException ex)
            {
                Console.WriteLine("LDAP bind failed: " + ex.Message);
               // return isSuccess;
            }

            await Task.Run(() =>
            {
                const int pageSize = 1000;                
                var searchFilter = "(&(objectClass=user)(mail=*))";
                var attributesToLoad = new[] { "displayName", "mail", "title", "department", "physicalDeliveryOfficeName", "sAMAccountName" };

                byte[]? cookie = null; // Cookie for pagination
                int pageCount = 0;
                int totalCount = 0;

                do
                {
                    var request = new SearchRequest(baseDn, searchFilter, SearchScope.Subtree, attributesToLoad);
                    var pageControl = new PageResultRequestControl(pageSize) { Cookie = cookie };
                    request.Controls.Add(pageControl);

                    var response = (SearchResponse)connection.SendRequest(request);

                    pageCount++;
                    Console.WriteLine($"Page {pageCount} - Entries: {response.Entries.Count}");

                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        var displayName = entry.Attributes["displayName"]?[0]?.ToString();
                        var mail = entry.Attributes["mail"]?[0]?.ToString();
                        var jobTitle = entry.Attributes["title"]?[0]?.ToString();
                        var department = entry.Attributes["department"]?[0]?.ToString();
                        var office = entry.Attributes["physicalDeliveryOfficeName"]?[0]?.ToString();
                        var username = entry.Attributes["sAMAccountName"]?[0]?.ToString();

                        if (!string.IsNullOrWhiteSpace(displayName))
                        {
                            employeeList.Add(new ADEMPLOYEE
                            {
                                mail = mail,
                                displayName = displayName,
                                jobTitle = jobTitle,
                                department = department,
                                office = office,
                                username = username
                            });

                            totalCount++;
                        }
                    }

                    // Extract cookie from response for the next page
                    cookie = response.Controls
                        .OfType<PageResultResponseControl>()
                        .FirstOrDefault()?.Cookie;

                    Console.WriteLine($"Received cookie length: {cookie?.Length ?? 0}");

                } while (cookie != null && cookie.Length > 0);

                Console.WriteLine($"AD Query complete. Total users fetched: {totalCount}");
                return employeeList.OrderBy(x => x.mail);
            });

            if (employeeList.Count > 0)
            {
                isSuccess = true;
            }

            bool rowAffected = await InsertADEMPLOYEES(employeeList);
            updateBtnStatus = await SetButtonStatus("SYNCBTN", false);
            return isSuccess;           
        }
    }
}
