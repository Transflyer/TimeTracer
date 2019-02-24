using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Controllers;
using TimeTracker.Data.Models;
using TimeTracker.Services;
using TimeTracker.ViewModels;
using Xunit;
using System.Collections.Generic;

namespace TimeTracker.Tests
{
    public class ProjectControllerTest
    {
        #region Properties

        private Mock<INodeElementRepository> mockNodeElements;
        private Mock<IRequestUserProvider> mockRequestUserProvider;
        private ProjectController controller;

        #endregion Properties

        #region constructor

        public ProjectControllerTest()
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
            controller = new ProjectController(null, null, mockRequestUserProvider.Object, null, mockNodeElements.Object);
        }

        #endregion constructor

        [Fact]
        public async Task Can_Get()
        {
            //Arrange
            var mockId = 5;
            var mockElementToGet = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockNodeElements.Setup(repo => repo.GetNodeElement(mockId)).Returns(Task.FromResult<NodeElement>(mockElementToGet));

            //Act
            var resultType = await controller.Get(mockId);
            ElementViewModel result = (resultType as JsonResult).Value as ElementViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal("T5", result.Title);
        }

        [Fact]
        public async Task Can_Not_Get_With_Wrong_Id()
        {
            //Arrange
            ProjectController controller = new ProjectController(null, null, null, null, mockNodeElements.Object);

            //Act
            var resultType = await controller.Get(10000);
            JsonResult result = resultType as JsonResult;

            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.True(result == null);
        }

        [Fact]
        public async Task Can_Get_Root_List()
        {
            //Arrange
            var mockUserId = "id";
            mockRequestUserProvider.Setup(provider => provider.GetUserId()).Returns(mockUserId);
            var mockElementsToGet = mockNodeElements.Object.NodeElements.Where(u => u.UserId == mockUserId);
            mockNodeElements.Setup(repo => repo.UserNodeElements(mockUserId))
                .Returns(Task.FromResult<IEnumerable<NodeElement>>(mockElementsToGet));


            //Act
            var resultType = await controller.Root(5);
            ElementViewModel[] result = (resultType as JsonResult).Value as ElementViewModel[];

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.True(result.Count() == 5);
            Assert.Equal("T1", result[0].Title);
            Assert.Equal("T3", result[2].Title);
        }

        [Fact]
        public async Task Delete_When_No_Id_Provided()
        {
            //Act
            var resultType = await controller.Delete(null);

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, (resultType as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Delete_When_Wrong_Id_Provided()
        {
            //Arrange
            var mockId = 999;

            //Act
            var result = await controller.Delete(mockId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("{ Error = NodeElement 999 has not been found }", (result as NotFoundObjectResult).Value.ToString());
        }

        [Fact]
        public async Task Delete_When_Right_Id_Provided()
        {
            //Arrange
            var mockId = 9;
            var mockDeletedElement = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockNodeElements.Setup(repo => repo.DeleteNodeElement(mockId)).Returns(Task.FromResult<NodeElement>(mockDeletedElement));

            //Act
            var result = await controller.Delete(mockId);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Can_Put()
        {
            //Arrange
            var mockId = 5;
            var mockUserId = "id";
            mockRequestUserProvider.Setup(provider => provider.GetUserId()).Returns(mockUserId);
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);

            mockNodeElements.Setup(repo => repo.AddUserNodeElement(mockElementToPut, mockElementToPut.UserId))
                .Returns(Task.FromResult<NodeElement>(mockElementToPut));

            //Act
            var resultType = await controller.Put(mockElementToPut);
            var result = (resultType as JsonResult).Value as ElementViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public async Task Put_Without_Model()
        {
            //Act
            var resultType = await controller.Put(null);
            var result = resultType as StatusCodeResult;

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task Post_Without_Model()
        {
            //Act
            var resultType = await controller.Post(null);
            var result = resultType as StatusCodeResult;

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task Can_Post()
        {
            //Arrange
            var mockId = 5;
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);

            mockNodeElements.Setup(repo => repo.UpdateNodeElement(mockElementToPut))
                .Returns(Task.FromResult(mockElementToPut));

            //Act
            var resultType = await controller.Post(mockElementToPut);
            var result = (resultType as JsonResult).Value as ElementViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public async Task Post_Cant_Find_Element()
        {
            //Arrange
            var mockId = 5;
            var mockWrongID = 99;
            var mockElementToPost = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockElementToPost.Id = mockWrongID;
            var mockEnementNotFound = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockWrongID);
            mockNodeElements.Setup(repo => repo.UpdateNodeElement(mockElementToPost))
                .Returns(Task.FromResult(mockEnementNotFound));

            //Act
            var resultType = await controller.Post(mockElementToPost);

            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.Equal("{ Error = NodeElement 99 has not been found }", (resultType as NotFoundObjectResult).Value.ToString());
        }
    }
}