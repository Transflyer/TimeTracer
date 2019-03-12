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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "JwtAuthorization")]
    public class ElementsController : BaseApiController
    {
        #region Properties
        private readonly INodeElementRepository NodeElementRepo;
        private readonly ITimeSpentRepository TimeSpentRepo;
        #endregion

        #region Constructor
        public ElementsController(ApplicationDbContext context,
           RoleManager<IdentityRole> roleManager,
           IRequestUserProvider requstUserProvider,
           IConfiguration configuration,
           INodeElementRepository elementRepo,
           ITimeSpentRepository timeRepo)
           : base(context, roleManager, requstUserProvider, configuration)
        {
            NodeElementRepo = elementRepo;
            TimeSpentRepo = timeRepo;
        }
        #endregion

        #region  Attribute-based routing methods
        // GET: api/elements/root/
        [HttpGet("root/{parentId?}")]
        public async Task<IActionResult> GetChildElements(long? parentId)
        {
            if (parentId == null) return new StatusCodeResult(500);

            var nodeElements = await NodeElementRepo.GetChildElementsAsync(parentId);

            if (nodeElements == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no child elements with {0} parent element", parentId)
                });
            }

            return new JsonResult(nodeElements.Adapt<ElementViewModel[]>(), JsonSettings);
        }

        // GET: api/elements/parents/
        [HttpGet("parents/{childId}")]
        public async Task<IActionResult> GetParentElements(long? childId)
        {
            if (childId == null) return new StatusCodeResult(500);

            var nodeElements = await NodeElementRepo.GetParentElementsAsync(childId);

            if (nodeElements == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no parent elements chain with {0} child element", childId)
                });
            }

            return new JsonResult(nodeElements.Adapt<ElementViewModel[]>(), JsonSettings);
        }
        #endregion

        #region RESTful conventions methods

        /// <summary>
        /// GET: api/elements/{id}
        /// Retrieves the NodeElement with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NodeElement</param>
        /// <returns>the NodeElement with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            var nodeElement = await NodeElementRepo.GetNodeElementAsync(id);
            
            //handle requests asking for non-existing NodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", id)
                });
            }
            
            bool isStarted = (await TimeSpentRepo.GetOpenTimeSpentAsync(nodeElement.Id))==null ? false:true;
            var model = nodeElement.Adapt<ElementViewModel>();
            model.IsStarted = isStarted;

            // output the result in JSON format
            return new JsonResult(model, JsonSettings);
        }

        /// <summary>
        /// Adds a new NodeElement to the Database
        /// </summary>
        /// <param name="m">The ElementViewModel containing the data to insert</param>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] NodeElement model)
        {
            if (model == null) return new StatusCodeResult(500);
            if (model.ParentId == null) return new StatusCodeResult(500);

            var nodeElement = await NodeElementRepo.AddChildElementAsync(model, model.ParentId);

            //return the newly-created NodeElement to the client.
            return new JsonResult(nodeElement.Adapt<ElementViewModel>(),
                JsonSettings);
        }

        // POST: api/Elements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NodeElement model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            var nodeElement = await NodeElementRepo.UpdateNodeElementAsync(model);

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

        // DELETE: api/elements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] long? id)
        {
            if (id == null) return new StatusCodeResult(500);

            var result = await NodeElementRepo.DeleteNodeElementAsync(id);

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

        private bool NodeElementExists(long id)
        {
            throw new NotImplementedException();
        }
    }
}