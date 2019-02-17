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


namespace TimeTracker.Tests
{
    public class ProjectControllerTest
    {
        #region Properties

        private Mock<INodeElementRepository> mockNodeElements;
        private Mock<IRequestUserProvider> mockRequestUserProvider;
        private ProjectController controller;

        #endregion

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
        #endregion

       
        [Fact]
        public void Can_Get()
        {
            //Arrange
            ProjectController controller = new ProjectController(null, null, null, null, mockNodeElements.Object);

            //Act
            var resultType = controller.Get(3);
            ProjectViewModel result = (resultType as JsonResult).Value as ProjectViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal("T3", result.Title);
        }

        [Fact]
        public void Can_Not_Get_With_Wrong_Id()
        {
            //Arrange
            ProjectController controller = new ProjectController(null, null, null, null, mockNodeElements.Object);

            //Act
            var resultType = controller.Get(10000);
            JsonResult result = resultType as JsonResult;

            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.True(result == null);
        }

        [Fact]
        public void Can_Get_Root_List()
        {
            //Arrange
            var mockUserId = "id";
            mockRequestUserProvider.Setup(provider => provider.GetUserId()).Returns(mockUserId);

            //Act
            var resultType = controller.Root(5);
            ProjectViewModel [] result = (resultType as JsonResult).Value as ProjectViewModel[];

            //Assert
            Assert.IsType<JsonResult> (resultType);
            Assert.True(result.Count() == 5);
            Assert.Equal("T1", result[0].Title);
            Assert.Equal("T3", result[2].Title);
        }

        [Fact]
        public void Delete_When_No_Id_Provided()
        {
            //Act
            var result = controller.Delete(null);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("{ Error = NodeElement  has not been found }", (result as NotFoundObjectResult).Value.ToString());
        }

        [Fact]
        public void Delete_When_Wrong_Id_Provided()
        {
            //Arrange
            var mockId = 999;

            //Act
            var result = controller.Delete(mockId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("{ Error = NodeElement 999 has not been found }", (result as NotFoundObjectResult).Value.ToString());

        }

        [Fact]
        public void Delete_When_Right_Id_Provided()
        {
            //Arrange
            var mockId = 9;
            var mockDeletedElement = mockNodeElements.Object.NodeElements.FirstOrDefault(e=>e.Id == mockId);
            mockNodeElements.Setup(repo => repo.DeleteNodeElement(mockId)).Returns(mockDeletedElement);

            //Act
            var result = controller.Delete(mockId);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Can_Put()
        {
            //Arrange
            var mockId = 5;
            var mockUserId = "id";
            mockRequestUserProvider.Setup(provider => provider.GetUserId()).Returns(mockUserId);
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockNodeElements.Setup(repo => repo.AddUserNodeElement(mockElementToPut, mockElementToPut.UserId))
                .Returns(mockElementToPut);

            //Act
            var resultType = controller.Put(mockElementToPut);
            var result = (resultType as JsonResult).Value as ProjectViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public void Put_Without_Model()
        {
            //Act
            var resultType = controller.Put(null);
            var result = resultType as StatusCodeResult;

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public void Post_Without_Model()
        {
            //Act
            var resultType = controller.Post(null);
            var result = resultType as StatusCodeResult;

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public void Can_Post()
        {
            //Arrange
            var mockId = 5;
            var mockElementToPut = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockNodeElements.Setup(repo => repo.UpdateNodeElement(mockElementToPut))
                .Returns(mockElementToPut);

            //Act
            var resultType = controller.Post(mockElementToPut);
            var result = (resultType as JsonResult).Value as ProjectViewModel;

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public void Post_Cant_Find_Element()
        {
            //Arrange
            var mockId = 5;
            var mockWrongID = 99;
            var mockElementToPost = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId);
            mockElementToPost.Id = mockWrongID;
            var mockEnementNotFound = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockWrongID);
            mockNodeElements.Setup(repo => repo.UpdateNodeElement(mockElementToPost))
                .Returns(mockEnementNotFound);

            //Act
            var resultType = controller.Post(mockElementToPost);
           
            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.Equal("{ Error = NodeElement 99 has not been found }", (resultType as NotFoundObjectResult).Value.ToString());
        }

    }
}







