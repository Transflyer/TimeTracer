﻿using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Data.Models;
using TimeTracker.Services;
using TimeTracker.ViewModels;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "JwtAuthorization")]
    public class ElementsController : BaseApiController
    {
        private readonly INodeElementRepository NodeElementRepo;

        public ElementsController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IRequestUserProvider requstUserProvider,
            IConfiguration configuration,
            INodeElementRepository elementRepo)
            : base(context, roleManager, requstUserProvider, configuration)
        {
            NodeElementRepo = elementRepo;
        }

        // GET: api/elements/root/
        [HttpGet("root/{parentId?}")]
        public async Task<IActionResult> GetChildElements(int? parentId)
        {
            if (parentId == null) return new StatusCodeResult(500);

            var nodeElements = await NodeElementRepo.GetChildElements(parentId);

            if (nodeElements == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no child elements with {0} parent element", parentId)
                });
            }

            return new JsonResult(nodeElements.Adapt<ElementViewModel []>(), JsonSettings);
        }

        // GET: api/elements/parents/
        [HttpGet("parents/{childId}")]
        public async Task<IActionResult> GetParentElements(int? childId)
        {
            if (childId == null) return new StatusCodeResult(500);

            var nodeElements = await NodeElementRepo.GetParentElements(childId);

            if (nodeElements == null)
            {
                return NotFound(new
                {
                    Error = String.Format("There are no parent elements chain with {0} child element", childId)
                });
            }

            return new JsonResult(nodeElements.Adapt<ElementViewModel[]>(), JsonSettings);
        }

        #region RESTful conventions methods

        /// <summary>
        /// GET: api/elements/{}id
        /// Retrieves the NodeElement with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing NodeElement</param>
        /// <returns>the NodeElement with the given {id}</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {

            var nodeElement = await NodeElementRepo.GetNodeElement(id);

            //handle requests asking for non-existing NodeElement
            if (nodeElement == null)
            {
                return NotFound(new
                {
                    Error = String.Format("NodeElement {0} has not been found", id)
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
        public async Task<IActionResult> Put([FromBody] NodeElement model)
        {
            if (model == null) return new StatusCodeResult(500);
            if (model.ParentId == null) return new StatusCodeResult(500);

            var nodeElement = await NodeElementRepo.AddChildElement(model, model.ParentId);

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

        // DELETE: api/Elements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        #endregion
        private bool NodeElementExists(int id)
        {
            throw new NotImplementedException();
        }
    }
}