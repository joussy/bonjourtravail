using bonjourtravail_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace bonjourtravail_api.Services;

public class JobService : IJobService
{
    private readonly IMongoCollection<Job> _jobCollection;

    public JobService(
        IOptions<Settings.MongoDatabaseSettings> jobStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            jobStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            jobStoreDatabaseSettings.Value.DatabaseName);

        _jobCollection = mongoDatabase.GetCollection<Job>(
            jobStoreDatabaseSettings.Value.JobCollectionName);
    }

    public async Task<List<Job>> GetAsync() =>
        await _jobCollection.Find(_ => true).ToListAsync();

    public async Task<Job?> GetAsync(string id) =>
        await _jobCollection.Find(x => id.Equals(x.Id)).FirstOrDefaultAsync();

    public async Task InsertAsync(Job newJob) =>
        await _jobCollection.InsertOneAsync(newJob);
    public async Task InsertManyAsync(IEnumerable<Job> newJob) =>
        await _jobCollection.InsertManyAsync(newJob);

    public async Task UpdateAsync(string id, Job updatedJob) =>
        await _jobCollection.ReplaceOneAsync(x => id.Equals(x.Id), updatedJob);

    public async Task DeleteAsync(string id) =>
        await _jobCollection.DeleteOneAsync(x => id.Equals(x.Id));

    public async Task DeleteManyAsync(Expression<Func<Job, bool>> filter) =>
    await _jobCollection.DeleteManyAsync(filter);
}