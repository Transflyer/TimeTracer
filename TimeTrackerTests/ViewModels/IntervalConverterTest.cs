using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Controllers;
using TimeTracker.Data.Models;
using TimeTracker.Services;
using TimeTracker.ViewModels;
using TimeTracker.ViewModels.Service;
using Xunit;

namespace TimeTracker.Tests
{
    public class IntervalConverterTest
    {
        [Fact]
        public void Result_IntervalConverter()
        {
            //Arrange
            List<IntervalViewModel> intervalViewModelList = new List<IntervalViewModel>() {
                new IntervalViewModel(){
                    TotalSecond = 2000
                },
                new IntervalViewModel()
                {
                    TotalSecond = 452521
                },
                new IntervalViewModel()
                {
                    TotalSecond = 500
                }
            };

            //Act
            var result = IntervalConverter.FillDHMSProp(intervalViewModelList).ToArray();
            var resultSingleCase = IntervalConverter.FillDHMSProp(intervalViewModelList[0]);

            //Assert
            Assert.Equal(0, result[0].Days);
            Assert.Equal(0, result[0].Hours);
            Assert.Equal(33, result[0].Minutes);
            Assert.Equal(20, result[0].Seconds);
            Assert.Equal(5, result[1].Days);
            Assert.Equal(5, result[1].Hours);
            Assert.Equal(42, result[1].Minutes);
            Assert.Equal(1, result[1].Seconds);
            Assert.Equal(0, resultSingleCase.Days);
            Assert.Equal(0, resultSingleCase.Hours);
            Assert.Equal(33, resultSingleCase.Minutes);
            Assert.Equal(20, resultSingleCase.Seconds);
        }
    }
}
