using MongoDB.Driver;

namespace Net.Bluewalk.MongoDbEntities;

public static class MongoClientSingleton
{
    private static IMongoClient _client;

    /// <summary>
    /// Get client
    /// </summary>
    /// <param name="mongoUrl"></param>
    /// <returns></returns>
    public static IMongoClient GetClient(MongoUrl mongoUrl) =>
        _client ??= new MongoClient(mongoUrl);
}