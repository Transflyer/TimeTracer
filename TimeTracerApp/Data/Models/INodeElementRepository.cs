using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface INodeElementRepository
    {
        IEnumerable<NodeElement> UserNodeElements(ApplicationUser user);
        NodeElement AddUserNodeElement(NodeElement nodeElement, ApplicationUser user);
        NodeElement GetNodeElement(int id);
        NodeElement DeleteNodeElement(int id);
        NodeElement UpdateNodeElement(NodeElement nodeElement);
    }
}
