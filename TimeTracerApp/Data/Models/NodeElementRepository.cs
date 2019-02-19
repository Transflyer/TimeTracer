using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Data;
using TimeTracker.ViewModels;

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

        public async Task<NodeElement> AddUserNodeElement(NodeElement nodeElement, string userId)
        {
            //properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // Set a author using user login
            nodeElement.UserId = userId;

            //add new nodeElement
            context.NodeElements.Add(nodeElement);

            //persist the newly-created NodeElement into the Database
            await context.SaveChangesAsync();

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

        public async Task<NodeElement> UpdateNodeElement(NodeElement nodeElement)
        {

            // handle requests asking for non-existing nodeElement
            if (nodeElement == null)
            {
                return null;
            }

            var elementToUpdate = await GetNodeElement(nodeElement.Id);
            if (elementToUpdate == null) return null;

            elementToUpdate.Description = nodeElement.Description;
            elementToUpdate.Text = nodeElement.Text;
            elementToUpdate.Title = nodeElement.Title;
            elementToUpdate.Notes = nodeElement.Notes;
            elementToUpdate.UserId = nodeElement.UserId;
            elementToUpdate.ParentId = nodeElement.ParentId;

            // properties set from server-side
            elementToUpdate.LastModifiedDate = DateTime.UtcNow;

          
            // persist the changes into the Database.
            await context.SaveChangesAsync();

            return elementToUpdate;
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

        public async Task<IEnumerable<NodeElement>> GetParentElements(int? childElementId)
        {
            
            List<NodeElement> nodeElements = new List<NodeElement>();
            NodeElement item = await NodeElements.FirstOrDefaultAsync(elem => elem.Id == childElementId);
            while(item != null)
            {
                if (item.ParentId == null)
                {
                    nodeElements.Reverse();
                    return nodeElements;
                }
                item = await NodeElements.FirstOrDefaultAsync(elem => elem.Id == item.ParentId);
                nodeElements.Add(item);
            }

            nodeElements.Reverse();
            return nodeElements;
        }
    }
}
