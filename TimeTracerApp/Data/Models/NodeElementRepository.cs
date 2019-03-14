using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public class NodeElementRepository : INodeElementRepository
    {
        private ApplicationDbContext context;

        #region Constructor

        public NodeElementRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        #endregion Constructor

        public IQueryable<NodeElement> NodeElements => context.NodeElements;

        public async Task<NodeElement> AddChildElementAsync(NodeElement nodeElement, long? parentElementId)
        {
            if (nodeElement == null || parentElementId == null) return null;
            var parent = await NodeElements.FirstOrDefaultAsync(r => r.Id == (long)parentElementId);

            //Properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            //Set a parent element
            nodeElement.ParentId = parentElementId;
            nodeElement.UserId = parent.UserId;

            //Add new nodeElement
            context.NodeElements.Add(nodeElement);

            //Persist the newly-created NodeElement into the Database
            await context.SaveChangesAsync();

            //Create empty TimeSpent element
            TimeSpent timeSpent = new TimeSpent()
            {
                Id = 0,
                CreatedDate = nodeElement.CreatedDate,
                LastModifiedDate = nodeElement.CreatedDate,
                ElementId = nodeElement.Id,
                Start = nodeElement.CreatedDate,
                End = nodeElement.CreatedDate,
                IsOpen = false,
                TotalSecond = 0,
                UserId = nodeElement.UserId
            };

            context.TimeSpents.Add(timeSpent);
            await context.SaveChangesAsync();

            return nodeElement;
        }

        public async Task<NodeElement> CreateUserNodeElementAsync(NodeElement nodeElement, string userId)
        {
            //properties set from server-side
            nodeElement.CreatedDate = DateTime.UtcNow;
            nodeElement.LastModifiedDate = nodeElement.CreatedDate;

            // Set a author using user login
            nodeElement.UserId = userId;

            //add new nodeElement
            context.NodeElements.Add(nodeElement);

            //Create initial child TimeSpent entity
            var cd = DateTime.UtcNow;
            context.TimeSpents.Add(new TimeSpent()
            {
                CreatedDate = cd,
                TotalSecond = 0,
                ElementId = nodeElement.Id,
                IsOpen = false,
                UserId = nodeElement.UserId,
                Start = cd,
                End = cd,
                LastModifiedDate = cd
            });

            //persist the newly-created NodeElement into the Database
            await context.SaveChangesAsync();
            return nodeElement;
        }

        public async Task<NodeElement> DeleteNodeElementAsync(long? id)
        {
            if (id == null) return null;

            // retrieve the nodeElement
            var detetedElement = NodeElements.Where(i => i.Id == id).FirstOrDefault();

            if (detetedElement == null) return null;

            //set properties of deleted element
            detetedElement.Deleted = true;
            detetedElement.DeletedUserId = detetedElement.UserId;
            detetedElement.UserId = null;
            detetedElement.DeletedParentId = detetedElement.ParentId;
            detetedElement.ParentId = null;

            // persist the changes into the Database.
            await context.SaveChangesAsync();

            return detetedElement;
        }

        public async Task<IEnumerable<NodeElement>> GetChildElementsAsync(long? parentElementId)
        {
            if (parentElementId == null) return null;
            return await NodeElements
                .Where(p => p.ParentId == parentElementId)
                .OrderBy(t => t.Title)
                .ToArrayAsync();
        }

        public async Task<NodeElement> GetNodeElementAsync(long? id)
        {
            return await NodeElements.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<NodeElement>> GetParentElementsAsync(long? childElementId)
        {
            List<NodeElement> nodeElements = new List<NodeElement>();
            NodeElement item = await NodeElements.FirstOrDefaultAsync(elem => elem.Id == childElementId);
            while (item != null)
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

        public Task<NodeElement> MoveChildElementToOtherParentAsync(long childElementId, long OtherParentElementId)
        {
            throw new NotImplementedException();
        }

        public Task<NodeElement> RemoveChildElementAsync(long childElementId)
        {
            throw new NotImplementedException();
        }

        public async Task<NodeElement> UpdateNodeElementAsync(NodeElement nodeElement)
        {
            // handle requests asking for non-existing nodeElement
            if (nodeElement == null)
            {
                return null;
            }

            var elementToUpdate = await GetNodeElementAsync(nodeElement.Id);
            if (elementToUpdate == null) return null;

            elementToUpdate.Description = nodeElement.Description;
            elementToUpdate.Text = nodeElement.Text;
            elementToUpdate.Title = nodeElement.Title;
            elementToUpdate.Notes = nodeElement.Notes;
            //elementToUpdate.UserId = nodeElement.UserId;
            //elementToUpdate.ParentId = nodeElement.ParentId;

            // properties set from server-side
            elementToUpdate.LastModifiedDate = DateTime.UtcNow;

            // persist the changes into the Database.
            await context.SaveChangesAsync();

            return elementToUpdate;
        }

        public async Task<IEnumerable<NodeElement>> UserNodeElementsAsync(string userId)
        {
            return await NodeElements
            .Where(u => u.UserId == userId)
            .ToArrayAsync();
        }

        public async Task<IEnumerable<NodeElement>> UserNodeElementsWithTimeSpentsAsync(string userId)
        {
            var userElements = await NodeElements
                .Where(u => u.UserId == userId)
                .Include(t=>t.TimeSpents)
                .ToArrayAsync();

            //Constraction NodeElement tree
            foreach (var elem in userElements)
            {
                var childs = userElements.Where(t => t.ParentId == elem.Id);
            }

            //Filter - only root elements
            return userElements.Where(r => r.ParentId == null);
        }
    }
}