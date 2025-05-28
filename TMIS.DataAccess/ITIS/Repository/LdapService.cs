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
        //private readonly string _ldapServer = "timex.local"; // e.g., dc01.domain.local
        //private readonly string _ldapUser = "servicedesk";
        //private readonly string _ldapPassword = "T9#vLp@72k!QzM$w";
        //private readonly string _ldapBaseDn = "DC=timex.local,DC=com"; // Update to match your domain

        //public async Task<IEnumerable<SelectListItem>> GetEmployeesFromAD()
        //{
        //    var employeeList = new List<SelectListItem>();

        //    var credentials = new NetworkCredential("servicedesk", "T9#vLp@72k!QzM$w");
        //    var connection = new LdapConnection("timex.local")  // Use your actual domain controller 192.168.1.10
        //    {
        //        Credential = credentials,
        //        AuthType = AuthType.Negotiate,
        //        Timeout = new TimeSpan(0, 5, 0)
        //    };

        //    try
        //    {
        //        connection.Bind(); // Test connection
        //        Console.WriteLine("LDAP connection successful.");
        //    }
        //    catch (LdapException ex)
        //    {
        //        Console.WriteLine("LDAP bind failed: " + ex.Message);
        //        return employeeList; // Return empty if failed
        //    }

        //    return await Task.Run(() =>
        //    {
        //        try
        //        {
        //            var searchFilter = "(&(objectClass=user)(mail=*))"; // Only users with email
        //            //var searchFilter = "(&(objectClass=user))"; // Only users with email
        //            var attributesToLoad = new[] { "displayName", "mail" };
        //            //var request = new SearchRequest("DC=timex,DC=local", searchFilter, SearchScope.Subtree, attributesToLoad);
        //            var request = new SearchRequest("DC=timex,DC=local", searchFilter, SearchScope.Subtree, attributesToLoad);
        //            var response = (SearchResponse)connection.SendRequest(request);

        //            foreach (SearchResultEntry entry in response.Entries)
        //            {
        //                var email = entry.Attributes["mail"]?[0]?.ToString();
        //                var name = entry.Attributes["displayName"]?[0]?.ToString();

        //                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(name))
        //                {
        //                    employeeList.Add(new SelectListItem
        //                    {
        //                        Value = email,
        //                        Text = $"{name} - {email}"
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("LDAP search error: " + ex.Message);
        //        }

        //        return employeeList.OrderBy(e => e.Text);
        //    });
        //}

        public async Task<IEnumerable<SelectListItem>> GetEmployeesFromAD()
        {
            var employeeList = new List<SelectListItem>();
            var credentials = new NetworkCredential("servicedesk", "T9#vLp@72k!QzM$w");

            var ldapIdentifier = "timex.local";
            var baseDn = "DC=timex,DC=local";

            var connection = new LdapConnection(ldapIdentifier)
            {
                Credential = credentials,
                AuthType = AuthType.Negotiate
            };

            try
            {
                connection.Bind();
                Console.WriteLine("✅ LDAP connection successful");
            }
            catch (LdapException ex)
            {
                Console.WriteLine("❌ LDAP bind failed: " + ex.Message);
                return employeeList;
            }

            return await Task.Run(() =>
            {
                const int pageSize = 2000;
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
                    Console.WriteLine($"📄 Page {pageCount} - Entries: {response.Entries.Count}");

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

                    // Get the new cookie from the response
                    cookie = null;
                    foreach (DirectoryControl control in response.Controls)
                    {
                        if (control is PageResultResponseControl prrc)
                        {
                            cookie = prrc.Cookie;
                            Console.WriteLine($"➡️ Received cookie length: {cookie?.Length ?? 0}");
                        }
                    }

                } while (cookie != null && cookie.Length > 0);

                Console.WriteLine($"✅ AD Query complete. Total users fetched: {totalCount}");
                return employeeList.OrderBy(x => x.Text);
            });
        }

    }
}
