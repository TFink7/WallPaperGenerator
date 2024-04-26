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
    private LocationData _testLocationData;

    [TestInitialize]
    public void SetUp()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();

        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_mockHttpMessageHandler.Object));

        _weatherService = new WeatherService(_mockHttpClientFactory.Object);
        Environment.SetEnvironmentVariable("WEATHER_API_KEY", _validApiKey);

        _testLocationData = new LocationData { City = "TestCity", Country = "TestCountry", Latitude = "43.6532", Longitude = "-79.3832" };
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsWeatherData_WhenApiCallSucceeds()
    {
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

        var result = await _weatherService.GetWeatherAsync(_testLocationData);

        Assert.IsNotNull(result);
        Assert.AreEqual("Sunny", result.Condition);
        Assert.AreEqual(20, result.TemperatureCelsius);
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsNull_WhenApiResponseIsNotFound()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await _weatherService.GetWeatherAsync(_testLocationData);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsNull_WhenApiResponseHasInvalidData()
    {
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

        var result = await _weatherService.GetWeatherAsync(_testLocationData);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsNull_WhenRequestFails()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        var result = await _weatherService.GetWeatherAsync(_testLocationData);

        Assert.IsNull(result);
    }
}

