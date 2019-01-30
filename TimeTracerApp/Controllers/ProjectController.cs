using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using TimeTracker.ViewModels;

namespace TimeTracker.Controllers
{

    [Route("api/[controller]")]
    public class ProjectController : BaseApiController
    {
        #region Constructor
        public ProjectController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
            : base(context, roleManager, userManager, configuration)
        { }
        #endregion


        #region RESTful conventions methods
        /// <summary>
        /// GET: api/project/{}id
        /// Retrieves the NodeElement with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NodeElement</param>
        /// <returns>the NodeElement with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var nodeElement = DbContext.NodeElements.Where(i => i.Id == id).FirstOrDefault();

            //handle requests asking for non-existing NodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                    {
                        Error = String.Format("NodeElement ID {0} has not been found", id)
                    });
            }

            // output the result in JSON format
            return new JsonResult(nodeElement.Adapt<ProjectViewModel>(), JsonSettings);

        }

        /// <summary>
        /// Adds a new NodeElement to the Database
        /// </summary>
        /// <param name="m">The ProjectViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put([FromBody]NodeElement model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            //handle the insert (window oblect-mapping)
            var nodeElement = new NodeElement();

            //properties taken from request
            nodeElement.Title = model.Title;
            nodeElement.Description = model.Description;

            //properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // Set a temporary author using the Admin user's userId
            // as user login isn't supported yet: we'll change this later on.
            nodeElement.UserId = DbContext.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

            //add new nodeElement
            DbContext.NodeElements.Add(nodeElement);

            //persist the newly-created NodeElement into the Database
            DbContext.SaveChanges();

            //return the newly-created NodeElement to the client.
            return new JsonResult(nodeElement.Adapt<ProjectViewModel>(),
                JsonSettings);

        }
        /// <summary>
        /// Edit the NodeElement with the given {id}
        /// </summary>
        /// <param name="m">The ProjectViewModel containing the data to  update</param>
        [HttpPost]
        public IActionResult Post([FromBody]NodeElement model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            // retrieve the nodeElement to edit
            var nodeElement = DbContext.NodeElements.Where(i => i.Id == model.Id).FirstOrDefault();

            // handle requests asking for non-existing nodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                    {
                        Error = String.Format("NodeElement {0} has not been found", model.Id)
                    });
            }

            // handle the update (without object-mapping)
            // by manually assigning the properties
            // we want to accept from the request
            nodeElement.Title = model.Title;
            nodeElement.Description = model.Description;

            // properties set from server-side
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // persist the changes into the Database.
            DbContext.SaveChanges();

            //return the updated NodeElement to the client.
            return new JsonResult(nodeElement.Adapt<ProjectViewModel>(),
                JsonSettings);
        }
        /// <summary>
        /// Deletes the NodeElement with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing NodeElement</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the nodeElement to edit
            var nodeElement = DbContext.NodeElements.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing nodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", id)
                });
            }

            //remove the NodeElement from DbContext
            DbContext.Remove(nodeElement);

            // persist the changes into the Database.
            DbContext.SaveChanges();

            // return an HTTP Status 200 (OK).
            return new OkResult();
        }
        #endregion

        #region Attribute-based routing methods
        /// <summary>
        /// GET: api/project/ByTitle
        /// Retrieves the {num} NodeElements sorted by Title (A to Z)
        /// </summary>
        /// <param name="num">the number of NodeElements to retrieve</param>
        /// <returns>{num} NodeElements sorted by Title</returns>

        [HttpGet("root/{num:int?}")]
        public IActionResult Root(int num=10)
        {
            // Set a temporary author using the Admin user's userId
            // as user login isn't supported yet: we'll change this later on.
            var userId = DbContext.Users.Where(u => u.UserName == "Admin")
                .FirstOrDefault().Id;

            var rootElements = DbContext.NodeElements
                .Where(u => u.UserId == userId)
                .OrderByDescending(q => q.Title)
                .Take(num)
                .ToArray();

            return new JsonResult(rootElements.Adapt<ProjectViewModel[]>(), JsonSettings);
        }

        #endregion
    }
}
