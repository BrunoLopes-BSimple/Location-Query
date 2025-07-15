using Application.DTO;
using Application.IService;
using AutoMapper;
using Domain.Factories.LocationFactory;
using Domain.Interfaces;
using Domain.IRepository;

namespace Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepo;
    private readonly ILocationFactory _locationFactory;
    private readonly IMapper _mapper;

    public LocationService(ILocationRepository repository, ILocationFactory factory, IMapper mapper)
    {
        _locationRepo = repository;
        _locationFactory = factory;
        _mapper = mapper;
    }

    public async Task<ILocation?> AddLocationReferenceAsync(LocationReference reference)
    {
        var locationAlreadyExists = await _locationRepo.AlreadyExists(reference.Id);
        if (locationAlreadyExists) return null;

        var newLocation = _locationFactory.Create(reference.Id, reference.Description);
        return await _locationRepo.AddAsync(newLocation);
    }

    public async Task<Result<IEnumerable<Guid>>> GetAll()
    {
        try
        {
            var locations = await _locationRepo.GetAllAsync();
            return Result<IEnumerable<Guid>>.Success(locations.Select(l => l.Id));
        }
        catch (Exception e)
        {
            return Result<IEnumerable<Guid>>.Failure(Error.InternalServerError(e.Message));
        }
    }

    public async Task<Result<LocationDTO>> GetById(Guid id)
    {
        try
        {
            var location = await _locationRepo.GetByIdAsync(id);
            if (location == null) return Result<LocationDTO>.Failure(Error.NotFound("Location not found"));

            var result = new LocationDTO { Id = location.Id, Description = location.Description };

            return Result<LocationDTO>.Success(result);
        }
        catch (Exception e)
        {
            return Result<LocationDTO>.Failure(Error.InternalServerError(e.Message));
        }
    }

    public async Task<Result<IEnumerable<LocationDTO>>> GetAllWithDetailsAsync()
    {
        try
        {
            var locations = await _locationRepo.GetAllAsync();
            var result = _mapper.Map<IEnumerable<LocationDTO>>(locations);
            return Result<IEnumerable<LocationDTO>>.Success(result);
        }
        catch (Exception e)
        {
            return Result<IEnumerable<LocationDTO>>.Failure(Error.InternalServerError(e.Message));
        }
    }
}
