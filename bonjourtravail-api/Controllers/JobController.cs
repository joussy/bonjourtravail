using bonjourtravail_api.Models;
using bonjourtravail_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace bonjourtravail_api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobController : ControllerBase
{
    private readonly JobService _jobService;

    public JobController(JobService jobsService) =>
        _jobService = jobsService;

    [HttpGet]
    public async Task<List<Job>> Get() =>
        await _jobService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Job>> Get(string id)
    {
        var job = await _jobService.GetAsync(id);

        if (job is null)
        {
            return NotFound();
        }

        return job;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Job newJob)
    {
        await _jobService.CreateAsync(newJob);

        return CreatedAtAction(nameof(Get), new { id = newJob.Id }, newJob);
    }

    [HttpPut("{id:length(24)}")]
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