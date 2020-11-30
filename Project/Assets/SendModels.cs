using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SendModels
{
    
    public static class RemoteSettings
    {
        public const string rootUrl = "ws://localhost:3000/cable";
    }
    [Serializable]
    class Subscribe
    {
        public string command;
        public string identifier;
        public string data;
        public Subscribe(string _command, string _identifier, string _data = "")
        {
            command = _command;
            identifier = _identifier;
            data = _data;
        }
    }

    [Serializable]
    class Data
    {
        public string action;
        public Data(string _action)
        {
            action = _action;
        }    
    }

    [Serializable]
    class PositionData : Data
    {
        Vector3 position;
        public PositionData(string _action,Vector3 _position) : base(_action)
        {
            position = _position;
        }
    }
    

    [Serializable]
    class Channel
    {
        public string channel;
        public Channel(string _channel)
        {
            channel = _channel;
        }
        //public string room;
    }
}
