using bonjourtravail_api.Models;
using bonjourtravail_api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace bonjourtravail_api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobController : ControllerBase
{
    private readonly JobService _jobService;
    private readonly IPoleEmploiService _poleEmploiService;
    private readonly IMongoClient _mongoClient;

    public JobController(JobService jobsService, IPoleEmploiService poleEmploiService, IMongoClient mongoClient)
    {
        _jobService = jobsService;
        _poleEmploiService = poleEmploiService;
        _mongoClient = mongoClient;
    }

    [HttpGet]
    public async Task<List<Offre>> Get() =>
        await _jobService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Offre>> Get(string id)
    {
        var job = await _jobService.GetAsync(id);

        if (job is null)
        {
            return NotFound();
        }

        return job;
    }

    [HttpGet("storeOffers")]
    [Produces(typeof(IEnumerable<Offre>))]
    public async Task<IActionResult> GetPoleEmploi()
    {
        var jobs = await _poleEmploiService.SearchOffers();
        await _mongoClient.GetDatabase("BonjourTravail").GetCollection<Offre>("Jobs").InsertManyAsync(jobs);

        return Ok(jobs);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Offre newJob)
    {
        await _jobService.CreateAsync(newJob);

        return CreatedAtAction(nameof(Get), new { id = newJob.Id }, newJob);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Offre updatedJob)
    {
        var job = await _jobService.GetAsync(id);

        if (job is null)
        {
            return NotFound();
        }

        updatedJob.Id = job.Id;

        await _jobService.UpdateAsync(id, updatedJob);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var job = await _jobService.GetAsync(id);

        if (job is null)
        {
            return NotFound();
        }

        await _jobService.RemoveAsync(id);

        return NoContent();
    }
}