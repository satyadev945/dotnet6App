using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;

namespace SampleDotNet6App.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Mock<ILogger<HealthController>> _loggerMock;
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            _loggerMock = new Mock<ILogger<HealthController>>();
            _controller = new HealthController(_loggerMock.Object);
        }

        // ---- Get Tests ----
        [Fact]
        public void Get_ReturnsOkResult()
        {
            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Get_ReturnsHealthyStatus()
        {
            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            // Use reflection to check anonymous type properties
            var statusProp = value.GetType().GetProperty("Status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void Get_ReturnsCorrectVersion()
        {
            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var versionProp = value.GetType().GetProperty("Version");
            Assert.NotNull(versionProp);
            Assert.Equal("1.0.0", versionProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void Get_ReturnsCorrectFramework()
        {
            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var frameworkProp = value.GetType().GetProperty("Framework");
            Assert.NotNull(frameworkProp);
            Assert.Equal(".NET 8", frameworkProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void Get_ReturnsTimestamp()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var timestampProp = value.GetType().GetProperty("Timestamp");
            Assert.NotNull(timestampProp);
            var timestamp = (DateTime)timestampProp.GetValue(value)!;
            Assert.True(timestamp >= before);
        }

        // ---- GetDetailed Tests ----
        [Fact]
        public void GetDetailed_ReturnsOkResult()
        {
            // Act
            var result = _controller.GetDetailed();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetDetailed_ReturnsHealthyStatus()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var statusProp = value.GetType().GetProperty("Status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetDetailed_ReturnsMachineName()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var machineNameProp = value.GetType().GetProperty("MachineName");
            Assert.NotNull(machineNameProp);
            Assert.NotNull(machineNameProp.GetValue(value));
        }

        [Fact]
        public void GetDetailed_ReturnsProcessorCount()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var processorCountProp = value.GetType().GetProperty("ProcessorCount");
            Assert.NotNull(processorCountProp);
            var processorCount = (int)processorCountProp.GetValue(value)!;
            Assert.True(processorCount > 0);
        }

        [Fact]
        public void GetDetailed_ReturnsWorkingSet()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var workingSetProp = value.GetType().GetProperty("WorkingSet");
            Assert.NotNull(workingSetProp);
            var workingSet = (long)workingSetProp.GetValue(value)!;
            Assert.True(workingSet >= 0);
        }

        [Fact]
        public void GetDetailed_ReturnsUptime()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var uptimeProp = value.GetType().GetProperty("Uptime");
            Assert.NotNull(uptimeProp);
            var uptime = (long)uptimeProp.GetValue(value)!;
            Assert.True(uptime >= 0);
        }

        [Fact]
        public void GetDetailed_ReturnsEnvironment()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var envProp = value.GetType().GetProperty("Environment");
            Assert.NotNull(envProp);
            Assert.NotNull(envProp.GetValue(value));
        }
    }
}
