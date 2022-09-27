﻿using bonjourtravail_api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace bonjourtravail_api.Services;

public class JobService
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
        await _jobCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Job newJob) =>
        await _jobCollection.InsertOneAsync(newJob);

    public async Task UpdateAsync(string id, Job updatedJob) =>
        await _jobCollection.ReplaceOneAsync(x => x.Id == id, updatedJob);

    public async Task RemoveAsync(string id) =>
        await _jobCollection.DeleteOneAsync(x => x.Id == id);
}