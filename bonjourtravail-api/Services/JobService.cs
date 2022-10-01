using bonjourtravail_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace bonjourtravail_api.Services;

public class JobService
{
    private readonly IMongoCollection<Offre> _jobCollection;

    public JobService(
        IOptions<Settings.MongoDatabaseSettings> jobStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            jobStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            jobStoreDatabaseSettings.Value.DatabaseName);

        _jobCollection = mongoDatabase.GetCollection<Offre>(
            jobStoreDatabaseSettings.Value.JobCollectionName);
    }

    public async Task<List<Offre>> GetAsync() =>
        await _jobCollection.Find(_ => true).ToListAsync();

    public async Task<Offre?> GetAsync(string id) =>
        await _jobCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Offre newJob) =>
        await _jobCollection.InsertOneAsync(newJob);

    public async Task UpdateAsync(string id, Offre updatedJob) =>
        await _jobCollection.ReplaceOneAsync(x => x.Id == id, updatedJob);

    public async Task RemoveAsync(string id) =>
        await _jobCollection.DeleteOneAsync(x => x.Id == id);
}