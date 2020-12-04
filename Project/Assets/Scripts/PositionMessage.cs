using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecieveModels
{
    /*
      "identifier":"{\"channel\":\"PlayerPositionSyncChannel\"}",
        "message":{
      "players":[
         {
            "id":1,
            "position":null
         },.....
    ]
    }
     */
    [Serializable]
    class PositionMessage 
    {
        [Serializable]
        public class Player
        {
            [Serializable]
            public class PositionVector3
            {
                public float x;
                public float y;
                public float z;
            }
            public string id;
            public PositionVector3 position;

        }

        [Serializable]
        public class Message{
            public Player[] players;
        }
        public string identifier;
        public Message message;
    }

}
