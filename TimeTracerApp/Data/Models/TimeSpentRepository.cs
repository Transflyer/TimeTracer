using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Data.Models
{
    public class IntervalRepository : IIntervalRepository
    {
        private readonly ApplicationDbContext context;
        private readonly INodeElementRepository nodeElementRepo;

        #region Constructor

        public IntervalRepository(ApplicationDbContext ctx,
            INodeElementRepository repo)
        {
            context = ctx;
            nodeElementRepo = repo;
        }

        #endregion Constructor

        public IQueryable<Interval> Intervals => context.Intervals;

        public async Task<Interval> CreateIntervalAsync(long elementId)
        {
            var element = await nodeElementRepo.GetNodeElementAsync(elementId);
            if (element == null) return null;

            //Only one Interval must be with property IsOpen == true,
            //thus we need set every existing Intervals property IsOpen to false regarding Current User
            var IsElementHasOpenedIntervals = Intervals
                .Where(o => o.IsOpen == true && o.UserId == element.UserId).ToArray();

            if (IsElementHasOpenedIntervals.Count() != 0)
            {
                foreach (var item in IsElementHasOpenedIntervals)
                {
                    item.IsOpen = false;
                    item.End = DateTime.UtcNow;
                    item.TotalSecond = Convert.ToInt64((item.End - item.Start).TotalSeconds);
                }
                context.Intervals.UpdateRange(IsElementHasOpenedIntervals);
                await context.SaveChangesAsync();
            }

            var CreatedDate = DateTime.UtcNow;
            Interval interval = new Interval()
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
            interval.ElementId = elementId;

            context.Intervals.Add(interval);
            await context.SaveChangesAsync();

            return interval;
        }

        public async Task<Interval> DeleteIntervalAsync(long id)
        {
            var item = await Intervals.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return null;
            item.ElementId = 0;
            item.UserId = "";

            await context.SaveChangesAsync();
            return item;
        }

        public async Task<Interval> GetOpenIntervalAsync(long? nodeElementId) =>
            await Intervals.FirstOrDefaultAsync(e => e.ElementId == nodeElementId && e.IsOpen == true);

        public async Task<IEnumerable<Interval>> GetElementIntervalsAsync(
            long? nodeElementId, DateTime? from = null, DateTime? to = null)
        {
            if (nodeElementId == null) return null;

            if (from == null && to == null) return await Intervals
                    .Where(n => n.ElementId == nodeElementId)
                    .ToArrayAsync();

            if (from != null && to == null) return await Intervals
                    .Where(n => n.ElementId == nodeElementId && n.Start > from)
                    .ToArrayAsync();

            if (from == null && to != null) return await Intervals
                    .Where(n => n.ElementId == nodeElementId && n.End < to)
                    .ToArrayAsync();

            if (from != null && to != null) return await Intervals
                    .Where(n => n.ElementId == nodeElementId && n.Start > from && n.End < to)
                    .ToArrayAsync();

            return null;
        }

        public async Task<Interval> SetEndAsync(long id, bool finish = false)
        {
            var result = await context.Intervals.FindAsync(id);
            if (result == null) return null;
            result.End = DateTime.UtcNow;
            result.TotalSecond = Convert.ToInt64((result.End - result.Start).TotalSeconds);
            result.LastModifiedDate = DateTime.UtcNow;
            if (finish == true) result.IsOpen = false;
            await context.SaveChangesAsync();
            return result;
        }

        public async Task<Interval> SetStartAsync(long? id, DateTime start)
        {
            Interval item = await Intervals.FirstOrDefaultAsync(e => e.Id == id);
            if (item == null) return null;
            item.Start = start;
            item.LastModifiedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return item;
        }

        public async Task<Interval> UpdateIntervalAsync(Interval interval)
        {
            var updatedItem = await Intervals.FirstOrDefaultAsync(i => i.Id == interval.Id);
            if (updatedItem == null) return null;

            updatedItem.CreatedDate = interval.CreatedDate;
            updatedItem.ElementId = interval.ElementId;
            updatedItem.End = interval.End;
            updatedItem.LastModifiedDate = DateTime.UtcNow;
            updatedItem.Start = interval.Start;
            updatedItem.IsOpen = interval.IsOpen;
            updatedItem.UserId = interval.UserId;

            await context.SaveChangesAsync();
            return updatedItem;
        }

        public async Task<Interval> UpdateEndAsync(long id, DateTime end)
        {
            var result = await context.Intervals.FindAsync(id);
            if (result == null) return null;
            result.End = end;
            result.TotalSecond = Convert.ToInt64((result.End - result.Start).TotalSeconds);
            result.LastModifiedDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return result;
        }

        public Task<Interval> GetInterval(long id) =>
            Intervals.FirstOrDefaultAsync(e => e.Id == id);
        
    }
}