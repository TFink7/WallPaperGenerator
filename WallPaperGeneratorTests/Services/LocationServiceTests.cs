using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WallPaperGenerator.Models;
using WallPaperGenerator.Services;
using Newtonsoft.Json;

[TestClass]
public class LocationServiceTests
{
    private Mock<IHttpClientFactory> _mockHttpClientFactory;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private LocationService _locationService;
    private string _validApiKey = "testipgeoapikey";
    
    [TestInitialize]
    public void SetUp()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();

        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_mockHttpMessageHandler.Object));

        _locationService = new LocationService(_mockHttpClientFactory.Object);
        Environment.SetEnvironmentVariable("IPGEOLOCATION_API_KEY", _validApiKey);
    }

    [TestMethod]
    public async Task GetCurrentLocationAsync_ReturnsLocationData_WhenApiCallSucceeds()
    {
        var fakeResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"city\":\"TestCity\",\"country_name\":\"TestCountry\", \"latitude\":\"43.6532\", \"longitude\":\"-79.3832\"}")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(fakeResponseMessage);

        var result = await _locationService.GetCurrentLocationAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual("TestCity", result.City);
        Assert.AreEqual("TestCountry", result.Country); 
                                                        
                                                        
        Assert.AreEqual("43.6532", result.Latitude);
        Assert.AreEqual("-79.3832", result.Longitude);
    }

    [TestMethod]
    public async Task GetCurrentLocationAsync_ReturnsNull_WhenApiResponseIsNotFound()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await _locationService.GetCurrentLocationAsync();

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetCurrentLocationAsync_ReturnsNull_WhenApiResponseHasInvalidData()
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

        var result = await _locationService.GetCurrentLocationAsync();

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetCurrentLocationAsync_ReturnsNull_WhenRequestFails()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        var result = await _locationService.GetCurrentLocationAsync();

        Assert.IsNull(result);
    }
}
