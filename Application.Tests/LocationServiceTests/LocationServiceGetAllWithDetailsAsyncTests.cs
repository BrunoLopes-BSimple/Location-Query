using Application.DTO;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Factories.LocationFactory;
using Domain.Interfaces;
using Domain.IRepository;
using Moq;

namespace Application.Tests.LocationServiceTests;

public class LocationServiceGetAllWithDetailsAsyncTests
{
    [Fact]
    public async Task GetAllWithDetailsAsync_WhenLocationsExist_ShouldReturnLocationDTOList()
    {
        // arrange
        var locations = new List<ILocation> { new Location(Guid.NewGuid(), "Location 1"), new Location(Guid.NewGuid(), "Location 1") };

        var repoDouble = new Mock<ILocationRepository>();
        repoDouble.Setup(r => r.GetAllAsync()).ReturnsAsync(locations);

        var mapperDouble = new Mock<IMapper>();
        var expectedDTOs = new List<LocationDTO>
        {
            new LocationDTO { Id = locations[0].Id, Description = locations[0].Description },
            new LocationDTO { Id = locations[1].Id, Description = locations[1].Description }
        };
        mapperDouble.Setup(m => m.Map<IEnumerable<LocationDTO>>(locations)).Returns(expectedDTOs);

        var factoryDouble = new Mock<ILocationFactory>();
        var service = new LocationService(repoDouble.Object, factoryDouble.Object, mapperDouble.Object);

        // act
        var result = await service.GetAllWithDetailsAsync();

        // assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedDTOs.Count, result.Value.Count());
        Assert.Equal(expectedDTOs.First().Id, result.Value.First().Id);
        Assert.Equal(expectedDTOs.Last().Description, result.Value.Last().Description);
    }

    [Fact]
    public async Task GetAllWithDetailsAsync_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // arrange
        var exceptionMessage = "Unexpected DB error";

        var repoDouble = new Mock<ILocationRepository>();
        repoDouble.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception(exceptionMessage));

        var factoryDouble = new Mock<ILocationFactory>();
        var mapperDouble = new Mock<IMapper>();

        var service = new LocationService(repoDouble.Object, factoryDouble.Object, mapperDouble.Object);

        // act
        var result = await service.GetAllWithDetailsAsync();

        // assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(exceptionMessage, result.Error.Message);
        Assert.Equal(500, result.Error.StatusCode);
    }


}
