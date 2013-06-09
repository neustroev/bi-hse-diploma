using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Code.DataExportProvider;
using Web.Code.Mongo;
using Web.Code.Mongo.Entities;

namespace Web.Models
{
    public class DataMartModel
    {
        public List<EntityAggregation> GetAggregations()
        {
            return Database.Instance.GetCollection<EntityAggregation>(Database.CollectionNames.Aggregations);
        }

        public List<EntityCount> GetAmounts()
        {
            return Database.Instance.GetCollection<EntityCount>(Database.CollectionNames.Counts);
        }

        public List<EntityCount> GetRatings()
        {
            return Database.Instance.GetCollection<EntityCount>(Database.CollectionNames.Ratings);
        }

        public List<TransactionStats> GetTransactions()
        {
            return Database.Instance.GetCollection<TransactionStats>(Database.CollectionNames.Transactions);
        }

        public void UpdateDatabase()
        {
            DataExportProvider.Instance.Export();
        }
    }
}