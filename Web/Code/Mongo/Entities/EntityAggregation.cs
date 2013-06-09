using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Code.Mongo.Aggregators;

namespace Web.Code.Mongo.Entities
{
    public class EntityAggregation
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Indicator { get; set; }
        public string Entity { get; set; }
        public Aggregator Aggregation { get; set; }        
    }
}