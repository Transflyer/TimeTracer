using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public interface IIntervalRepository
    {
        IQueryable<Interval> Intervals { get; }

        Task<Interval> CreateIntervalAsync(long elementId);

        Task<Interval> DeleteIntervalAsync(long id);

        Task<Interval> UpdateIntervalAsync(Interval interval);

        Task<Interval> SetStartAsync(long? id, DateTime start);

        Task<Interval> SetEndAsync(long id, bool finish = false);

        Task<Interval> UpdateEndAsync(long id, DateTime end);

        Task<IEnumerable<Interval>> GetElementIntervalsAsync(long? nodeElementId, DateTime? from = null, DateTime? to = null);

        Task<Interval> GetOpenIntervalAsync(long? nodeElementId);

        Task<Interval> GetInterval(long id);
    }
}