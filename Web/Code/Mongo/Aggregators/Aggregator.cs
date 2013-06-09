using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Mongo.Aggregators
{
    public class Aggregator
    {
        public ObjectId Id;
        public string Name;
        public double? Value;
    }
}