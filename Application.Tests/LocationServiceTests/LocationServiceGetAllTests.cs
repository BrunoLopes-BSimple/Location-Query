using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Factories.LocationFactory;
using Domain.Interfaces;
using Domain.IRepository;
using Moq;

namespace Application.Tests.LocationServiceTests;

public class LocationServiceGetAllTests
{
    [Fact]
    public async Task GetAll_WhenGetAllIsCalled_ShouldReturnAllLocationIds()
    {
        // arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var description = "some description";
        var locationIdsList = new List<ILocation> { new Location(id1, description), new Location(id2, description) };

        var expetectList = new List<Guid> { id1, id2 };

        var repoDouble = new Mock<ILocationRepository>();
        repoDouble.Setup(r => r.GetAllAsync()).ReturnsAsync(locationIdsList);
        var factoryDouble = new Mock<ILocationFactory>();
        var mapperDouble = new Mock<IMapper>();

        var service = new LocationService(repoDouble.Object, factoryDouble.Object, mapperDouble.Object);

        // act
        var result = await service.GetAll();

        // assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.NotNull(result.Value);
        Assert.Equal(expetectList, result.Value);
    }

    [Fact]
    public async Task GetAll_WhenThereIsAnError_ThenReturnsInternalServerError()
    {
        // arrange
        var exceptionMessage = "Database connection failed";

        var repoDouble = new Mock<ILocationRepository>();
        repoDouble.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception(exceptionMessage));
        var factoryDouble = new Mock<ILocationFactory>();
        var mapperDouble = new Mock<IMapper>();

        var service = new LocationService(repoDouble.Object, factoryDouble.Object, mapperDouble.Object);

        // act
        var result = await service.GetAll();

        // assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(exceptionMessage, result.Error.Message);
        Assert.Equal(500, result.Error.StatusCode);
    }
}
