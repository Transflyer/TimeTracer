using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TimeTracker.Data.Models;
using TimeTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Services
{
    public class RequestUserProvider : IRequestUserProvider
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly UserManager<ApplicationUser> userManager;
        private ApplicationDbContext context;

        public RequestUserProvider(IHttpContextAccessor ContextAccessor,
            UserManager<ApplicationUser> UserManager,
            ApplicationDbContext ctx)
        {
            contextAccessor = ContextAccessor;
            userManager = UserManager;
            context = ctx;
        }

        public string GetUserId() => userManager.GetUserId(contextAccessor.HttpContext.User);

        public async Task<ApplicationUser> GetUserAsync() => await userManager.GetUserAsync(contextAccessor.HttpContext.User);

        public async Task<ApplicationUser> FindByNameAsync(string userName) 
            => await userManager.FindByNameAsync(userName);

        public async Task<ApplicationUser> FindByEmailAsync(string email) 
            =>  await userManager.FindByEmailAsync(email);

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
            => await userManager.CreateAsync(user, password);

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
            => await userManager.AddToRoleAsync(user, role);

        public async Task UpdateLockOut(ApplicationUser user)
        {
            var dbUser = await context.Users.FindAsync(user.Id);
            dbUser.EmailConfirmed = true;
            dbUser.LockoutEnabled = false;
            await context.SaveChangesAsync();
        }
    }
}