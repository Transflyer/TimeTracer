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

        public IQueryable<NodeElement> NodeElements => context.NodeElements;

        public NodeElementRepository(ApplicationDbContext ctx) => context = ctx;

        public IEnumerable<NodeElement> UserNodeElements(string userId) => context
            .NodeElements
            .Where(u => u.UserId == userId)
            .ToArray();

        public NodeElement AddUserNodeElement(NodeElement nodeElement, string userId)
        {
            //properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // Set a author using user login
            nodeElement.UserId = userId;

            //add new nodeElement
            context.NodeElements.Add(nodeElement);

            //persist the newly-created NodeElement into the Database
            context.SaveChanges();

            return nodeElement;
        }

        public async Task<NodeElement> GetNodeElement(int id)
        {
            return await NodeElements.FirstOrDefaultAsync(i => i.Id == id);
        }

        public NodeElement DeleteNodeElement(int? id)
        {
            if (id == null) return null;

            // retrieve the nodeElement
            var detetedElement = context.NodeElements.Where(i => i.Id == id).FirstOrDefault();

            if (detetedElement == null) return null;

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
            nodeElementToUpdate.ParentId = nodeElement.ParentId;
            //nodeElementToUpdate.Text = nodeElement.Text;
            //nodeElementToUpdate.UserId = nodeElement.UserId;
            //nodeElementToUpdate.Notes = nodeElement.Notes;

            // properties set from server-side
            nodeElementToUpdate.LastModifiedDate = nodeElement.CreatedDate;

            // persist the changes into the Database.
            context.SaveChanges();
            return nodeElementToUpdate;
        }

        public async Task<IEnumerable<NodeElement>> GetChildElements(int? parentElementId)
        {
            if (parentElementId == null) return null;
            return await NodeElements
                .Where(p => p.ParentId == parentElementId)
                .OrderBy(t => t.Title)
                .ToArrayAsync();
        }

        public async Task<NodeElement> AddChildElement(NodeElement nodeElement, int? parentElementId)
        {
            if (nodeElement == null || parentElementId == null) return null;

            //properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // Set a parent element
            nodeElement.ParentId = parentElementId;

            //add new nodeElement
            context.NodeElements.Add(nodeElement);

            //persist the newly-created NodeElement into the Database
            await context.SaveChangesAsync();

            return nodeElement;
        }

        public NodeElement RemoveChildElement(int childElementId)
        {
            throw new NotImplementedException();
        }

        public NodeElement MoveChildElementToOtherParent(int childElementId, int OtherParentElementId)
        {
            throw new NotImplementedException();
        }
    }
}
