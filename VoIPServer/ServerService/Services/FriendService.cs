using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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


        public async Task<User> AddFriendByName(string friendName)
        {
            /*User f = null;

            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", friendName);

            using (IAsyncCursor<BsonDocument> cursor = await dataBaseService.UserCollection.FindAsync(filter))
            {
                cursor.MoveNext();
                if (cursor.Current != null)
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    BsonDocument doc = batch.First();
                    f = new User
                    {
                        Name = doc["username"].AsString,
                        IP = doc["ip"].AsString,
                        _id = doc["_id"].AsObjectId
                    };
                }
            }*/

            User friend = await (from user in dataBaseService.UserCollection.AsQueryable()
                                           where user.Name.Equals(friendName)
                                           select user).FirstAsync();

            if (friend != null)
            {
                await dataBaseService.FriendshipCollection.InsertOneAsync(
                    new Friendship
                    {
                        Receiver = friend._id,
                        Requester = loginService.UserId,
                        Date = DateTime.Now,
                        Accepted = true
                    }
                );
            }

            return friend;
        }
    }
}
