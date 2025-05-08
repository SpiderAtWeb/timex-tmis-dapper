using Microsoft.AspNetCore.Http;
using System.Text;
using log4net;
using TMIS.DataAccess.COMON.IRpository;

namespace TMIS.DataAccess.COMON.Rpository
{
    public class SessionHelper(IHttpContextAccessor httpContextAccessor) : ISessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILog _logger = LogManager.GetLogger(typeof(SessionHelper));

        public void SetUserSession(string userId, string nameWi, string userRole, int[] accessPlants)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            try
            {
                // Log the action of setting the session values
                _logger.Info($"Setting user session for UserId: {userId}");
                _logger.Info($"Setting user session for NameWi: {nameWi}");
                _logger.Info($"Setting user session for UserRole: {userRole}");
                _logger.Info($"Setting user session for AccessPlants: {string.Join(",", accessPlants)}");

                // Set each session value
                httpContext?.Session.SetString("UserId", userId);
                httpContext?.Session.SetString("NameWi", nameWi);
                httpContext?.Session.SetString("UserRole", userRole);
                httpContext?.Session.SetString("AccessPlants", string.Join(",", accessPlants));

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
        public string GetUserName()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetString("NameWi") ?? string.Empty;
        }

        public string GetUserRole()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetString("UserRole") ?? string.Empty;
        }

        public string[] GetAccessPlantsArray()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var accessPlantsString = httpContext?.Session.GetString("AccessPlants");

            return accessPlantsString?.Split(',') ?? [];
        }

        //clear session
        public void ClearSession()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext?.Session.Clear();
        }
    }
}
