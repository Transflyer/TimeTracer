using Mapster;
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

namespace TimeTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // GET: api/NodeElements
        [HttpGet]
        public IEnumerable<NodeElement> GetChildElements(int parentId)
        {
            throw new NotImplementedException();
        }

        // GET: api/NodeElements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        // PUT: api/NodeElements/5
        [HttpPut("{parentid}")]
        public async Task<IActionResult> Put([FromBody] NodeElement model, [FromRoute] int? parentid)
        {
            if (model == null || parentid == null) return new StatusCodeResult(500);
            var nodeElement = await NodeElementRepo.AddChildElement(model, parentid);

            //return the newly-created NodeElement to the client.
            return new JsonResult(nodeElement.Adapt<ProjectViewModel>(),
                JsonSettings);
            
        }

        // POST: api/NodeElements
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NodeElement nodeElement)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/NodeElements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

        private bool NodeElementExists(int id)
        {
            throw new NotImplementedException();
        }
    }
}