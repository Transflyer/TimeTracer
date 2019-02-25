using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public class TimeSpentRepository : ITimeSpentRepository
    {
        private ApplicationDbContext context;

        #region Constructor

        public TimeSpentRepository(ApplicationDbContext ctx) => context = ctx;

        #endregion Constructor

        public IQueryable<TimeSpent> TimeSpents => context.TimeSpents;

        public async Task<TimeSpent> CreateTimeSpentAsync(long elementId)
        {
            var element = context.NodeElements.FirstOrDefaultAsync(i => i.Id == elementId);
            if (element == null) return null;


            //Only one TimeSpent must be with property IsOpen == true, 
            //thus we need set every existing TimeSpents property IsOpen to false
            var IsElementHasOpenedTimeSpents = TimeSpents.Where(o => o.IsOpen == true);
            if (IsElementHasOpenedTimeSpents.Count() > 0)
            {
                foreach(var item in IsElementHasOpenedTimeSpents)
                {
                    item.IsOpen = false;
                }
                context.TimeSpents.UpdateRange(IsElementHasOpenedTimeSpents);
                await context.SaveChangesAsync();
            }

            var CreatedDate = DateTime.UtcNow;
            TimeSpent timeSpent = new TimeSpent()
            {
                //properties set from server-side
                CreatedDate = CreatedDate,
                LastModifiedDate = CreatedDate,
                Start = CreatedDate,
                End = CreatedDate,
                IsOpen = true
            };
            timeSpent.ElementId = elementId;

            context.TimeSpents.Add(timeSpent);
            await context.SaveChangesAsync();

            return timeSpent;
        }

        public async Task<TimeSpent> DeleteTimeSpentAsync(long id)
        {
            var item = await TimeSpents.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return null;
            item.ElementId = 0;
            
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<TimeSpent>> GetElementTimeSpentsAsync(long? elementId, DateTime? from = null, DateTime? to = null)
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
                    .Where(n => n.ElementId == elementId && n.Start > from && n.End < to)
                    .ToArrayAsync();

            return null;
        }

        public async Task<TimeSpent> SetEndAsync(long id)
        {
            TimeSpent item = await TimeSpents.FirstOrDefaultAsync(e => e.Id == id);
            if (item == null) return null;
            item.End = DateTime.UtcNow;
            item.LastModifiedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<TimeSpent> SetStartAsync(long? id, DateTime start)
        {
            TimeSpent item = await TimeSpents.FirstOrDefaultAsync(e => e.Id == id);
            if (item == null) return null;
            item.Start = start;
            item.LastModifiedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<TimeSpent> UpdateTimeSpentAsync(TimeSpent timeSpent)
        {
            var updatedItem = await TimeSpents.FirstOrDefaultAsync(i => i.Id == timeSpent.Id);
            if (updatedItem == null) return null;

            updatedItem.CreatedDate = timeSpent.CreatedDate;
            updatedItem.ElementId = timeSpent.ElementId;
            updatedItem.End = timeSpent.End;
            updatedItem.LastModifiedDate = DateTime.UtcNow;
            updatedItem.Start = timeSpent.Start;
            updatedItem.IsOpen = timeSpent.IsOpen;

            await context.SaveChangesAsync();
            return updatedItem;
        }
    }
}