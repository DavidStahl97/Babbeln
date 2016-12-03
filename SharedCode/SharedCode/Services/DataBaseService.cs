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
            this.client = new MongoClient();
        }

        public void Connect()
        {
            this.database = client.GetDatabase("test");
        }

        public IMongoDatabase Database
        {
            get { return this.database; }
        }
    }
}
