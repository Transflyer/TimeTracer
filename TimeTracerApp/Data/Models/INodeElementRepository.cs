using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface INodeElementRepository
    {
        IQueryable<NodeElement> Products { get; }

        IEnumerable<NodeElement> UserNodeElements(string userId);
        NodeElement AddUserNodeElement(NodeElement nodeElement, string userId);
        NodeElement GetNodeElement(int id);
        NodeElement DeleteNodeElement(int id);
        NodeElement UpdateNodeElement(NodeElement nodeElement);
    }
}
