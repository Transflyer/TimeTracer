using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TimeTracker.Services;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TimeTracker.ViewModels;

namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "JwtAuthorization")]
    public class ReportController : BaseApiController
    {
        #region Properties
        private readonly INodeElementRepository NodeElementRepo;
        #endregion

        public ReportController(ApplicationDbContext context,
           RoleManager<IdentityRole> roleManager,
           IRequestUserProvider requstUserProvider,
           IConfiguration configuration,
           INodeElementRepository elementRepo)
           : base(context, roleManager, requstUserProvider, configuration)
        {
            NodeElementRepo = elementRepo;
        }

        //Get: api/report/user/
        [HttpGet("user")]
        public async Task<IActionResult> Report(DateTime? start, DateTime? end)
        {
            var userId = RequestUserProvider.GetUserId();
            var userElements = await NodeElementRepo.UserNodeElementsWithTimeSpentsAsync(userId);

            List<ReportElement> ReportView = new List<ReportElement>();

            //Constraction NodeElement tree
            foreach (var elem in userElements)
            {
                var childList = ReportTree(elem);
                ReportView.Add(new ReportElement()
                {
                    NodeElementTitle = elem.Title,
                    Childs = childList,
                    TotalSeconds = Convert.ToInt64(elem.TimeSpents.Sum(ts => ts.TotalSecond)) 
                    + (childList == null? 0 : childList.Sum(t => t.TotalSeconds))
                });
            }
            return View();
        }

        private List<ReportElement> ReportTree(NodeElement node)
        {
            ReportElement reportElement = new ReportElement()
            {
                NodeElementTitle = node.Title
            };

            if (node.NodeElements != null)
            {
                List<ReportElement> elements = new List<ReportElement>();
                foreach (var elem in node.NodeElements)
                {
                    var e = new ReportElement();
                    e.NodeElementTitle = elem.Title;

                    var d = ReportTree(elem);
                    long childsTotalSeconds = 0;
                    if (d != null)
                    {
                        e.Childs = d;
                        childsTotalSeconds = d.Sum(ts => ts.TotalSeconds);
                    }

                    //If it have open Node Element, then we count total seconds to current time
                    var openTimeSpents = elem.TimeSpents.FirstOrDefault(i => i.IsOpen == true);
                    e.IsOpen = openTimeSpents == null ? false : true;
                    if (openTimeSpents != null)
                    {
                        openTimeSpents.End = DateTime.UtcNow;
                        openTimeSpents.TotalSecond = Convert.ToInt64((openTimeSpents.End - openTimeSpents.Start).TotalSeconds);
                    }
                    e.TotalSeconds = Convert.ToInt64(elem.TimeSpents.Sum(t => t.TotalSecond)) + childsTotalSeconds;

                    elements.Add(e);
                }
                return elements;
            }
            return null;
        }
    }
}