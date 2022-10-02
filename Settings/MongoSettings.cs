namespace bonjourtravail_api.Settings;

public class MongoDatabaseSettings
{
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string JobCollectionName { get; set; }
}
