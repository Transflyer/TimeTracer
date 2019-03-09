using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TimeTracker.Data.Models;

namespace TimeTracker.Services
{
    public class RequestUserProvider: IRequestUserProvider
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly UserManager<ApplicationUser> userManager;

        public RequestUserProvider(IHttpContextAccessor ContextAccessor,
            UserManager<ApplicationUser> UserManager)
        {
            contextAccessor = ContextAccessor;
            userManager = UserManager;
        }

        public string GetUserId()
        {
            return userManager.GetUserId(contextAccessor.HttpContext.User);
        }

        public async Task<ApplicationUser> GetUserAsync()
        {
            return await userManager.GetUserAsync(contextAccessor.HttpContext.User);
        }

      
    }
}
