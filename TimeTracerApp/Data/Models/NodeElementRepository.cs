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

        public NodeElement GetNodeElement(int id)
        {
            return context.NodeElements.FirstOrDefault(i => i.Id == id);
        }

        public NodeElement DeleteNodeElement(int id)
        {
            // retrieve the nodeElement
            var detetedElement = context.NodeElements.Where(i => i.Id == id).FirstOrDefault();

            if(detetedElement == null) return null;

            //remove the NodeElement from DbContext
            context.Remove(detetedElement);

            // persist the changes into the Database.
            context.SaveChanges();

            return detetedElement;
        }

        public NodeElement UpdateNodeElement(NodeElement nodeElement)
        {

            // retrieve the nodeElement to edit
            var nodeElementToUpdate = context.NodeElements.Where(i => i.Id == nodeElement.Id).FirstOrDefault();

            // handle the update (without object-mapping)
            // by manually assigning the properties
            // we want to accept from the request
            nodeElementToUpdate.Title = nodeElement.Title;
            nodeElementToUpdate.Description = nodeElement.Description;

            // properties set from server-side
            nodeElementToUpdate.LastModifiedDate = nodeElement.CreatedDate;

            //Update db element
            context.NodeElements.Update(nodeElementToUpdate);

            // persist the changes into the Database.
            context.SaveChanges();
            return nodeElementToUpdate;
        }
    }
}
