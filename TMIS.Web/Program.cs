using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using TMIS.DataAccess.COMON.IRpository;
using TMIS.DataAccess.COMON.Rpository;
using TMIS.DataAccess.GDRM.IRpository;
using TMIS.DataAccess.GDRM.Rpository;
using TMIS.DataAccess.ITIS.IRepository;
using TMIS.DataAccess.ITIS.Repository;
using TMIS.DataAccess.PLMS.IRpository;
using TMIS.DataAccess.PLMS.Rpository;
using TMIS.DataAccess.SMIM.IRpository;
using TMIS.DataAccess.SMIM.Repository;
using TMIS.DataAccess.TGPS.IRpository;
using TMIS.DataAccess.TGPS.Rpository;

var builder = WebApplication.CreateBuilder(args);


var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddControllersWithViews();

// Add CORS services and configure policy
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll",
      builder =>
      {
        builder.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
      });
});


//builder.Services.AddRouting(options =>
//{
//  options.LowercaseUrls = true;
//  options.LowercaseQueryStrings = true; // Optional: if you want query strings to be lowercase as well
//});

builder.Services.AddDataProtection()
       .PersistKeysToFileSystem(new DirectoryInfo(@"D:\Keys"))
       .SetApplicationName("TMIS");

// Add session services
builder.Services.AddDistributedMemoryCache(); // Use an in-memory cache


builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  options.Cookie.SameSite = SameSiteMode.Strict;
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true; // Mark the cookie as essential
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IDatabaseConnectionAdm, DatabaseConnectionAdm>();
builder.Services.AddScoped<IDatabaseConnectionSys, DatabaseConnectionSys>();

// Add services to the container.
builder.Services.AddScoped<IUserAccess, UserAccess>();
builder.Services.AddScoped<ITwoFieldsMDataAccess, TwoFieldsMDataAccess>();
builder.Services.AddScoped<IUserControls, UserControls>();

//SMIM
builder.Services.AddScoped<ICommon, Common>();
builder.Services.AddScoped<IInventory, Inventory>();
builder.Services.AddScoped<IDashBoard, Dashboard>();
builder.Services.AddScoped<ITransfers, Transfers>();
builder.Services.AddScoped<IRespond, Respond>();
builder.Services.AddScoped<IDisposal, Disposal>();
builder.Services.AddScoped<ITerminationRent, TerminationRent>();
builder.Services.AddScoped<ISMIMLogdb, SMIMLogdb>();
builder.Services.AddScoped<IPrintQR, PrintQR>();

//PLMS
builder.Services.AddScoped<IOverview, Overview>();
builder.Services.AddScoped<INewInquiry, NewInquiry>();
builder.Services.AddScoped<ISaveCriticalPathActivity, SaveCriticalPathActivity>();
builder.Services.AddScoped<ITaskCompletion, TaskCompletion>();
builder.Services.AddScoped<IFeedback, Feedback>();
builder.Services.AddScoped<INextStages, NextStages>();
builder.Services.AddScoped<ICosting, Costing>();
builder.Services.AddScoped<ISMV, SMV>();
builder.Services.AddScoped<IPLMSLogdb, PLMSLogdb>();

//ITIS
builder.Services.AddScoped<IDeviceTypeRepository, DeviceTypeRepository>();
builder.Services.AddScoped<IITISLogdb, ITISLogdb>();
builder.Services.AddScoped<IAttributeRepository, AttributeRepository>();
builder.Services.AddScoped<ICommonList, CommonList>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceUserRepository, DeviceUserRepository>();
builder.Services.AddScoped<ILdapService, LdapService>();
builder.Services.AddScoped<IApproveRepository, ApproveRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
//TGPS
builder.Services.AddScoped<IGoodsGatePass, GoodsGatePass>();
builder.Services.AddScoped<IAddressBank, AddressBank>();
builder.Services.AddScoped<IEmployeePass, EmployeePass>();
builder.Services.AddScoped<IResponse, Response>();

//GDRM
builder.Services.AddScoped<IGRGoods, GRGoods>();



builder.Services.AddScoped<ISessionHelper, SessionHelper>();

// Configure cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = "/Auth/Account/Login";
      options.LogoutPath = "/Auth/Account/Logout";
      options.AccessDeniedPath = "/Auth/Account/AccessDenied";
      options.SlidingExpiration = true;
      options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Configure authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

// Middleware to prevent caching
app.Use(async (context, next) =>
{
  context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
  context.Response.Headers.Pragma = "no-cache";
  context.Response.Headers.Expires = "0";
  await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();


// Use CORS middleware
app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
  try
  {
    if (!context.User.Identity!.IsAuthenticated && !context.Request.Path.StartsWithSegments("/Auth/Account/Login"))
    {
      context.Response.Redirect("/Auth/Account/Login");
      return;
    }

    await next();
  }
  catch (CryptographicException)
  {
    context.Response.Cookies.Delete(".AspNetCore.Session");
    context.Session.Clear();
    context.Response.Redirect("/Auth/Account/Login");
  }
});


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
