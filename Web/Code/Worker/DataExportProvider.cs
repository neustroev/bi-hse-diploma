using APIServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Code.Mongo;
using Web.Code.Mongo.Aggregators;
using Web.Code.Mongo.Entities;

namespace Web.Code.DataExportProvider
{
    public class DataExportProvider
    {
        private BookStoreEntities _dbModel;

        public BookStoreEntities DBModel
        {
            get
            {
                if (_dbModel == null)
                {
                    _dbModel = new BookStoreEntities();
                }
                return _dbModel;
            }
        }

        protected DataExportProvider() { }

        private sealed class WorkerCreator
        {
            private static readonly DataExportProvider instance = new DataExportProvider();
            public static DataExportProvider Instance { get { return instance; } }
        }

        public static DataExportProvider Instance
        {
            get { return WorkerCreator.Instance; }
        }

        private List<EntityCount> GenerateAmountsCollection()
        {
            var booksByGenre = (from p in DBModel.Genre
                                select new EntityCount()
                                {
                                    Name = p.Name,
                                    Count = p.Publication.Count,
                                    Search = "genre"
                                }).OrderByDescending(x => x.Count);

            var booksByCategory = (from c in DBModel.Category
                                   select new EntityCount()
                                   {
                                       Name = c.Name,
                                       Count = c.Publication.Count,
                                       Search = "category"
                                   }).OrderByDescending(x => x.Count);

            List<EntityCount> batch = new List<EntityCount>() {
                new EntityCount()
                {
                    Name = "Пользователи c публикациями",
                    Count = (from u in DBModel.User
                             where u.Publication.Count > 0
                             select u.Id).ToList().Count,
                    Search = "users"
                },
                new EntityCount()
                {
                    Name = "Пользователи без публикаций",
                    Count = (from u in DBModel.User
                             where u.Publication.Count == 0
                             select u.Id).ToList().Count,
                    Search = "users"
                },

                new EntityCount()
                {
                    Name = "Количество пользователей",
                    Count = DBModel.User.Count(),
                    Search = "amount"
                },

                new EntityCount()
                {
                    Name = "Количество публикаций",
                    Count = DBModel.Publication.Count(),
                    Search = "amount"
                },

                new EntityCount()
                {
                    Name = "Количество транзакций",
                    Count = DBModel.Transaction.Count(),
                    Search = "amount"
                },
            };

            batch = batch
                .Union(booksByGenre)
                .Union(booksByCategory)
                .ToList();
            return batch;
        }

        private List<EntityAggregation> GenerateAggregationsCollection()
        {
            List<EntityAggregation> batch = new List<EntityAggregation>();
            var booksByUser = (from u in DBModel.User
                               let _pubCount = u.Publication.Count
                               where _pubCount > 0
                               select new EntityAggregation()
                               {
                                   Aggregation = new Aggregator()
                                   {
                                       Name = "sum",
                                       Value = _pubCount
                                   },
                                   Indicator = "all",
                                   Entity = u.Name
                               });

            var soldPublications = (from t in DBModel.Transaction
                                    select t.Chapter.Publication_Id);

            var soldBooksByUser = (from u in DBModel.User
                                   where u.Publication.Count > 0
                                   select new EntityAggregation()
                                   {
                                       Aggregation = new Aggregator()
                                       {
                                           Name = "sum",
                                           Value = (from p in u.Publication
                                                    where soldPublications.Contains(p.Id)
                                                    select p).Count()
                                       },
                                       Indicator = "sold",
                                       Entity = u.Name
                                   });


            var averageBookPriceByUser = (from u in DBModel.User
                                          where u.Publication.Count > 0
                                          select new EntityAggregation()
                                          {
                                              Aggregation = new Aggregator()
                                              {
                                                  Name = "avg",
                                                  Value = (from p in u.Publication
                                                          select p.Chapter.Sum(el => el.Price))
                                                          .Average()
                                              },
                                              Indicator = "price",
                                              Entity = u.Name
                                          });

            var maxBookPriceByUser = (from u in DBModel.User
                                      where u.Publication.Count > 0
                                      select new EntityAggregation()
                                      {
                                          Aggregation = new Aggregator()
                                          {
                                              Name = "max",
                                              Value = (from p in u.Publication
                                                       select p.Chapter.Sum(el => el.Price))
                                                       .Max()
                                          },
                                          Indicator = "price",
                                          Entity = u.Name
                                      });

            batch = batch
                .Union(booksByUser)
                .Union(soldBooksByUser)
                .Union(averageBookPriceByUser)
                .Union(maxBookPriceByUser)
                .ToList();
            return batch;
        }

        private List<TransactionStats> GenerateTransactionsCollection()
        {
            return (from t in DBModel.Transaction
                    group t by t.Date into g
                    select new TransactionStats()
                    {
                        Date = g.Key,
                        Profit = g.Sum(el => el.Interest),
                        PurchaseCount = g.Count()
                    }).ToList();
        }

        public void Export()
        {
            Database.Instance.InsertCollection(this.GenerateAggregationsCollection(), Database.CollectionNames.Aggregations);
            Database.Instance.InsertCollection(this.GenerateAmountsCollection(), Database.CollectionNames.Counts);
            Database.Instance.InsertCollection(this.GenerateTransactionsCollection(), Database.CollectionNames.Transactions);
        }
    }
}