using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using TimeTracker.Services;
using TimeTracker.ViewModels;

namespace TimeTracker.Controllers
{
    public class UserController : BaseApiController
    {
        #region Properties
        protected UserManager<ApplicationUser> UserManager;
        #endregion Properties

        #region Constructor
        public UserController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRequestUserProvider requestUserProvider,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
            : base(context, roleManager, requestUserProvider, configuration)
        {
            UserManager = userManager;
        }
        #endregion Constructor

        #region RESTful Conventions

        /// <summary>
        /// PUT: api/user
        /// </summary>
        /// <returns>Creates a new User and return it accordingly.</returns>
        [HttpPut()]
        public async Task<IActionResult> Put([FromBody]UserViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            // check if the Username/Email already exists
            ApplicationUser user = await RequestUserProvider.FindByNameAsync(model.UserName);
            if (user != null)
            {
                model.HasErrors = true;
                model.Errors.Add("Username already exists");
                return Json(model, JsonSettings);
            }

            user = await RequestUserProvider.FindByEmailAsync(model.Email);
            if (user != null)
            {
                model.HasErrors = true;
                model.Errors.Add("Email already exists");
                return Json(model, JsonSettings);
            }

            var now = DateTime.UtcNow;

            // create a new Item with the client-sent json data
            user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                DisplayName = model.DisplayName,
                CreatedDate = now,
                LastModifiedDate = now
            };
                      
            // Add the user to the Db with the choosen password
            var result = await RequestUserProvider.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the user to the 'RegisteredUser' role.
                var roleResult = await RequestUserProvider.AddToRoleAsync(user, "RegisteredUser");
                if (roleResult.Succeeded)
                {
                    // Remove Lockout and E-Mail confirmation
                    await RequestUserProvider.UpdateLockOut(user);
                    model = user.Adapt<UserViewModel>();
                }
                else
                {
                    //Populating error properties
                    model.HasErrors = true;
                    foreach (var er in roleResult.Errors)
                    {
                        model.Errors.Add(er.Description);
                    }
                }
            }

            //When it has errors during creation new user
            else
            {
                //Populating error properties
                model.HasErrors = true;
                foreach (var er in result.Errors)
                {
                    model.Errors.Add(er.Description);
                }
            }

            // return the newly-created User to the client.
            return Json(model, JsonSettings);
        }

        /// <summary>
        /// GET: api/user
        /// </summary>

        /// <returns>Return current user</returns>
        [HttpGet()]
        [Authorize(Policy = "JwtAuthorization")]
        public async Task<IActionResult> Get()
        {
            var user = await RequestUserProvider.GetUserAsync();
            return Json(user.Adapt<UserViewModel>(), JsonSettings);
        }

        #endregion RESTful Conventions
    }
}