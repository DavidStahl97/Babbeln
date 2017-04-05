using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            JObject m = JObject.Parse(message);
            string type = m["type"].ToString();
        }
    }
}
