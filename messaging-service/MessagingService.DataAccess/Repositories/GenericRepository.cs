using MessagingService.DataAccess.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MessagingService.DataAccess.Repositories
{
    public abstract class GenericRepository<TModel> : IGenericRepository<TModel>
        where TModel : BaseDocumentModel
    {
        protected readonly IMongoCollection<TModel> mongoCollection;

        protected readonly IMongoClient _mongoClient;
        protected readonly IMongoDatabase mongoDatabase;

        public GenericRepository(IMongoClient mongoClient, string dbName, string collectionName)
        {
            _mongoClient = mongoClient;
            mongoDatabase = _mongoClient.GetDatabase(dbName);
            mongoCollection = mongoDatabase.GetCollection<TModel>(collectionName);
        }

        public virtual List<TModel> GetList()
        {
            return mongoCollection.Find(x => true).ToList();
        }

        public virtual TModel GetById(string id)
        { 
            ObjectId docId;
            try
            {
                docId = new ObjectId(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Id. id : '{id}'", ex);
            }
            return mongoCollection.Find<TModel>(m => m.Id == docId).FirstOrDefault();
        }

        public virtual TModel Create(TModel model)
        {
            model.InsertAt = DateTime.UtcNow;
            mongoCollection.InsertOne(model);
            return model;
        }

        public virtual void Update(TModel model)
        {
            mongoCollection.ReplaceOne(m => m.Id == model.Id, model);
        }

        public virtual void Delete(TModel model)
        {
            mongoCollection.DeleteOne(m => m.Id == model.Id);
        }

        public virtual void Delete(string id)
        {
            ObjectId docId;
            try
            {
                docId = new ObjectId(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid Id. id : '{id}'", ex);
            }
            mongoCollection.DeleteOne(m => m.Id == docId);
        }


        public IEnumerable<TModel> Filter(Expression<Func<TModel, bool>> filter)
        {
            return mongoCollection.Find(filter).ToList();
        }

        public TModel GetFirstOrDefault(Expression<Func<TModel, bool>> filter)
        {
            return mongoCollection.Find(filter).FirstOrDefault();
        }

        public TModel GetSingleOrDefault(Expression<Func<TModel, bool>> filter)
        {
            return mongoCollection.Find(filter).SingleOrDefault();
        }

        public long Count(Expression<Func<TModel, bool>> filter)
        {
            return mongoCollection.CountDocuments(filter);
        }

        public bool DoesExist(Expression<Func<TModel, bool>> filter)
        {
            return mongoCollection.CountDocuments(filter) > 0;
        }


    }


}
