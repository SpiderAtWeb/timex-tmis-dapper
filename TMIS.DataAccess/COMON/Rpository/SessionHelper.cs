using log4net;
using Microsoft.AspNetCore.Http;
using System.Text;
using TMIS.DataAccess.COMON.IRpository;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class SessionHelper(IHttpContextAccessor httpContextAccessor) : ISessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(SessionHelper));

        public void SetUserSession(string userId, string shortName, string[] userRoles, int[] userLocList)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            try
            {
                // Log the action of setting the session values
                _logger.Info($"Setting user session for UserId: {userId}");
                _logger.Info($"Setting user session for ShortName: {shortName}");
                _logger.Info($"Setting user session for UserLocation: {string.Join(",", userLocList)}");
                _logger.Info($"Setting user session for UserRoles: {string.Join(",", userRoles)}");

                // Set each session value
                httpContext?.Session.SetString("UserId", userId);
                httpContext?.Session.SetString("ShortName", shortName);
                httpContext?.Session.SetString("UserLocations", string.Join(",", userLocList));
                httpContext?.Session.SetString("UserRoles", string.Join(",", userRoles));

                // Log success
                _logger.Info("User session successfully set.");
            }
            catch (Exception ex)
            {
                // Log error if any
                _logger.Error("Error setting user session.", ex);
            }
        }

        public string GetUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetString("UserId") ?? string.Empty;
        }
        public string GetShortName()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetString("ShortName") ?? string.Empty;
        }

        public string[] GetUserRolesList()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userRoles = httpContext?.Session.GetString("UserRoles");

            return userRoles?.Split(',') ?? [];
        }

        public string[] GetLocationList()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userLocList = httpContext?.Session.GetString("UserLocations");

            return userLocList?.Split(',') ?? [];
        }

        //clear session
        public void ClearSession()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext?.Session.Clear();
        }


    }
}
