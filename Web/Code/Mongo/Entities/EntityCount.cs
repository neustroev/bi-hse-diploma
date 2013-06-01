using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Mongo.Entities
{
    public class EntityCount
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Search { get; set; }
    }
}