using MongoDB.Bson;
using MongoDB.Driver;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class FriendService
    {
        private readonly DataBaseService dataBaseService;
        private readonly LoginService loginService;

        public FriendService(DataBaseService dataBaseService, LoginService loginService)
        {
            this.dataBaseService = dataBaseService;
            this.loginService = loginService;
        }


        public async Task<Friend> AddFriendByName(string friendName)
        {
            Friend f = null;

            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", friendName);

            using (IAsyncCursor<BsonDocument> cursor = await dataBaseService.UserCollection.FindAsync(filter))
            {
                cursor.MoveNext();
                if (cursor.Current != null)
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    BsonDocument doc = batch.First();
                    f = new Friend
                    {
                        Name = doc["username"].AsString,
                        IP = doc["ip"].AsString,
                        _id = doc["_id"].AsObjectId
                    };
                }
            }

            if (f != null)
            {
                BsonDocument document = new BsonDocument
                {
                    { "requester", loginService.UserId },
                    { "receiver", f._id },
                    { "date", DateTime.Now },
                    { "accepted", true }
                };

                await dataBaseService.FriendCollection.InsertOneAsync(document);
            }

        return f;
        }
    }
}
