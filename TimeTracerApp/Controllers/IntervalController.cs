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
using Serilog;

namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "JwtAuthorization")]
    public class IntervalController : BaseApiController
    {
        #region Properties
        private readonly INodeElementRepository NodeElementRepo;
        private readonly IIntervalRepository IntervalRepo;
        #endregion

        #region Constructor
        public IntervalController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRequestUserProvider requstUserProvider,
            IConfiguration configuration,
            IIntervalRepository timerepo,
            INodeElementRepository noderepo)
            : base(context, roleManager, requstUserProvider, configuration)
        {
            IntervalRepo = timerepo;
            NodeElementRepo = noderepo;
        }
        #endregion

        #region Attribute-based routing methods

        /// <summary>
        /// GET: api/interval/element/{id}
        /// </summary>
        /// <param name="id">The Id of NodeElement which own Intervals </param>
        /// <returns>Array of intervals</returns>
        [HttpGet("element/{id?}")]
        public async Task<IActionResult> GetElementTotalInterval(long? id)
        {
            var result = await IntervalRepo.GetElementIntervalsAsync(id);

            //handle requests asking for non-existing Intervals on NodeElement
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no Intervals for NodeElement {0} has been found", id)
                });
            }


            //If exist open Interval Element - update current total seconds 
            var openInterval = result.FirstOrDefault(r => r.IsOpen == true);
            if (openInterval != null)
            {
                openInterval = await IntervalRepo.SetEndAsync(openInterval.Id);
            }
            
            var timeSpan = TimeSpan.FromSeconds(Convert.ToDouble(result.Sum(s => s.TotalSecond)));

            ElementSpanViewModel viewModel = new ElementSpanViewModel()
            {
                NodeElementId = (long)id,
                Days = timeSpan.Days,
                Hours = timeSpan.Hours + timeSpan.Days * 24,
                Minutes = timeSpan.Minutes,
                Seconds = timeSpan.Seconds,
                IsOpenIntervalId = openInterval == null? 0:openInterval.Id
            };
            return new JsonResult(viewModel, JsonSettings);
        }

        [HttpGet("current/{id?}")]
        public async Task<IActionResult> GetElementCurrentInterval(long? id)
        {
            var openInterval = await IntervalRepo.GetOpenIntervalAsync(id);

            //handle requests asking for non-existing Intervals on NodeElement
            if (openInterval == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no Open Intervals for NodeElement {0} has been found", id)
                });
            }

            //Setting end of open interval to count total 
            openInterval = await IntervalRepo.SetEndAsync(openInterval.Id);

            var timeSpan = TimeSpan.FromSeconds(Convert.ToDouble(openInterval.TotalSecond));
            ElementSpanViewModel viewModel = new ElementSpanViewModel()
            {
                NodeElementId = (long)id,
                Days = timeSpan.Days,
                Hours = timeSpan.Hours + timeSpan.Days * 24,
                Minutes = timeSpan.Minutes,
                Seconds = timeSpan.Seconds,
                IsOpenIntervalId = openInterval == null ? 0 : openInterval.Id
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
                var error = String.Format("NodeElement {0} has not been found", elementId);
                Log.Error($"IntervalController: {error}");
                return NotFound(new
                {
                    Error = error
                });
            }

            var intervalItem = await IntervalRepo.GetOpenIntervalAsync(nodeElement.Id);

            //handle requests asking for non-existing NodeElement
            if (intervalItem == null)
            {
                var error = String.Format("There is no Interval with property IsOpen == true on NodeElement {0}", elementId);
                Log.Error($"IntervalController: {error}");
                return NotFound(new
                {
                    Error = error
                });
            }

            var result = await IntervalRepo.SetEndAsync(intervalItem.Id, 
                finish == null ? false : (bool)finish);

            return new JsonResult(result.Adapt<IntervalViewModel>(), JsonSettings);

        }

        [HttpPost("updateend/element/")]
        public async Task<IActionResult> UpdateEnd([FromBody] IntervalViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var interval = await IntervalRepo.GetInterval(model.Id);
            DateTime end = interval.End.AddSeconds(
                Convert.ToDouble((model.Seconds + model.Minutes * 60 + model.Hours * 60 * 60)-interval.TotalSecond));
            var result = await IntervalRepo.UpdateEndAsync(model.Id, end);

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
                var error = String.Format("NodeElement {0} has not been found", elementId);
                Log.Error($"IntervalController: {error}");
                return NotFound(new
                {
                    Error = error
                });
            }

            var result = await IntervalRepo.SetStartAsync(nodeElement.Id, start);

            //handle requests asking for non-existing NodeElement
            if (result == null)
            {
                var error = String.Format("There is no Interval with property IsOpen == true on NodeElement {0}", elementId);
                Log.Error($"IntervalController: {error}");
                return NotFound(new
                {
                    Error = error
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

            var result = await IntervalRepo.DeleteIntervalAsync((long)id);

            // handle requests asking for non-existing nodeElement
            if (result == null)
            {
                var error = String.Format("Interval {0} has not been found", id);
                Log.Error($"IntervalController: {error}");
                return NotFound(new
                {
                    Error = error
                });
            }

            // return an HTTP Status 200 (OK).
            return new OkResult();
        }

        /// <summary>
        ///  GET: api/interval/{id}
        ///  Retrieves the IntervalViewModel object with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Interval</param>
        /// <returns>NodeElement intervals with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long? id)
        {
            if (id == null) return new StatusCodeResult(500);

            var intervals = await IntervalRepo.GetElementIntervalsAsync((long)id);

            //handle requests asking for non-existing NodeElement
            if (intervals == null)
            {
                var error = String.Format("Interval {0} has not been found", id);
                Log.Error($"IntervalController: {error}");
                return NotFound(new
                {
                    Error = error
                });
            }

            var viewModel  =  IntervalConverter
                .FillDHMSProp(intervals
                .OrderByDescending(d=>d.Start)
                .Adapt<IEnumerable<IntervalViewModel>>());

            // output the result in JSON format
            return new JsonResult(viewModel, JsonSettings);
        }

        // POST: api/Interval
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IntervalViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);
            var itemToUpdate = model.Adapt<Interval>();
            var result = await IntervalRepo.UpdateIntervalAsync(itemToUpdate);
            return new JsonResult(result.Adapt<IntervalViewModel>());
        }

        // PUT: api/interval/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long? id)
        {
            if (id == null) return new StatusCodeResult(500);
            var result = await IntervalRepo.CreateIntervalAsync((long)id);
            return new JsonResult(result.Adapt<IntervalViewModel>(), JsonSettings);
        }
        #endregion
    }
}
