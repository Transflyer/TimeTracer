using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data;

namespace TimeTracker.Data.Models
{
    public class NodeElementRepository : INodeElementRepository
    {
        private ApplicationDbContext context;
        public NodeElementRepository(ApplicationDbContext ctx) => context = ctx;

        public IEnumerable<NodeElement> UserNodeElements(ApplicationUser user) => context
            .NodeElements
            .Where(u => u.UserId == user.Id)
            .ToArray();

        public NodeElement AddUserNodeElement(NodeElement nodeElement, ApplicationUser user)
        {
            //properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // Set a author using user login
            nodeElement.UserId = user.Id;

            //add new nodeElement
            context.NodeElements.Add(nodeElement);

            //persist the newly-created NodeElement into the Database
            context.SaveChanges();

            return nodeElement;
        }
    }
}
