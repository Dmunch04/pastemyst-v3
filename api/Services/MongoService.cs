using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using pastemyst.Models;

namespace pastemyst.Services;

public interface IMongoService
{
    public IMongoCollection<Paste> Pastes { get; }
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<ActionLog> ActionLogs { get; }
    public GridFSBucket Images { get; }
}

public class MongoService : IMongoService
{
    public IMongoCollection<Paste> Pastes { get; private set; }

    public IMongoCollection<User> Users { get; private set; }

    public IMongoCollection<ActionLog> ActionLogs { get; private set; }

    public GridFSBucket Images { get; private set; }

    public MongoService(IConfiguration configuration)
    {
        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
        var enumConvention = new ConventionPack { new EnumRepresentationConvention(BsonType.String) };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
        ConventionRegistry.Register("EnumString", enumConvention, type => true);

        var mongoClient = new MongoClient(configuration.GetConnectionString("DefaultDb"));

        var mongoDb = mongoClient.GetDatabase("pastemyst");

        Pastes = mongoDb.GetCollection<Paste>("pastes");
        Users = mongoDb.GetCollection<User>("users");
        ActionLogs = mongoDb.GetCollection<ActionLog>("actionLogs");

        Images = new GridFSBucket(mongoDb, new() {
            BucketName = "images",
            ChunkSizeBytes = 1_000_000
        });

        var usernameIndex = Builders<User>.IndexKeys.Text(u => u.Username);

        Users.Indexes.CreateOne(new CreateIndexModel<User>(usernameIndex, new() {
                    Unique = true
        }));
    }
}
