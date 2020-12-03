using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SendModels
{
    
    public static class RemoteSettings
    {
        public const string rootUrl = "ws://localhost:3000/cable";
        public const string authorithateUrl = "http://localhost:3000/player_token";
        public const string authentificateUrl = "http://localhost:3000/player";
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
        [Serializable]
        public class PositionVector3
        {
            public float x;
            public float y;
            public float z;
            public PositionVector3(Vector3 _position)
            {
                x = _position.x;
                y = _position.y;
                z = _position.z;
            }
        }
        public PositionVector3 position;
        public PositionData(string _action, PositionVector3 _position) : base(_action)
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

    /*
     {
    "player":{
        "name":"Aram",
        "password":"12345"
    }
}
     */
    [Serializable]
    class Authentificate
    {
        [Serializable]
        public class Player
        {
            public string name;
            public string password;
            public Player(string _name, string _password)
            {
                name = _name;
                password = _password;
            }
        }
        public Player player;
        public Authentificate(string _name, string _password)
        {
            player = new Player(_name, _password);
        }

    }
    /*
     {
    "auth":{
        "name":"Aram",
        "password":"12345"
    }
    }
     */
    [Serializable]
    class Authorithate
    {
        [Serializable]
        public class Auth
        {
            public string name;
            public string password;
            public Auth(string _name,string _password)
            {
                name = _name;
                password = _password;
            }
        }
        public Auth auth;
        public Authorithate(string _name, string _password)
        {
            auth = new Auth(_name, _password);
        }
    }
    class JWT
    {
        public string jwt;
    }
    class PlayerInfo
    {
        public string id;
        public string name;
    }
}
