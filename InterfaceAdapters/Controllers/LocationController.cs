using Application.DTO;
using Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace InterfaceAdapters.Controllers;

[Route("api/location")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ILocationService _service;

    public LocationController(ILocationService service)
    {
        _service = service;
    }

    [HttpGet("{locationId}")]
    public async Task<ActionResult<LocationDTO>> GetById(Guid locationId)
    {
        var result = await _service.GetById(locationId);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Guid>>> GetAll()
    {
        var result = await _service.GetAll();
        return result.ToActionResult();
    }

    [HttpGet("/all/details")]
    public async Task<ActionResult<IEnumerable<LocationDTO>>> GetAllWithDetails()
    {
        var result = await _service.GetAllWithDetailsAsync();
        return result.ToActionResult();
    }
}
