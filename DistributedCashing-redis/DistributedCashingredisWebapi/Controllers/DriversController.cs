using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DistributedCashingredisWebapi;

[ApiController]
[Route("api/v1/[Controller]")]
public class DriversController : ControllerBase
{

    private readonly AppDbContext _context ; 
    private readonly ICacheService _cacheService ;

    public DriversController(AppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Driver>),(int)HttpStatusCode.OK)]
    public async Task<ActionResult> Get()
    {
        var CacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");
        if(CacheData !=null && CacheData.Count() > 0)
            return Ok(CacheData);
        
        var expiryTime = DateTime.Now.AddSeconds(30);
        CacheData=  await _context.Drivers.ToListAsync();
        _cacheService.SetData<IEnumerable<Driver>>("drivers",CacheData,expiryTime);
        return Ok(CacheData);


    }   

    [HttpPost]
    [ProducesResponseType(typeof(Driver),(int)HttpStatusCode.OK)]
    public async Task<ActionResult<Driver>> Post([FromBody] Driver value )
    {
        var addedObj = await _context.Drivers.AddAsync(value);

        var expiryTime = DateTime.Now.AddSeconds(30);

        _cacheService.SetData<Driver>($"driver{value.Id}",addedObj.Entity,expiryTime);

        await _context.SaveChangesAsync();

        return Ok(addedObj.Entity);

    }

    [HttpDelete]
    public async Task<ActionResult> Delete(int id )
    {
        var existObj = await _context.Drivers.FirstOrDefaultAsync(x=>x.Id == id);

        if(existObj != null)
        {
             _context.Drivers.Remove(existObj);
             _cacheService.RemoveData($"driver{existObj.Id}");
             _context.SaveChangesAsync();
             return NoContent();
        }

        return NotFound();
           




    }

}