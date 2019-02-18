using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface INodeElementRepository
    {
        IQueryable<NodeElement> NodeElements { get; }

        IEnumerable<NodeElement> UserNodeElements(string userId);
        NodeElement AddUserNodeElement(NodeElement nodeElement, string userId);
        Task<NodeElement> GetNodeElement(int id);
        Task<IEnumerable<NodeElement>> GetChildElements(int? parentElementId);
        Task<NodeElement> AddChildElement(NodeElement nodeElement, int? parentElementId);
        NodeElement RemoveChildElement(int childElementId);
        NodeElement MoveChildElementToOtherParent(int childElementId, int OtherParentElementId);
        NodeElement DeleteNodeElement(int? id);
        NodeElement UpdateNodeElement(NodeElement nodeElement);
    }
}
