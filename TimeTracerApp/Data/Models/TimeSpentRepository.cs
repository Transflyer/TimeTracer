using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeTracker.Data.Models
{
    public class TimeSpentRepository : ITimeSpentRepository
    {
        private ApplicationDbContext context;

        #region Constructor
        public TimeSpentRepository(ApplicationDbContext ctx) => context = ctx;
        #endregion

        public IQueryable<TimeSpent> TimeSpents => context.TimeSpents;

        public async Task<TimeSpent> CreateTimeSpentAsync(TimeSpent timeSpent, long? elementId)
        {
            if (timeSpent == null || elementId == null) return null;

            //properties set from server-side
            timeSpent.CreatedDate = DateTime.UtcNow;
            timeSpent.LastModifiedDate = timeSpent.CreatedDate;
            timeSpent.Start = timeSpent.CreatedDate;
            timeSpent.End = timeSpent.CreatedDate;

            timeSpent.ElementId = (long)elementId;

            context.TimeSpents.Add(timeSpent);
            await context.SaveChangesAsync();

            return timeSpent;
        }

        public async Task<IEnumerable<TimeSpent>> GetElementTimeSpentAsync(long? elementId, DateTime? from, DateTime? to)
        {
            if (elementId == null) return null;

            if (from == null && to == null) return await TimeSpents
                    .Where(n => n.ElementId == elementId)
                    .ToArrayAsync();

            if (from != null && to == null) return await TimeSpents
                    .Where(n => n.ElementId == elementId && n.Start > from)
                    .ToArrayAsync();

            if (from == null && to != null) return await TimeSpents
                    .Where(n => n.ElementId == elementId && n.End < to)
                    .ToArrayAsync();

            if (from != null && to != null) return await TimeSpents
                    .Where(n => n.ElementId == elementId &&  n.Start > from && n.End < to)
                    .ToArrayAsync();

            return null;
        }

        public async Task<TimeSpent> SetEndAsync(long? id, DateTime end)
        {
            TimeSpent item = await TimeSpents.FirstOrDefaultAsync(e => e.Id == id);
            if (item == null) return null;
            item.End = end;
            item.LastModifiedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return item;
        }

        public Task<TimeSpent> SetStartAsync(long? id, DateTime start)
        {
            throw new NotImplementedException();
        }
    }
}
