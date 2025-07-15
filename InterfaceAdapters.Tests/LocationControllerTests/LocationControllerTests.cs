using System.Net;
using Application.DTO;
using Infrastructure;
using Infrastructure.DataModels;
using Microsoft.Extensions.DependencyInjection;

namespace InterfaceAdapters.Tests.LocationControllerTests;

public class LocationControllerTests : IntegrationTestBase, IClassFixture<IntegrationTestsWebApplicationFactory<Program>>
{
    private readonly IntegrationTestsWebApplicationFactory<Program> _factory;

    public LocationControllerTests(IntegrationTestsWebApplicationFactory<Program> factory) : base(factory.CreateClient())
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetById_WithExistingLocation_ReturnsOk()
    {
        // arrange
        var id = Guid.NewGuid();
        var description = "some description";

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LocationContext>();
            context.Locations.RemoveRange(context.Locations);
            context.Locations.Add(new LocationDataModel { Id = id, Description = description });
            await context.SaveChangesAsync();
        }

        // act
        var result = await GetAndDeserializeAsync<LocationDTO>($"/api/location/{id}");

        // assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(description, result.Description);
    }

    [Fact]
    public async Task GetById_WithNonExistingLocation_ReturnsNotFound()
    {
        // arrange
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LocationContext>();
            context.Locations.RemoveRange(context.Locations);
            await context.SaveChangesAsync();
        }
        var id = Guid.NewGuid();

        // act
        var response = await GetAsync($"/api/location/{id}");

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenLocationsExist_ReturnsOkWithListOfGuids()
    {
        // arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LocationContext>();
            context.Locations.RemoveRange(context.Locations);
            context.Locations.Add(new LocationDataModel { Id = id1, Description = "Location 1" });
            context.Locations.Add(new LocationDataModel { Id = id2, Description = "Location 2" });
            await context.SaveChangesAsync();
        }

        // act
        var result = await GetAndDeserializeAsync<IEnumerable<Guid>>("/api/location");

        // assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(id1, result);
        Assert.Contains(id2, result);
    }

    [Fact]
    public async Task GetAll_WhenNoLocationsExist_ReturnsOkWithEmptyList()
    {
        // arrange
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LocationContext>();
            context.Locations.RemoveRange(context.Locations);
            await context.SaveChangesAsync();
        }

        // act
        var result = await GetAndDeserializeAsync<IEnumerable<Guid>>("/api/location");

        // assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllWithDetails_WhenLocationsExist_ReturnsOkWithListOfDtos()
    {
        // arrange
        var location1 = new LocationDataModel { Id = Guid.NewGuid(), Description = "Data Center PT" };
        var location2 = new LocationDataModel { Id = Guid.NewGuid(), Description = "Office ES" };

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LocationContext>();
            context.Locations.RemoveRange(context.Locations);
            context.Locations.AddRange(location1, location2);
            await context.SaveChangesAsync();
        }

        // act
        var result = await GetAndDeserializeAsync<IEnumerable<LocationDTO>>("/all/details");

        // assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, l => l.Id == location1.Id && l.Description == location1.Description);
        Assert.Contains(result, l => l.Id == location2.Id && l.Description == location2.Description);
    }

    [Fact]
    public async Task GetAllWithDetails_WhenNoLocationsExist_ReturnsOkWithEmptyList()
    {
        // arrange
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LocationContext>();
            context.Locations.RemoveRange(context.Locations);
            await context.SaveChangesAsync();
        }

        // act
        var result = await GetAndDeserializeAsync<IEnumerable<LocationDTO>>("/all/details");

        // assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}