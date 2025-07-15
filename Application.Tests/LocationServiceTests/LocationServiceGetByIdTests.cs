using Application.Services;
using AutoMapper;
using Domain.Factories.LocationFactory;
using Domain.Interfaces;
using Domain.IRepository;
using Moq;

namespace Application.Tests.LocationServiceTests;

public class LocationServiceGetByIdTests
{
    [Fact]
    public async Task GetAll_WhenGetByIdIsCalled_ShouldReturnLocationDTO()
    {
        // arrange
        var id = Guid.NewGuid();
        var description = "some description";
        var locationDouble = new Mock<ILocation>();

        var repoDouble = new Mock<ILocationRepository>();
        repoDouble.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(locationDouble.Object);

        var factoryDouble = new Mock<ILocationFactory>();
        var mapperDouble = new Mock<IMapper>();

        var service = new LocationService(repoDouble.Object, factoryDouble.Object, mapperDouble.Object);

        // act
        var result = await service.GetById(id);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.NotNull(result.Value);
        Assert.Equal(locationDouble.Object.Id, result.Value.Id);
        Assert.Equal(locationDouble.Object.Description, result.Value.Description);
    }

    [Fact]
    public async Task GetAll_WhenThereIsAnError_ShouldReturnInternalServerError()
    {
        // arrange
        var exceptionMessage = "Database connection failed";
        var id = Guid.NewGuid();

        var repoDouble = new Mock<ILocationRepository>();
        repoDouble.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception(exceptionMessage));
        var factoryDouble = new Mock<ILocationFactory>();
        var mapperDouble = new Mock<IMapper>();

        var service = new LocationService(repoDouble.Object, factoryDouble.Object, mapperDouble.Object);

        // act
        var result = await service.GetById(id);

        // assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(exceptionMessage, result.Error.Message);
        Assert.Equal(500, result.Error.StatusCode);
    }
}
