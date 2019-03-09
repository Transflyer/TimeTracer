using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker.Services;
using Xunit;
using Moq;
using TimeTracker.Data.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TimeTracker.Controllers;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.ViewModels;

namespace TimeTracker.Tests
{
    public class UserControllerTest
    {
        #region Properties
        private readonly Mock<IRequestUserProvider> mockRequestUserProvider;
        private ApplicationUser applicationUser;
        private UserController controller;
        private UserViewModel UserViewModel;
        #endregion


        #region Constructor
        public UserControllerTest()
        {
            mockRequestUserProvider = new Mock<IRequestUserProvider>();
            applicationUser = new ApplicationUser()
            {
                DisplayName = "Petrov",
                Id = "0646c26f-e6d7-4f99-b28d-da100af11e12",
                Email = "Ivan@time.top",
                NormalizedUserName = "IVAN PETROV",
                UserName = "Ivan Petrov",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            UserViewModel = new UserViewModel()
            {
                DisplayName = "Petrov",
                Email = "Ivan@time.top",
                UserName = "Ivan Petrov",
                Password = "Pass4Ivan"
            };

            controller = new UserController(null, null, mockRequestUserProvider.Object, null, null);
        }
        #endregion

        [Fact]
        public async Task Can_Get()
        {
            //Arrange
            mockRequestUserProvider.Setup(provider => provider.GetUserAsync()).Returns(Task.FromResult(applicationUser));

            //Act
            var resultType = await controller.Get();
            var result = (resultType as JsonResult).Value as UserViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(applicationUser.DisplayName, result.DisplayName);
            Assert.Equal(applicationUser.Email, result.Email);

        }

        [Fact]
        public async Task Gant_Put_Without_Model()
        {
            //Act
            var resultType = await controller.Put(null);

            //Assert
            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, (resultType as StatusCodeResult).StatusCode);
        }
    }
}
