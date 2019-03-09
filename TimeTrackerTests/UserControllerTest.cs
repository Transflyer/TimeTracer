using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker.Services;
using Xunit;
using Moq;
using TimeTracker.Data.Models;

namespace TimeTracker.Tests
{
    public class UserControllerTest
    {
        #region Properties
        private readonly Mock<RequestUserProvider> mockRequestUserProvider;
        #endregion


        #region Constructor
        public UserControllerTest()
        {
            mockRequestUserProvider = new Mock<RequestUserProvider>();
            ApplicationUser applicationUser = new ApplicationUser()
            {
                DisplayName = "Ivan Petrov",
                Id = 
            }
        }
        #endregion

        [Fact]
        public void Can_Get()
        {

        }
    }
}
