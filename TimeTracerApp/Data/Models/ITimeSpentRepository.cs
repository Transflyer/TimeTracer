using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface ITimeSpentRepository
    {
        IQueryable<TimeSpent> TimeSpents { get; }
        Task<TimeSpent> CreateTimeSpent(TimeSpent timeSpent, long? elementId);
        Task<TimeSpent> SetStart(long? id, DateTime start);
        Task<TimeSpent> SetEnd(long? id, DateTime end);
        Task<IEnumerable<TimeSpent>> GetElementTimeSpent(long? elementId, DateTime? from, DateTime? to);
       
    }
}
