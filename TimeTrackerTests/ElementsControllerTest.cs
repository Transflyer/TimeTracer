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
            var result = (resultType as JsonResult).Value as ElementViewModel;
                        
            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(mockId, result.Id);
        }

        [Fact]
        public async Task Cant_Put_Without_Element()
        {

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
            var result = (resultType as JsonResult).Value as ElementViewModel;

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

        [Fact]
        public async Task Can_Get_Child_Elements()
        {
            //Arrange
            int mockId = 5;
            var mockElementChild1 = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId+1);
            var mockElementChild2 = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId+2);

            mockElementChild1.ParentId = 5;
            mockElementChild2.ParentId = 5;

            var mockArray = new NodeElement[2];
            mockArray[0] = mockElementChild1;
            mockArray[1] = mockElementChild2;

            mockNodeElements.Setup(repo => repo.GetChildElements(mockId))
                .Returns(Task.FromResult(mockArray as IEnumerable<NodeElement>));

            //Act
            var resultType = await controller.GetChildElements(mockId);
            var result = (resultType as JsonResult).Value as ElementViewModel[];

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(2, result.Count());
            Assert.Equal(mockId + 1, result[0].Id);
            Assert.Equal(mockId + 2, result[1].Id);
            Assert.Equal(mockId, result[0].ParentId);
            Assert.Equal(mockId, result[1].ParentId);
        }

        [Fact]
        public async Task Cant_Get_Child_Elements_Without_Id()
        {
            //Act
            var resultType = await controller.GetChildElements(null);
            
            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, (resultType as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Cant_Get_Child_Elements_With_No_Child()
        {
            //Arrange
            int mockId = 5;
            NodeElement[] nodeElements = null;
            mockNodeElements.Setup(repo => repo.GetChildElements(mockId))
                .Returns(Task.FromResult(nodeElements as IEnumerable<NodeElement>));

            //Act
            var resultType = await controller.GetChildElements(mockId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.Equal("{ Error = There are no child elements with 5 parent element }", (resultType as NotFoundObjectResult).Value.ToString());
        }

        [Fact]
        public async Task Can_Get_Parent_Elements()
        {
            //Arrange
            int mockId = 5;
            var mockElementParent1 = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId + 1);
            var mockElementParent2 = mockNodeElements.Object.NodeElements.FirstOrDefault(e => e.Id == mockId + 2);

            var mockArray = new NodeElement[2];
            mockArray[0] = mockElementParent1;
            mockArray[1] = mockElementParent2;

            mockNodeElements.Setup(repo => repo.GetParentElements(mockId))
                .Returns(Task.FromResult(mockArray as IEnumerable<NodeElement>));

            //Act
            var resultType = await controller.GetParentElements(mockId);
            var result = (resultType as JsonResult).Value as ElementViewModel[];

            //Assert
            Assert.IsType<JsonResult>(resultType);
            Assert.Equal(2, result.Count());
            Assert.Equal(mockId + 1, result[0].Id);
            Assert.Equal(mockId + 2, result[1].Id);
           }

        [Fact]
        public async Task Cant_Get_Parent_Elements_Without_Id()
        {
            //Act
            var resultType = await controller.GetParentElements(null);

            //Assert
            Assert.IsType<StatusCodeResult>(resultType);
            Assert.Equal(500, (resultType as StatusCodeResult).StatusCode);
        }

        [Fact]
        public async Task Cant_Get_Parent_Elements_With_No_Child()
        {
            //Arrange
            int mockId = 5;
            NodeElement[] nodeElements = null;
            mockNodeElements.Setup(repo => repo.GetParentElements(mockId))
                .Returns(Task.FromResult(nodeElements as IEnumerable<NodeElement>));

            //Act
            var resultType = await controller.GetParentElements(mockId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(resultType);
            Assert.Equal("{ Error = There are no parent elements chain with 5 child element }", (resultType as NotFoundObjectResult).Value.ToString());
        }


    }
}
