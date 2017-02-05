using MongoDB.Bson;
using MongoDB.Driver;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            string connectionString = "mongodb://babbeln.ddns.net:27017";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            client = new MongoClient(settings);
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

        public async Task<List<Friend>> GetFriendList(ObjectId userId, FilterDefinition<BsonDocument> filter)
        {
            List<Friend> friends = new List<Friend>();
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            filter = filter & (builder.Eq("requester", userId) | builder.Eq("receiver", userId));

            using (IAsyncCursor<BsonDocument> cursor = await FriendCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        ObjectId friendId = (document["requester"].AsObjectId.Equals(userId)) ? document["receiver"].AsObjectId : document["requester"].AsObjectId;
                        FilterDefinition<BsonDocument> userFilter = builder.Eq("_id", friendId);
                        using (IAsyncCursor<BsonDocument> userCursor = await UserCollection.FindAsync(userFilter))
                        {
                            await userCursor.MoveNextAsync();
                            if (userCursor.Current != null)
                            {
                                BsonDocument userDocument = userCursor.Current.First();
                                friends.Add(new Friend { Name = userDocument["username"].AsString, IP = userDocument["ip"].AsString, _id = userDocument["_id"].AsObjectId });
                            }
                        }
                    }
                }
            }

            return friends;
        }

        public IMongoCollection<BsonDocument> MessageCollection { get; private set; }

        public IMongoCollection<BsonDocument> UserCollection { get; private set; }

        public IMongoCollection<BsonDocument> FriendCollection { get; private set; }
    }
}
