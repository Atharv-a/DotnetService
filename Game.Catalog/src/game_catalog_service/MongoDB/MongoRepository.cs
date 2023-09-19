using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Game.Common.MongoDB
{   
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
      
         private readonly IMongoCollection<T> dbCollection;
         private readonly FilterDefinitionBuilder<T>filterBuilder = Builders<T>.Filter;
         
         //Dependency injection to provide an session of database in MongoDB
         public MongoRepository(IMongoDatabase database,string collectionName)
         {
             dbCollection = database.GetCollection<T>(collectionName);
         }
         //Get all documents from DB
         public async Task<IReadOnlyCollection<T>> GetAllAsync()
         {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
         }
         //Get all documents find which pass the filter
         public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter)
         { 
            return await dbCollection.Find(filter).ToListAsync();
         }
         // Get one document from DB
         public async Task<T> GetAsync(Guid id){
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
         }
         //Get one document that passes the filter
         public async Task<T> GetAsync(Expression<Func<T,bool>> filter){

            return await dbCollection.Find(filter).FirstOrDefaultAsync();
         }
         //Create a document in DB
         public async Task CreateAsync(T entity)
         {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
         }
         //Make changes to a document in DB
         public async Task UpdateAsync(T entity){
             if(entity == null)
             {
                throw new ArgumentNullException(nameof(entity));
             }
            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity=>existingEntity.Id,entity.Id);
            await dbCollection.ReplaceOneAsync(filter,entity);
         }
         //Remove a  document in DB
         public async Task RemoveAsync(Guid id)
         {
           FilterDefinition<T> filter = filterBuilder.Eq(entity=>entity.Id,id);
           await dbCollection.DeleteOneAsync(filter);
         }

    }
}

   