using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Services;
using VoIPApp.Common.Services;

namespace VoIPApp.Modules.Chat.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IMongoCollection<BsonDocument> friendCollection;
        private readonly IMongoCollection<BsonDocument> userCollection;
        private readonly ServerServiceProxy serverService;
        private ObjectId userId;

        public FriendsService(IUnityContainer container)
        {
            DataBaseService dbService = container.Resolve<DataBaseService>();
            this.friendCollection = dbService.Database.GetCollection<BsonDocument>("friends");
            this.userCollection = dbService.Database.GetCollection<BsonDocument>("users");
            Friends = new ObservableCollection<Friend>();

            this.serverService = container.Resolve<ServerServiceProxy>();
            this.userId = serverService.UserId;
        }

        public ObservableCollection<Friend> Friends { get; set; }

        public async Task UpdateFriendsList()
        {
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = builder.Eq("requester", userId) | builder.Eq("receiver", userId);
            foreach (Friend f in Friends)
            {
                filter = filter & builder.Not(builder.Eq("requester", f._id)) | builder.Not(builder.Eq("receiver", f._id));
            }

            using (IAsyncCursor<BsonDocument> cursor = await friendCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach(BsonDocument document in batch)
                    {
                        ObjectId friendId = (document["requester"].AsObjectId.Equals(userId)) ? document["receiver"].AsObjectId : document["requester"].AsObjectId;
                        FilterDefinition<BsonDocument> userFilter = builder.Eq("_id", friendId);
                        using (IAsyncCursor<BsonDocument> userCursor = await userCollection.FindAsync(userFilter))
                        {
                            await userCursor.MoveNextAsync();
                            if(userCursor.Current != null)
                            {
                                BsonDocument userDocument = userCursor.Current.First();
                                Friends.Add(new Friend { Name = userDocument["username"].AsString, IP = userDocument["ip"].AsString, _id = userDocument["_id"].AsObjectId });
                            }
                        }
                    }
                }
            }
        }

        public async Task AddFriendByName(string friendName)
        {
            Friend f = await serverService.ServerService.AddFriendByNameAsync(friendName);
            if(f != null)
            {
                Friends.Add(f);
            }
        }

        public Friend GetFriendById(ObjectId id)
        {
            foreach(Friend f in Friends)
            {
                if(f._id.Equals(id))
                {
                    return f;
                }
            }

            return null;
        }
    }
}
