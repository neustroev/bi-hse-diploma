using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Mongo.Entities
{
    public class TransactionStats
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime Date { get; set; }
        public int PurchaseCount { get; set; }
        public double Profit { get; set; }
    }
}