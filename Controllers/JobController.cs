using bonjourtravail_api.Models;
using bonjourtravail_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace bonjourtravail_api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IPoleEmploiService _poleEmploiService;

    public JobController(IJobService jobsService, IPoleEmploiService poleEmploiService)
    {
        _jobService = jobsService;
        _poleEmploiService = poleEmploiService;
    }

    /// <summary>
    /// Retourne la totalité des offres d'emploi stockées en BDD
    /// </summary>
    [HttpGet]
    public async Task<List<Job>> Get() =>
        await _jobService.GetAsync();

    /// <summary>
    /// Retourne une offre d'emploi
    /// </summary>
    /// <param name="id">Identifiant unique de l'offre d'emploi</param>
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

    /// <summary>
    /// Met a jour la BDD avec la totalité des offres provenant de Pole-Emploi
    /// </summary>
    [HttpGet("fetchPoleEmploi")]
    [Produces(typeof(IEnumerable<Job>))]
    public async Task<IActionResult> GetPoleEmploi()
    {
        var jobs = await _poleEmploiService.SearchOffers();
        await _jobService.DeleteManyAsync(x => !x.Internal);
        await _jobService.InsertManyAsync(jobs);

        return Ok(jobs);
    }

    /// <summary>
    /// Ajoute une offre d'emploi
    /// </summary>
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

    /// <summary>
    /// Supprime une offre d'emploi
    /// </summary>
    /// <param name="id">Identifiant unique de l'offre d'emploi</param>
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