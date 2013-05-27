
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Code.Mongo.Entities;

namespace Web.Code.Mongo
{
    public class Database
    {
        protected Database() { }

        private sealed class DatabaseCreator
        {
            private static readonly Database instance = new Database();
            public static Database Instance { get { return instance; } }
        }

        public static Database Instance
        {
            get { return DatabaseCreator.Instance; }
        }

        private MongoServer _server;
        private MongoServer Server
        {
            get
            {
                if (_server == null)
                {
                    _server = MongoServer.Create("mongodb://localhost:27017/");
                }
                return _server;
            }
        }

        private MongoDatabase _db;
        private MongoDatabase Db
        {
            get
            {
                if (_db == null)
                {
                    _db = Server.GetDatabase("DataMart");
                }
                return _db;
            }
        }

        public ObjectId IsUser(string login, string password)
        {
            var user = Db.GetCollection<User>("Users").FindOne(Query.And(Query.EQ("Login", login), Query.EQ("Password", password)));
            return user.Id;
        }

        public bool IsUser(string id)
        {
            var user = Db.GetCollection<User>("Users").FindOneById(id);
            return user != null;
        }


    }
}