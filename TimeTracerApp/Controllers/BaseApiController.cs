using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using TimeTracker.Services;


namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        #region Constructor
        public BaseApiController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRequestUserProvider requestUserProvider,
            IConfiguration configuration)
        {
            // Instantiate instances through DI
            DbContext = context;
            RoleManager = roleManager;
            RequestUserProvider = requestUserProvider;
            Configuration = configuration;

            // Instantiate a single JsonSerializerSettings object
            // that can be reused multiple times.
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        #region Shared Properties
        protected ApplicationDbContext DbContext { get; private set; }
        protected RoleManager<IdentityRole> RoleManager { get; private set; }
        protected IRequestUserProvider RequestUserProvider { get; private set; }
        protected IConfiguration Configuration { get; private set; }

        protected JsonSerializerSettings JsonSettings
        {
            get; private set;
        }
        #endregion
    }
}