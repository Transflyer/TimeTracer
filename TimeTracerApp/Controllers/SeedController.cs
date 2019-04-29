using System;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using TimeTracker.Services;
using TimeTracker.ViewModels;
using System.Diagnostics;

namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : BaseApiController
    {
        #region Properties
        private readonly INodeElementRepository NodeElementRepo;
        private readonly IIntervalRepository IntervalRepo;
        protected UserManager<ApplicationUser> UserManager;
        #endregion

        #region Constructor
        public SeedController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IRequestUserProvider requstUserProvider,
            IConfiguration configuration,
            INodeElementRepository erepo, 
            IIntervalRepository trepo)
            : base(context, roleManager, requstUserProvider, configuration)
        {
            NodeElementRepo = erepo;
            IntervalRepo = trepo;
            UserManager = userManager;
        }
        #endregion Constructor

        // POST: api/Seed
        [HttpPost]
        public async Task<IActionResult> CreateSeedData(int count)
        {
            string role_RegisteredUser = "RegisteredUser";

            string passForUser = "Pass4Users";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                do
                {
                    var user = new ApplicationUser()
                    {
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = $"TestUser{count}",
                        Email = $"TestUser{count}@time.com",
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    };

                    // Insert sample registered users into the Database and also assign the "Registered" role to him.
                    if (await UserManager.FindByNameAsync(user.UserName) == null)
                    {
                        await UserManager.CreateAsync(user, passForUser);
                        await UserManager.AddToRoleAsync(user,
                        role_RegisteredUser);
                        // Remove Lockout and E-Mail confirmation.
                        user.EmailConfirmed = true;
                        user.LockoutEnabled = false;
                    }
                    else
                    {
                        user = await UserManager.FindByNameAsync(user.UserName);
                    }

                    //Create User Elements(Tasks)
                    var NodeElementsCount = new Random().Next(1, 20);
                    DateTime createdDate = new DateTime(2018, 08, 08, 12, 30, 00);
                    DateTime lastModifiedDate = DateTime.UtcNow;
                    List<NodeElement> nodeElementsList = new List<NodeElement>();
                    List<Interval> intervalList = new List<Interval>();
                    do
                    {
                        var element = new NodeElement()
                        {
                            CreatedDate = createdDate,
                            LastModifiedDate = lastModifiedDate,
                            Description = "Bla-Bla-Bla",
                            Text = $"Best Item for {user.UserName}{NodeElementsCount}",
                            UserId = user.Id,
                            Title = $"Title for {user.UserName}{NodeElementsCount}"
                        };

                        nodeElementsList.Add(element);
                        NodeElementsCount--;
                    } while (NodeElementsCount > 0);
                    DbContext.NodeElements.AddRange(nodeElementsList);
                    DbContext.SaveChanges();

                    foreach (var element in nodeElementsList)
                    {
                        var intervalsCount = new Random().Next(1, 100);
                        do
                        {
                            var month = new Random().Next(1, 13);
                            var day = new Random().Next(1, 28);
                            var hour = new Random().Next(23);
                            var minutes = new Random().Next(59);
                            var seconds = new Random().Next(59);
                            var start = new DateTime(2018, month, day, hour, minutes, seconds);
                            var end = start.AddSeconds(new Random().Next(10000));
                            var span = Convert.ToInt64((end - start).TotalSeconds);

                            var interval = new Interval()
                            {
                                CreatedDate = createdDate,
                                LastModifiedDate = DateTime.UtcNow,
                                ElementId = element.Id,
                                Start = start,
                                End = end,
                                TotalSecond = span,
                                IsOpen = false,
                                UserId = user.Id
                            };

                            intervalList.Add(interval);
                            intervalsCount--;
                        } while (intervalsCount > 0);
                    }

                    DbContext.Intervals.AddRange(intervalList);
                    await DbContext.SaveChangesAsync();

                    count--;
                } while (count > 0);
            }
            catch
            {
                DbContext.SaveChanges();
            }

            Console.Clear();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
            return new OkResult();
        }

    }
}