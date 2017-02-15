using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
            client = new MongoClient();
        }

        public void Connect()
        {
            this.database = client.GetDatabase("test");
            MessageCollection = database.GetCollection<Message>("messages");
            UserCollection = database.GetCollection<User>("users");
            FriendshipCollection = database.GetCollection<Friendship>("friends");
            UserBsonCollection = database.GetCollection<BsonDocument>("users");
        }

        public async Task<ObjectId> GetUserId(string userName)
        {
            IMongoQueryable<ObjectId> query = from user in UserCollection.AsQueryable()
                                              where user.Name.Equals(userName)
                                              select user._id;

            return (await query.AnyAsync()) ? await query.FirstAsync() : ObjectId.Empty;
        }

        public async Task<ObjectId> GetUserId(string userName, string password)

        {
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", userName) & builder.Eq("password", password);

            using (IAsyncCursor<BsonDocument> cursor = await UserBsonCollection.FindAsync(filter))
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

        public async Task<List<User>> GetFriendList(ObjectId userId)
        {
            List<ObjectId> friendIds = await GetFriendIdList(userId);

            IMongoQueryable<User> userQuery = from user in UserCollection.AsQueryable()
                                              where friendIds.Contains(user._id)
                                              select user;

            return await userQuery.ToListAsync();

            /*List<User> friends = new List<User>();
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
                                friends.Add(new User { Name = userDocument["username"].AsString, IP = userDocument["ip"].AsString, _id = userDocument["_id"].AsObjectId });
                            }
                        }
                    }
                }
            }

            return friends;*/
        }

        public async Task<List<ObjectId>> GetFriendIdList(ObjectId userId)
        {
            List<ObjectId> friendIds = new List<ObjectId>();

            var friendshipQuery = from friendship in FriendshipCollection.AsQueryable()
                                  where friendship.Receiver.Equals(userId) | friendship.Requester.Equals(userId)
                                  select new { friendship.Receiver, friendship.Requester };

            await friendshipQuery.ForEachAsync((friendship) =>
            {
                ObjectId friendId = (friendship.Receiver.Equals(userId)) ? friendship.Requester : friendship.Receiver;
                friendIds.Add(friendId);
            });

            return friendIds;
        }

        public IMongoCollection<Message> MessageCollection { get; private set; }

        public IMongoCollection<User> UserCollection { get; private set; }

        public IMongoCollection<BsonDocument> UserBsonCollection { get; private set; }

        public IMongoCollection<Friendship> FriendshipCollection { get; private set; }
    }
}
