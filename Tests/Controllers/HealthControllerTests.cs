using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;

namespace SampleDotNet6App.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Mock<ILogger<HealthController>> _mockLogger;
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            _mockLogger = new Mock<ILogger<HealthController>>();
            _controller = new HealthController(_mockLogger.Object);
        }

        // ─── Get ───────────────────────────────────────────────────────────────────

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

            var statusProp = value.GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void Get_ReturnsVersionInfo()
        {
            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var versionProp = value.GetType().GetProperty("version");
            Assert.NotNull(versionProp);
            Assert.Equal("10.0", versionProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void Get_ReturnsTimestamp()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var result = _controller.Get() as OkObjectResult;
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var timestampProp = value.GetType().GetProperty("timestamp");
            Assert.NotNull(timestampProp);
            var timestamp = (DateTime)timestampProp.GetValue(value)!;
            Assert.True(timestamp >= before && timestamp <= after);
        }

        [Fact]
        public void Get_ReturnsFrameworkDescription()
        {
            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var frameworkProp = value.GetType().GetProperty("framework");
            Assert.NotNull(frameworkProp);
            Assert.NotNull(frameworkProp.GetValue(value));
        }

        // ─── GetDetailed ───────────────────────────────────────────────────────────

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

            var statusProp = value.GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetDetailed_ReturnsVersionInfo()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var versionProp = value.GetType().GetProperty("version");
            Assert.NotNull(versionProp);
            Assert.Equal("10.0", versionProp.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetDetailed_ReturnsOsDescription()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var osProp = value.GetType().GetProperty("os");
            Assert.NotNull(osProp);
            Assert.NotNull(osProp.GetValue(value));
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

            var machineNameProp = value.GetType().GetProperty("machineName");
            Assert.NotNull(machineNameProp);
            Assert.Equal(Environment.MachineName, machineNameProp.GetValue(value)?.ToString());
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

            var processorCountProp = value.GetType().GetProperty("processorCount");
            Assert.NotNull(processorCountProp);
            Assert.Equal(Environment.ProcessorCount, (int)processorCountProp.GetValue(value)!);
        }

        [Fact]
        public void GetDetailed_ReturnsEnvironmentInfo()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;
            Assert.NotNull(value);

            var envProp = value.GetType().GetProperty("environment");
            Assert.NotNull(envProp);
            Assert.NotNull(envProp.GetValue(value));
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

            var workingSetProp = value.GetType().GetProperty("workingSet");
            Assert.NotNull(workingSetProp);
            Assert.True((long)workingSetProp.GetValue(value)! > 0);
        }
    }
}
