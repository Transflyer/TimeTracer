using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TimeTracker.ViewModels;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using Mapster;
using System.Text;
using System.Collections.Generic;

namespace TimeTracker.Controllers
{
    public class UserController : BaseApiController
    {
        #region Constructor
        public UserController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
            )
            : base(context, roleManager, userManager, configuration) { }
        #endregion

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

            //Prepare ViewModel
            UserViewModel viewModel = new UserViewModel();

            // check if the Username/Email already exists
            ApplicationUser user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                viewModel.HasErrors = true;
                viewModel.Errors.Add("Username already exists");
                return Json(viewModel, JsonSettings);
            }
            user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                viewModel.HasErrors = true;
                viewModel.Errors.Add("Email already exists.");
                return Json(viewModel, JsonSettings);
            }

            var now = DateTime.Now;

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
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the user to the 'RegisteredUser' role.
                await UserManager.AddToRoleAsync(user, "RegisteredUser");

                // Remove Lockout and E-Mail confirmation
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;

                // persist the changes into the Database
                DbContext.SaveChanges();
                viewModel = user.Adapt<UserViewModel>();
            }

            //When it has errors during creation new user
            else
            {
                //Populating error properties
                viewModel.HasErrors = true;
                foreach (var er in result.Errors)
                {
                    viewModel.Errors.Add(er.Description);
                }
            }
                       
            // return the newly-created User to the client.
            return Json(viewModel, JsonSettings);
        }

        /// <summary>
        /// GET: api/user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Return current user</returns>
        [HttpGet()]
        public IActionResult Get()
        {
            return Json(User.Adapt<UserViewModel>(), JsonSettings);
        }

        #endregion
    }
}
