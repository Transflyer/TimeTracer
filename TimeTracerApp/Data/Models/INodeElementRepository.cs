using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface INodeElementRepository
    {
        IQueryable<NodeElement> NodeElements { get; }

        Task<NodeElement> AddChildElementAsync(NodeElement nodeElement, long? parentElementId);

        Task<NodeElement> CreateUserNodeElementAsync(NodeElement nodeElement, string userId);

        Task<NodeElement> DeleteNodeElementAsync(long? id);

        Task<IEnumerable<NodeElement>> GetChildElementsAsync(long? parentElementId);

        Task<NodeElement> GetNodeElementAsync(long? id);

        Task<IEnumerable<NodeElement>> GetParentElementsAsync(long? childElementId);

        Task<NodeElement> MoveChildElementToOtherParentAsync(long childElementId, long OtherParentElementId);

        Task<NodeElement> RemoveChildElementAsync(long childElementId);

        Task<NodeElement> UpdateNodeElementAsync(NodeElement model);

        Task<IEnumerable<NodeElement>> UserNodeElementsAsync(string userId);
    }
}