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

    public JobController(JobService jobsService, IPoleEmploiService poleEmploiService)
    {
        _jobService = jobsService;
        _poleEmploiService = poleEmploiService;
    }

    [HttpGet]
    public async Task<List<Job>> Get() =>
        await _jobService.GetAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Job>> Get(string id)
    {
        var job = await _jobService.GetAsync(id);

        if (job is null)
        {
            return NotFound();
        }

        return job;
    }

    [HttpGet("storeOffers")]
    [Produces(typeof(IEnumerable<Job>))]
    public async Task<IActionResult> GetPoleEmploi()
    {
        var jobs = await _poleEmploiService.SearchOffers();
        await _jobService.DeleteManyAsync(x => !x.Internal);
        await _jobService.InsertManyAsync(jobs);

        return Ok(jobs);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Job newJob)
    {
        if (newJob?.Id == null)
        {
            return BadRequest("ID is required");
        }

        await _jobService.InsertAsync(newJob);

        return CreatedAtAction(nameof(Get), new { id = newJob.Id }, newJob);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Job updatedJob)
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var job = await _jobService.GetAsync(id);

        if (job is null)
        {
            return NotFound();
        }

        await _jobService.DeleteAsync(id);

        return NoContent();
    }
}