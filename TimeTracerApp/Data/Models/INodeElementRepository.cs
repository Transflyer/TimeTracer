using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface INodeElementRepository
    {
        IQueryable<NodeElement> NodeElements { get; }

        Task<NodeElement> AddChildElement(NodeElement nodeElement, long? parentElementId);

        Task<NodeElement> AddUserNodeElement(NodeElement nodeElement, string userId);

        Task<NodeElement> DeleteNodeElement(long? id);

        Task<IEnumerable<NodeElement>> GetChildElements(long? parentElementId);

        Task<NodeElement> GetNodeElement(long id);

        Task<IEnumerable<NodeElement>> GetParentElements(long? childElementId);

        NodeElement MoveChildElementToOtherParent(long childElementId, long OtherParentElementId);

        NodeElement RemoveChildElement(long childElementId);

        Task<NodeElement> UpdateNodeElement(NodeElement model);

        IEnumerable<NodeElement> UserNodeElements(string userId);
    }
}