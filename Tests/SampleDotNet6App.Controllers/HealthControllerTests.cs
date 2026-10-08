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

        // ── GetHealth ─────────────────────────────────────────────────────────

        [Fact]
        public void GetHealth_ReturnsOkResult()
        {
            var result = _controller.GetHealth();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetHealth_ReturnsNonNullValue()
        {
            var result = _controller.GetHealth() as OkObjectResult;

            Assert.NotNull(result);
            Assert.NotNull(result!.Value);
        }

        [Fact]
        public void GetHealth_ResponseContainsStatusHealthy()
        {
            var result = _controller.GetHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            Assert.NotNull(value);

            // Use reflection to check anonymous type properties
            var statusProp = value!.GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp!.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetHealth_ResponseContainsVersion()
        {
            var result = _controller.GetHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var versionProp = value!.GetType().GetProperty("version");
            Assert.NotNull(versionProp);
            Assert.Equal("1.0.0", versionProp!.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetHealth_ResponseContainsTimestamp()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var result = _controller.GetHealth() as OkObjectResult;
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.NotNull(result);
            var value = result!.Value;
            var timestampProp = value!.GetType().GetProperty("timestamp");
            Assert.NotNull(timestampProp);

            var timestamp = (DateTime)timestampProp!.GetValue(value)!;
            Assert.True(timestamp >= before && timestamp <= after);
        }

        [Fact]
        public void GetHealth_ResponseContainsEnvironment()
        {
            var result = _controller.GetHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var envProp = value!.GetType().GetProperty("environment");
            Assert.NotNull(envProp);
            // Environment value should not be null
            Assert.NotNull(envProp!.GetValue(value));
        }

        // ── GetDetailedHealth ─────────────────────────────────────────────────

        [Fact]
        public void GetDetailedHealth_ReturnsOkResult()
        {
            var result = _controller.GetDetailedHealth();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetDetailedHealth_ReturnsNonNullValue()
        {
            var result = _controller.GetDetailedHealth() as OkObjectResult;

            Assert.NotNull(result);
            Assert.NotNull(result!.Value);
        }

        [Fact]
        public void GetDetailedHealth_ResponseContainsStatusHealthy()
        {
            var result = _controller.GetDetailedHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var statusProp = value!.GetType().GetProperty("status");
            Assert.NotNull(statusProp);
            Assert.Equal("Healthy", statusProp!.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetDetailedHealth_ResponseContainsFramework()
        {
            var result = _controller.GetDetailedHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var frameworkProp = value!.GetType().GetProperty("framework");
            Assert.NotNull(frameworkProp);
            Assert.NotNull(frameworkProp!.GetValue(value));
        }

        [Fact]
        public void GetDetailedHealth_ResponseContainsOs()
        {
            var result = _controller.GetDetailedHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var osProp = value!.GetType().GetProperty("os");
            Assert.NotNull(osProp);
            Assert.NotNull(osProp!.GetValue(value));
        }

        [Fact]
        public void GetDetailedHealth_ResponseContainsChecks()
        {
            var result = _controller.GetDetailedHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var checksProp = value!.GetType().GetProperty("checks");
            Assert.NotNull(checksProp);
            var checks = checksProp!.GetValue(value) as Array;
            Assert.NotNull(checks);
            Assert.Equal(2, checks!.Length);
        }

        [Fact]
        public void GetDetailedHealth_ResponseContainsVersion()
        {
            var result = _controller.GetDetailedHealth() as OkObjectResult;
            Assert.NotNull(result);

            var value = result!.Value;
            var versionProp = value!.GetType().GetProperty("version");
            Assert.NotNull(versionProp);
            Assert.Equal("1.0.0", versionProp!.GetValue(value)?.ToString());
        }

        [Fact]
        public void GetDetailedHealth_ResponseContainsTimestamp()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var result = _controller.GetDetailedHealth() as OkObjectResult;
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.NotNull(result);
            var value = result!.Value;
            var timestampProp = value!.GetType().GetProperty("timestamp");
            Assert.NotNull(timestampProp);

            var timestamp = (DateTime)timestampProp!.GetValue(value)!;
            Assert.True(timestamp >= before && timestamp <= after);
        }
    }
}
