using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using TimeTracker.Services;
using TimeTracker.ViewModels;
using System.Collections.Generic;


namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseApiController
    {
        #region Constructor

        public ProjectController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRequestUserProvider requstUserProvider,
            IConfiguration configuration,
            INodeElementRepository elementRepo)
            : base(context, roleManager, requstUserProvider, configuration)
        {
            NodeElementRepo = elementRepo;
        }

        #endregion Constructor

        #region Properties

        private readonly INodeElementRepository NodeElementRepo;

        #endregion Properties

        #region RESTful conventions methods

        /// <summary>
        /// GET: api/project/{}id
        /// Retrieves the NodeElement with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NodeElement</param>
        /// <returns>the NodeElement with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long? id)
        {
            if (id == null) return new StatusCodeResult(500);

            var nodeElement = await NodeElementRepo.GetNodeElement(id);

            //handle requests asking for non-existing NodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement ID {0} has not been found", id)
                });
            }

            // output the result in JSON format
            return new JsonResult(nodeElement.Adapt<ElementViewModel>(), JsonSettings);
        }

        /// <summary>
        /// Adds a new NodeElement to the Database
        /// </summary>
        /// <param name="m">The ElementViewModel containing the data to insert</param>
        [HttpPut]
        [Authorize(Policy = "JwtAuthorization")]
        public async Task<IActionResult> Put([FromBody]NodeElement model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            var userId = RequestUserProvider.GetUserId();

            var element = await NodeElementRepo.AddUserNodeElement(model, userId);
            //return the newly-created NodeElement to the client.
            return new JsonResult(element.Adapt<ElementViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Edit the NodeElement with the given {id}
        /// </summary>
        /// <param name="m">The ElementViewModel containing the data to  update</param>
        [HttpPost]
        [Authorize(Policy = "JwtAuthorization")]
        public async Task<IActionResult> Post([FromBody]NodeElement model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            model.UserId = RequestUserProvider.GetUserId();

            var nodeElement = await NodeElementRepo.UpdateNodeElement(model);

            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", model.Id)
                });
            }

            //return the updated NodeElement to the client.
            return new JsonResult(nodeElement.Adapt<ElementViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Deletes the NodeElement with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing NodeElement</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = "JwtAuthorization")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return new StatusCodeResult(500);

            var result = await NodeElementRepo.DeleteNodeElement(id);

            // handle requests asking for non-existing nodeElement
            if (result == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", id)
                });
            }
            // return an HTTP Status 200 (OK).
            return new OkResult();
        }

        #endregion RESTful conventions methods

        #region Attribute-based routing methods

        /// <summary>
        /// GET: api/project/root
        /// Retrieves the {num} NodeElements sorted by Title (A to Z)
        /// </summary>
        /// <param name="num">the number of NodeElements to retrieve</param>
        /// <returns>{num} NodeElements sorted by Title</returns>

        [HttpGet("root/{num:int?}")]
        [Authorize(Policy = "JwtAuthorization")]
        public async Task<IActionResult> Root(int num = 10)
        {
            var userId = RequestUserProvider.GetUserId();
            var rootElements = (await NodeElementRepo.UserNodeElements(userId))
                .OrderBy(q => q.Title)
                .Take(num);

            return new JsonResult(rootElements.Adapt<ElementViewModel[]>(), JsonSettings);
        }

        #endregion Attribute-based routing methods
    }
}