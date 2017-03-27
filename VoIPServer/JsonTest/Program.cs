using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = @"{
                                'type':'message',
                                'data':{
                                    'from':'UID',
                                    'to':'UID',
                                    'date':'Date',
                                    'message':'message'
                                    }
                              }";

            JsonMessage j = JsonConvert.DeserializeObject<JsonMessage>(message);
        }
    }
}
