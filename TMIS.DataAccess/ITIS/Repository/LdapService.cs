using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.SMIM.Repository;
using TMIS.Models.ITIS;

namespace TMIS.DataAccess.ITIS.Repository
{
    public class LdapService(): ILdapService
    {        
        public async Task<IEnumerable<SelectListItem>> GetEmployeesFromAD()
        {
            var employeeList = new List<SelectListItem>();
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
                return employeeList;
            }

            return await Task.Run(() =>
            {
                const int pageSize = 1000;
                var searchFilter = "(&(objectClass=user)(mail=*))";
                var attributesToLoad = new[] { "displayName", "mail" };

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

                        if (!string.IsNullOrWhiteSpace(displayName))
                        {
                            employeeList.Add(new SelectListItem
                            {
                                Value = !string.IsNullOrEmpty(mail) ? mail : displayName,
                                Text = !string.IsNullOrEmpty(mail) ? $"{displayName} - {mail}" : displayName
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
                return employeeList.OrderBy(x => x.Text);
            });
        }

    }
}
