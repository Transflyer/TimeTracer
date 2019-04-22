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
using Microsoft.Extensions.Logging;
using Serilog;

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
                //Recursively call func ReportTree for each elem
                var childList = ReportTree(elem);

                //Set TimeSpan from all TimeSpent records in DB
                var ts = TimeSpan.FromSeconds(Convert.ToInt64(elem.TimeSpents.Sum(t => t.TotalSecond))
                    + (childList == null ? 0 : childList.Sum(t => t.TotalSeconds)));

                //Create report element for model to client
                ReportView.Add(new ReportElement()
                {
                    NodeElementTitle = elem.Title,
                    Children = childList,
                    TotalSeconds = Convert.ToInt64(ts.TotalSeconds),
                    Days = ts.Days,
                    Hours = ts.Hours,
                    Minutes = ts.Minutes,
                    Seconds = ts.Seconds,
                });
            }

            return new JsonResult(ReportView, JsonSettings);
        }

        private List<ReportElement> ReportTree(NodeElement node)
        {
            //Create report element for model to client
            ReportElement reportElement = new ReportElement()
            {
                NodeElementTitle = node.Title
            };

            //Verify that Node Element has some children
            if (node.NodeElements != null)
            {
                List<ReportElement> elements = new List<ReportElement>();
                foreach (var elem in node.NodeElements)
                {
                    //Create report element for model to client
                    var e = new ReportElement()
                    {
                        NodeElementTitle = elem.Title
                    };

                    //Recursively call func ReportTree for each elem
                    var d = ReportTree(elem);
                    long childrenTotalSeconds = 0;

                    //Verify that element has children
                    if (d != null)
                    {
                        e.Children = d;
                        childrenTotalSeconds = d.Sum(t => t.TotalSeconds);
                    }

                    //If it have open Node Element, then counting total seconds to current time
                    var openTimeSpents = elem.TimeSpents.FirstOrDefault(i => i.IsOpen == true);
                    e.IsOpen = openTimeSpents == null ? false : true;
                    if (openTimeSpents != null)
                    {
                        openTimeSpents.End = DateTime.UtcNow;
                        openTimeSpents.TotalSecond = Convert.ToInt64((openTimeSpents.End - openTimeSpents.Start).TotalSeconds);
                    }

                    //Fill client model properties
                    e.TotalSeconds = Convert.ToInt64(elem.TimeSpents.Sum(t => t.TotalSecond)) + childrenTotalSeconds;
                    var ts = TimeSpan.FromSeconds(e.TotalSeconds);
                    e.Days = ts.Days;
                    e.Hours = ts.Hours;
                    e.Minutes = ts.Minutes;
                    e.Seconds = ts.Seconds;

                    elements.Add(e);
                }
                return elements;
            }
            return null;
        }
    }
}