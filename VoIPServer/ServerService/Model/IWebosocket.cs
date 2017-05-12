using MongoDB.Bson;
using SharedCode.Models;

namespace ServerServiceLibrary.Model
{
    public interface IWebosocket
    {
        void OnFriendshipRequestAnswered(ObjectId friendId, bool accept);
        void OnFriendshipRequested(ObjectId friendId, ObjectId userId);
        void OnFriendStatusChanged(ObjectId friendId, Status status);
        void OnMessageReceived(SharedCode.Models.Message msg);
    }
}