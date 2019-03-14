using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TimeTracker.Data.Models;
using TimeTracker.Data;
using TimeTracker.Services;
using Microsoft.Extensions.Configuration;
using Mapster;
using TimeTracker.ViewModels;
using TimeTracker.ViewModels.Service;

namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "JwtAuthorization")]
    public class TimeSpentController : BaseApiController
    {
        #region Properties
        private readonly INodeElementRepository NodeElementRepo;
        private readonly ITimeSpentRepository TimeSpentRepo;
        #endregion

        #region Constructor
        public TimeSpentController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRequestUserProvider requstUserProvider,
            IConfiguration configuration,
            ITimeSpentRepository timerepo,
            INodeElementRepository noderepo)
            : base(context, roleManager, requstUserProvider, configuration)
        {
            TimeSpentRepo = timerepo;
            NodeElementRepo = noderepo;
        }
        #endregion

        #region Attribute-based routing methods
        /// <summary>
        /// GET: api/timespent/element/{id}
        /// </summary>
        /// <param name="id">The Id of NodeElement which own TimeSpents </param>
        /// <returns>Array of timespents</returns>
        [HttpGet("element/{id?}")]
        public async Task<IActionResult> GetElementTimeSpan(long? id)
        {
            var result = await TimeSpentRepo.GetElementTimeSpentsAsync(id);

            //handle requests asking for non-existing TimeSpents on NodeElement
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no TimeSpents for NodeElement {0} has been found", id)
                });
            }


            //If exist open TimeSpent Element - update current total seconds 
            var openTimeSpent = result.FirstOrDefault(r => r.IsOpen == true);
            if (openTimeSpent != null)
            {
                openTimeSpent = await TimeSpentRepo.SetEndAsync(openTimeSpent.Id);
            }
            
            var timeSpan = TimeSpan.FromSeconds(Convert.ToDouble(result.Sum(s => s.TotalSecond)));

            ElementSpanViewModel viewModel = new ElementSpanViewModel()
            {
                NodeElementId = (long)id,
                Days = timeSpan.Days,
                Hours = timeSpan.Hours,
                Minutes = timeSpan.Minutes,
                Seconds = timeSpan.Seconds,
                IsOpenTimeSpentId = openTimeSpent == null? 0:openTimeSpent.Id
            };
            return new JsonResult(viewModel, JsonSettings);
        }

        [HttpPost("end/element/{elementId}/{finish?}")]
        public async Task<IActionResult> SetEnd(long? elementId, bool? finish)
        {
            if (elementId == null) return new StatusCodeResult(500);
            var nodeElement = await NodeElementRepo.GetNodeElementAsync(elementId);

            //handle requests asking for non-existing NodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", elementId)
                });
            }

            var timeSpentItem = await TimeSpentRepo.GetOpenTimeSpentAsync(nodeElement.Id);

            //handle requests asking for non-existing NodeElement
            if (timeSpentItem == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There is no TimeSpent with property IsOpen == true on NodeElement {0}", elementId)
                });
            }

            var result = await TimeSpentRepo.SetEndAsync(timeSpentItem.Id, 
                finish == null ? false : (bool)finish);

            return new JsonResult(result.Adapt<IntervalViewModel>(), JsonSettings);

        }

        [HttpPost("updateend/element/")]
        public async Task<IActionResult> UpdateEnd([FromBody] IntervalViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var interval = await TimeSpentRepo.GetTimeSpent(model.Id);
            DateTime end = interval.End.AddSeconds(
                Convert.ToDouble((model.Seconds + model.Minutes * 60 + model.Hours * 60 * 60)-interval.TotalSecond));
            var result = await TimeSpentRepo.UpdateEndAsync(model.Id, end);

            return new JsonResult(result.Adapt<IntervalViewModel>(), JsonSettings);
        }

        [HttpPost("start/element/{elementId}/{start}")]
        public async Task<IActionResult> SetStart(long? elementId, DateTime start)
        {
            if (elementId == null) return new StatusCodeResult(500);
            var nodeElement = await NodeElementRepo.GetNodeElementAsync(elementId);

            //handle requests asking for non-existing NodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", elementId)
                });
            }

            var result = await TimeSpentRepo.SetStartAsync(nodeElement.Id, start);

            //handle requests asking for non-existing NodeElement
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There is no TimeSpent with property IsOpen == true on NodeElement {0}", elementId)
                });
            }

            return new JsonResult(result.Adapt<IntervalViewModel>(), JsonSettings);
        }

        #endregion

        #region  RESTful conventions methods

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return new StatusCodeResult(500);

            var result = await TimeSpentRepo.DeleteTimeSpentAsync((long)id);

            // handle requests asking for non-existing nodeElement
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("TimeSpent {0} has not been found", id)
                });
            }

            // return an HTTP Status 200 (OK).
            return new OkResult();
        }

        /// <summary>
        ///  GET: api/timespent/{id}
        ///  Retrieves the TimeSpentViewModel object with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing TimeSpent</param>
        /// <returns>NodeElement intervals with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long? id)
        {
            if (id == null) return new StatusCodeResult(500);

            var intervals = await TimeSpentRepo.GetElementTimeSpentsAsync((long)id);

            //handle requests asking for non-existing NodeElement
            if (intervals == null)
            {
                return NotFound(new
                {
                    Error = String.Format("TimeSpent {0} has not been found", id)
                });
            }

            var viewModel  =  IntervalConverter
                .FillDHMSProp(intervals
                .OrderByDescending(d=>d.Start)
                .Adapt<IEnumerable<IntervalViewModel>>());

            // output the result in JSON format
            return new JsonResult(viewModel, JsonSettings);
        }

        // POST: api/TimeSpent
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IntervalViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);
            var itemToUpdate = model.Adapt<TimeSpent>();
            var result = await TimeSpentRepo.UpdateTimeSpentAsync(itemToUpdate);
            return new JsonResult(result.Adapt<IntervalViewModel>());
        }

        // PUT: api/timespent/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long? id)
        {
            if (id == null) return new StatusCodeResult(500);
            var result = await TimeSpentRepo.CreateTimeSpentAsync((long)id);
            return new JsonResult(result.Adapt<IntervalViewModel>(), JsonSettings);
        }
        #endregion
    }
}
