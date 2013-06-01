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
                                    Name = "Количество книг по жанру " + p.Name,
                                    Count = p.Publication.Count,
                                    Search = p.Name
                                });

            var booksByCategory = (from c in DBModel.Category
                                   select new EntityCount()
                                   {
                                       Name = "Количество книг по категории " + c.Name,
                                       Count = c.Publication.Count,
                                       Search = c.Name
                                   });

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
                    Search = "Amount"
                },

                new EntityCount()
                {
                    Name = "Количество публикаций",
                    Count = DBModel.Publication.Count(),
                    Search = "Amount"
                },

                new EntityCount()
                {
                    Name = "Количество транзакций",
                    Count = DBModel.Transaction.Count(),
                    Search = "Amount"
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
                               select new EntityAggregation()
                               {
                                   Aggregation = new Aggregator()
                                   {
                                       Name = "Сумма",
                                       Value = u.Publication.Count
                                   },
                                   Indicator = "Количество книг у автора",
                                   Entity = u.Name
                               });

            var soldPublications = (from t in DBModel.Transaction
                                    select t.Chapter.Publication_Id);

            var soldBooksByUser = (from u in DBModel.User
                                   select new EntityAggregation()
                                   {
                                       Aggregation = new Aggregator()
                                       {
                                           Name = "Сумма",
                                           Value = (from p in u.Publication
                                                    where soldPublications.Contains(p.Id)
                                                    select p).Count()
                                       },
                                       Indicator = "Количество проданных книг у автора",
                                       Entity = u.Name
                                   });

            var averageBookPriceByUser = (from u in DBModel.User
                                          select new EntityAggregation()
                                          {
                                              Aggregation = new Aggregator()
                                              {
                                                  Name = "Среднее",
                                                  Value = u.Publication.Average(p => p.Chapter.Sum(el => el.Price))
                                              },
                                              Indicator = "Средняя стоимость книги у автора",
                                              Entity = u.Name
                                          });

            var maxBookPriceByUser = (from u in DBModel.User
                                      select new EntityAggregation()
                                      {
                                          Aggregation = new Aggregator()
                                          {
                                              Name = "Максимум",
                                              Value = u.Publication.Max(p => p.Chapter.Sum(c => c.Price))
                                          },
                                          Indicator = "Максимальная стоимость книги у автора",
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