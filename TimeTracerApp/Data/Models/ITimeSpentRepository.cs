using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface ITimeSpentRepository
    {
        IQueryable<TimeSpent> TimeSpents { get; }

        Task<TimeSpent> CreateTimeSpentAsync(long elementId);

        Task<TimeSpent> DeleteTimeSpentAsync(long id);

        Task<TimeSpent> UpdateTimeSpentAsync(TimeSpent timeSpent);

        Task<TimeSpent> SetStartAsync(long? id, DateTime start);

        Task<TimeSpent> SetEndAsync(long id, bool finish = false);

        Task<TimeSpent> UpdateEndAsync(long id, DateTime end);

        Task<IEnumerable<TimeSpent>> GetElementTimeSpentsAsync(long? nodeElementId, DateTime? from = null, DateTime? to = null);

        Task<TimeSpent> GetOpenTimeSpentAsync(long? nodeElementId);

        Task<TimeSpent> GetTimeSpent(long id);
    }
}