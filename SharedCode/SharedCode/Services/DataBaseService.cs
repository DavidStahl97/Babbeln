using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Services
{
    public class DataBaseService
    {
        private readonly IMongoClient client;
        private IMongoDatabase database;

        public DataBaseService()
        {
            //string connectionString = "mongodb://192.168.1.80:27017";
            //MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
           client = new MongoClient();
        }

        public void Connect()
        {
            this.database = client.GetDatabase("test");
            MessageCollection = database.GetCollection<BsonDocument>("messages");
            UserCollection = database.GetCollection<BsonDocument>("users");
            FriendCollection = database.GetCollection<BsonDocument>("friends");
        }

        public async Task<ObjectId> GetUserId(string userName, string password)
        {
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", userName) & builder.Eq("password", password);

            using (IAsyncCursor<BsonDocument> cursor = await UserCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        return document["_id"].AsObjectId;
                    }
                }
            }

            return ObjectId.Empty;
        }

        public IMongoCollection<BsonDocument> MessageCollection { get; private set; }

        public IMongoCollection<BsonDocument> UserCollection { get; private set; }

        public IMongoCollection<BsonDocument> FriendCollection { get; private set; }
    }
}
