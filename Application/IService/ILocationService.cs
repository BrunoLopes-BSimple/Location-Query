using Application.DTO;
using Domain.Interfaces;

namespace Application.IService;

public interface ILocationService
{
    public Task<ILocation?> AddLocationReferenceAsync(LocationReference reference);
    public Task<Result<IEnumerable<Guid>>> GetAll();
    public Task<Result<LocationDTO>> GetById(Guid id);
    public Task<Result<IEnumerable<LocationDTO>>> GetAllWithDetailsAsync();
}
