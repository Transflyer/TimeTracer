using System;
using Xunit;
using Moq;
using TimeTracker.Controllers;
using TimeTracker.Data.Models;
using System.Collections.Generic;
using System.Linq;


namespace TimeTracker.Tests
{
    public class ProjectControllerTest
    {
        [Fact]
        public void Can_Get()
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                DisplayName = "Pete"
            };

            Mock<INodeElementRepository> mock = new Mock<INodeElementRepository>();
          
        }

        private List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser() { DisplayName = "Pete" },
            new ApplicationUser() { DisplayName = "Kolya" }
        };
    }
}
