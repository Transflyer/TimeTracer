using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface ITimeSpentRepository
    {
        IQueryable<TimeSpent> TimeSpents { get; }
        Task<TimeSpent> CreateTimeSpentAsync(TimeSpent timeSpent, long? elementId);
        Task<TimeSpent> SetStartAsync(long? id, DateTime start);
        Task<TimeSpent> SetEndAsync(long? id, DateTime end);
        Task<IEnumerable<TimeSpent>> GetElementTimeSpentAsync(long? elementId, DateTime? from, DateTime? to);
       
    }
}
