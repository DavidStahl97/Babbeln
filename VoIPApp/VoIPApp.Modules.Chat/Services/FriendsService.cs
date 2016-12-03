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

namespace VoIPApp.Modules.Chat.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IMongoCollection<BsonDocument> friendCollection;

        public FriendsService(IUnityContainer container)
        {
            DataBaseService dbService = container.Resolve<DataBaseService>();
            this.friendCollection = dbService.Database.GetCollection<BsonDocument>("users");
            Friends = new ObservableCollection<Friend>();
        }

        public ObservableCollection<Friend> Friends { get; set; }

        public async Task UpdateFriendsList()
        {
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = builder.Empty;
            foreach (Friend f in Friends)
            {
                filter = filter & builder.Not(builder.Eq("_id", f._id));
            }

            using (IAsyncCursor<BsonDocument> cursor = await friendCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach(BsonDocument document in batch)
                    {
                        Friends.Add(BsonSerializer.Deserialize<Friend>(document));
                    }
                }
            }
        }

        public async void UpdateFriendById(ObjectId id)
        {
            FilterDefinitionBuilder<BsonDocument> builder = new FilterDefinitionBuilder<BsonDocument>();
            FilterDefinition<BsonDocument> filter = builder.Eq("_id", id);
        }

    }
}
