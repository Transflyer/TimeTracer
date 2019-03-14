using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public class TimeSpentRepository : ITimeSpentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly INodeElementRepository nodeElementRepo;

        #region Constructor

        public TimeSpentRepository(ApplicationDbContext ctx,
            INodeElementRepository repo)
        {
            context = ctx;
            nodeElementRepo = repo;
        }

        #endregion Constructor

        public IQueryable<TimeSpent> TimeSpents => context.TimeSpents;

        public async Task<TimeSpent> CreateTimeSpentAsync(long elementId)
        {
            var element = await nodeElementRepo.GetNodeElementAsync(elementId);
            if (element == null) return null;

            //Only one TimeSpent must be with property IsOpen == true,
            //thus we need set every existing TimeSpents property IsOpen to false regarding Current User
            var IsElementHasOpenedTimeSpents = TimeSpents
                .Where(o => o.IsOpen == true && o.UserId == element.UserId).ToArray();

            if (IsElementHasOpenedTimeSpents.Count() != 0)
            {
                foreach (var item in IsElementHasOpenedTimeSpents)
                {
                    item.IsOpen = false;
                    item.End = DateTime.UtcNow;
                    item.TotalSecond = Convert.ToInt64((item.End - item.Start).TotalSeconds);
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
                TotalSecond = 0,
                IsOpen = true,
                UserId = element.UserId
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
            item.UserId = "";

            await context.SaveChangesAsync();
            return item;
        }

        public async Task<TimeSpent> GetOpenTimeSpentAsync(long? nodeElementId) =>
            await TimeSpents.FirstOrDefaultAsync(e => e.ElementId == nodeElementId && e.IsOpen == true);

        public async Task<IEnumerable<TimeSpent>> GetElementTimeSpentsAsync(
            long? nodeElementId, DateTime? from = null, DateTime? to = null)
        {
            if (nodeElementId == null) return null;

            if (from == null && to == null) return await TimeSpents
                    .Where(n => n.ElementId == nodeElementId)
                    .ToArrayAsync();

            if (from != null && to == null) return await TimeSpents
                    .Where(n => n.ElementId == nodeElementId && n.Start > from)
                    .ToArrayAsync();

            if (from == null && to != null) return await TimeSpents
                    .Where(n => n.ElementId == nodeElementId && n.End < to)
                    .ToArrayAsync();

            if (from != null && to != null) return await TimeSpents
                    .Where(n => n.ElementId == nodeElementId && n.Start > from && n.End < to)
                    .ToArrayAsync();

            return null;
        }

        public async Task<TimeSpent> SetEndAsync(long id, bool finish = false)
        {
            var result = await context.TimeSpents.FindAsync(id);
            if (result == null) return null;
            result.End = DateTime.UtcNow;
            result.TotalSecond = Convert.ToInt64((result.End - result.Start).TotalSeconds);
            result.LastModifiedDate = DateTime.UtcNow;
            if (finish == true) result.IsOpen = false;
            await context.SaveChangesAsync();
            return result;
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
            updatedItem.UserId = timeSpent.UserId;

            await context.SaveChangesAsync();
            return updatedItem;
        }

        public async Task<TimeSpent> UpdateEndAsync(long id, DateTime end)
        {
            var result = await context.TimeSpents.FindAsync(id);
            if (result == null) return null;
            result.End = end;
            result.TotalSecond = Convert.ToInt64((result.End - result.Start).TotalSeconds);
            result.LastModifiedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return result;
        }

        public Task<TimeSpent> GetTimeSpent(long id) =>
            TimeSpents.FirstOrDefaultAsync(e => e.Id == id);
        
    }
}