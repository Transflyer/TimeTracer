using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Services
{
    public interface IRequestUserProvider
    {
        string GetUserId();
    }
}
