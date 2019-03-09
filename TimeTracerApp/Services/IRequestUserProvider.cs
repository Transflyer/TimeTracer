using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Data.Models;

namespace TimeTracker.Services
{
    public interface IRequestUserProvider
    {
        string GetUserId();
        Task<ApplicationUser> GetUserAsync();
    }
}
