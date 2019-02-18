using System;
using Xunit;
using Moq;
using TimeTracker.Controllers;
using TimeTracker.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.ViewModels;
using TimeTracker.Services;
using System.Threading.Tasks;
using System.Threading;

namespace TimeTracker.Tests
{
    public class ElementsControllerTest
    {
        #region Properties
        private Mock<INodeElementRepository> mockNodeElements;
        private Mock<IRequestUserProvider> mockRequestUserProvider;
        private ElementsController controller;
        #endregion

        #region constructor
        public ElementsControllerTest()
        {
            //Common arrange
            mockNodeElements = new Mock<INodeElementRepository>();
            mockRequestUserProvider = new Mock<IRequestUserProvider>();

            //Number of elements
            const int t = 9;
            mockNodeElements.Setup(p => p.NodeElements).Returns(() =>
            {
                NodeElement[] elements = new NodeElement[t];
                for (int i = 0; i < t; i++)
                {
                    var y = i + 1;
                    elements[i] = new NodeElement()
                    {
                        Id = y,
                        Title = "T" + y,
                        Description = "D" + y,
                        UserId = "id",
                        CreatedDate = DateTime.UtcNow,
                    };
                }
                return elements.AsQueryable<NodeElement>();
            });
            controller = new ElementsController(null, null, mockRequestUserProvider.Object, null, mockNodeElements.Object);
        }
        #endregion

        [Fact]
        public async Task Can_Put()
        {
            //Arrange
            var mockId = 5;
            var mockParentId = 6;
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockElementToPut.ParentId = mockParentId;

            mockNodeElements.Setup(repo => repo.AddChildElement(mockElementToPut, mockElementToPut.ParentId))
                .Returns(Task.FromResult(mockElementToPut));

            //Act
            var resultType = await controller.Put(mockElementToPut);
            var result = (resultType as JsonResult).Value as ProjectViewModel;
                        
            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public async Task Cant_Put_Without_Element()
        {
            //Arrange
            var mockParentId = 6;

            //Act
            var resultType = await controller.Put(null);
            
            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, (resultType as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Cant_Put_Without_ParentId()
        {
            //Arrange
            var mockId = 5;
            int? mockParentId = null;
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockElementToPut.ParentId = mockParentId;

            //Act
            var resultType = await controller.Put(mockElementToPut);

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, (resultType as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Can_Get()
        {
            //Arrange
            var mockId = 5;
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);

            mockNodeElements.Setup(repo => repo.GetNodeElement(mockId))
                .Returns(Task.FromResult(mockElementToPut));

            //Act
            var resultType = await controller.Get(mockId);
            var result = (resultType as JsonResult).Value as ProjectViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public async Task Cant_Get_With_Wrong_Id()
        {
            //Arrange
            var mockId = 99;
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);

            mockNodeElements.Setup(repo => repo.GetNodeElement(mockId))
                .Returns(Task.FromResult(mockElementToPut));

            //Act
            var resultType = await controller.Get(mockId);
                        
            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.Equal("{ Error = NodeElement 99 has not been found }", (resultType as NotFoundObjectResult).Value.ToString());
        }

    }
}
