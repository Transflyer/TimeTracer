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

        public IQueryable<TimeSpent> TimeSpents => throw new NotImplementedException();

        public Task<TimeSpent> CreateTimeSpent(TimeSpent timeSpent, long? elementId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TimeSpent>> GetElementTimeSpent(long? elementId, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpent> SetEnd(long? id, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpent> SetStart(long? id, DateTime start)
        {
            throw new NotImplementedException();
        }
    }
}
