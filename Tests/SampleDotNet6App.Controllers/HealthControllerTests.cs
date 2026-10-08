using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;

namespace SampleDotNet6App.Controllers.Tests
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

        // ─── Get ──────────────────────────────────────────────────────────────────

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
            var value = result!.Value;
            Assert.NotNull(value);

            // Use reflection to check anonymous type properties
            var statusProp = value!.GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp!.GetValue(value));
        }

        [Fact]
        public void Get_ReturnsVersion100()
        {
            // Act
            var result = _controller.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var versionProp = value!.GetType().GetProperty("version");
            Assert.NotNull(versionProp);
            Assert.Equal("1.0.0", versionProp!.GetValue(value));
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
            var value = result!.Value;
            var timestampProp = value!.GetType().GetProperty("timestamp");
            Assert.NotNull(timestampProp);
            var timestamp = (DateTime)timestampProp!.GetValue(value)!;
            Assert.True(timestamp >= before);
        }

        // ─── GetDetailed ──────────────────────────────────────────────────────────

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
            var value = result!.Value;
            var statusProp = value!.GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp!.GetValue(value));
        }

        [Fact]
        public void GetDetailed_ReturnsVersion100()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var versionProp = value!.GetType().GetProperty("version");
            Assert.NotNull(versionProp);
            Assert.Equal("1.0.0", versionProp!.GetValue(value));
        }

        [Fact]
        public void GetDetailed_ReturnsFrameworkDescription()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var frameworkProp = value!.GetType().GetProperty("framework");
            Assert.NotNull(frameworkProp);
            var framework = frameworkProp!.GetValue(value) as string;
            Assert.NotNull(framework);
            Assert.NotEmpty(framework!);
        }

        [Fact]
        public void GetDetailed_ReturnsMachineName()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var machineNameProp = value!.GetType().GetProperty("machineName");
            Assert.NotNull(machineNameProp);
            var machineName = machineNameProp!.GetValue(value) as string;
            Assert.NotNull(machineName);
            Assert.NotEmpty(machineName!);
        }

        [Fact]
        public void GetDetailed_ReturnsProcessorCount()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var processorCountProp = value!.GetType().GetProperty("processorCount");
            Assert.NotNull(processorCountProp);
            var processorCount = (int)processorCountProp!.GetValue(value)!;
            Assert.True(processorCount >= 1);
        }

        [Fact]
        public void GetDetailed_ReturnsTimestamp()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var timestampProp = value!.GetType().GetProperty("timestamp");
            Assert.NotNull(timestampProp);
            var timestamp = (DateTime)timestampProp!.GetValue(value)!;
            Assert.True(timestamp >= before);
        }

        [Fact]
        public void GetDetailed_ReturnsEnvironment()
        {
            // Act
            var result = _controller.GetDetailed() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result!.Value;
            var envProp = value!.GetType().GetProperty("environment");
            Assert.NotNull(envProp);
            var env = envProp!.GetValue(value) as string;
            Assert.NotNull(env);
            Assert.NotEmpty(env!);
        }
    }
}
