using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WallPaperGenerator.Models;
using WallPaperGenerator.Services;

[TestClass]
public class WeatherServiceTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private WeatherService _weatherService;
    private string _validApiKey = "testapikey";

    [TestInitialize]
    public void SetUp()
    {
        // Setup mocks and service instance before each test
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();

        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_mockHttpMessageHandler.Object));

        _weatherService = new WeatherService(_mockHttpClientFactory.Object);
        Environment.SetEnvironmentVariable("WEATHER_API_KEY", _validApiKey);
    }


    [TestMethod]
    public async Task GetWeatherAsync_ReturnsWeatherData_WhenApiCallSucceeds()
    {
        // Arrange
        var fakeResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"location\":{\"name\":\"TestCity\"},\"current\":{\"temp_c\":20,\"condition\":{\"text\":\"Sunny\"}}}")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(fakeResponseMessage);

        // Act
        var result = await _weatherService.GetWeatherAsync("TestCity");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Sunny", result.Condition);
        Assert.AreEqual(20, result.TemperatureCelsius);
    }


    [TestMethod]
    public async Task GetWeatherAsync_ReturnsNull_WhenApiResponseIsNotFound()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act
        var result = await _weatherService.GetWeatherAsync("InvalidLocation");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsNull_WhenApiResponseHasInvalidData()
    {
        // Arrange
        var invalidContent = "Invalid Content";
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(invalidContent)
            });

        // Act
        var result = await _weatherService.GetWeatherAsync("TestCity");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsNull_WhenRequestFails()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        // Act
        var result = await _weatherService.GetWeatherAsync("TestCity");

        // Assert
        Assert.IsNull(result);
    }

}