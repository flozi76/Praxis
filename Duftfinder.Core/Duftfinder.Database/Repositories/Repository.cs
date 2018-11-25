using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Duftfinder.Database.Helpers;
using System.Transactions;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;
using Duftfinder.Domain.Interfaces.Repositories;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Duftfinder.Database.Repositories
{
    /// <summary>
    /// Represents the generic store of objects of a specific type.
    /// Contains basic functionality for all other repositories.
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <seealso> href="http://selfdocumenting.net/a-quick-mongodb-repository/">selfdocumenting.net</seealso>  
    public abstract class Repository<TEntity, TFilter> : IRepository<TEntity, TFilter> where TEntity : Entity where TFilter : Filter
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MongoContext _dbContext;

        private readonly IMongoCollection<TEntity> _collection;

        // FindOptions are used when finding a document in MongoDB. 
        // CollationStrength.Primary is used in order to perform comparison invariant of case.
        private readonly FindOptions _findOptions = new FindOptions { Collation = new Collation(Constants.de, strength: CollationStrength.Primary) };

        public Repository(MongoContext context)
        {
            _dbContext = context;

            // Gets e.g. the essential oil mongo db collection from the database.
            // -> GetCollection<TEntity>(collection name in MongoDB);
            _collection = _dbContext.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public abstract FilterDefinition<TEntity> ApplyFilter(TFilter filter, IMongoCollection<TEntity> collection);

        public abstract SortDefinition<TEntity> ApplySorting(TFilter filter, IMongoCollection<TEntity> collection);

        /// <summary>
        /// Gets all entries of a collection from MongoDB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        public async Task<IList<TEntity>> GetAllAsync(TFilter filter)
        {
            // Filter collection according to filter values. 
            FilterDefinition<TEntity> queryFilter = ApplyFilter(filter, _collection);

            // Sort collection according to sorting defined in filter.
            SortDefinition<TEntity> sortDefinition = ApplySorting(filter, _collection);

            // Get documents from MongoDB according to filter.
            List<TEntity> documentList = await _collection.Find(queryFilter, _findOptions).Sort(sortDefinition).ToListAsync();
            IList<TEntity> entityList = MongoHelper<TEntity>.DeserializeBsonDocumentsToEntities(documentList);

            return entityList;
        }

        /// <summary>
        /// Gets one specific entry from a collection in MongoDB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        public async Task<TEntity> GetByIdAsync(string id)
        {
            // Create filter, that filters for ObjectId in MongoDB.
            FilterDefinition<TEntity> idFilter = MongoHelper.GetFilter<TEntity>(Constants._id, new ObjectId(id));

            // Get documents from MongoDB by id.
            List<TEntity> documentList = await _collection.Find(idFilter).ToListAsync();

            if (documentList.Count > 0)
            {
                // Return single entity if document was found in MongoDB.
                IList<TEntity> entities = MongoHelper<TEntity>.DeserializeBsonDocumentsToEntities(documentList);
                return entities.SingleOrDefault();
            }

            Log.Error($"An unexpected error occurred while getting id. No entity with id {id} could be found.");
            throw new ArgumentNullException(string.Format(Resources.Resources.Error_NoEntityWithIdFound, id));
        }

        /// <summary>
        /// Inserts an entity in MongDB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="entity"></param>
        public async Task<ValidationResultList> InsertAsync(TEntity entity)
        {
            ValidationResultList validationResult = new ValidationResultList();

            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                // Check, if entry already exists in database.
                bool isDuplicate = await MongoHelper<TEntity>.IsDuplicate(entity.GetPrimaryName(), entity.GetPrimaryValue(), _collection, null);

                // If entity doesn't exist, insert in database.
                if (!isDuplicate)
                {
                    // Serialize entity to BsonDocument and insert.
                    await _collection.InsertOneAsync(entity);
                }
                else
                {
                    // Add error to validation result, if entity already exists.
                    validationResult.Errors.Add(typeof(TEntity).Name, Resources.Resources.Error_EntityAlreadyExists);
                }

                ts.Complete();

                Log.Info($"Inserted {entity} with id {entity.ObjectId} in database.");
            }

            return validationResult;
        }

        /// <summary>
        /// Updates an entity in MongDB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ValidationResultList> UpdateAsync(TEntity entity)
        {
            ValidationResultList validationResult = new ValidationResultList();

            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Create filter, that filters for ObjectId in MongoDB.
                FilterDefinition<TEntity> idFilter = MongoHelper.GetFilter<TEntity>(Constants._id, new ObjectId(entity.Id));

                // Check, if entry already exists in database.
                bool isDuplicate = await MongoHelper<TEntity>.IsDuplicate(entity.GetPrimaryName(), entity.GetPrimaryValue(), _collection, entity.ObjectId);

                ReplaceOneResult replaceResult = null;
                // If changed entity doesn't exist, update in database.
                if (!isDuplicate)
                {
                    // Update the entry with the appropriate MongoDB id.
                    replaceResult = await _collection.ReplaceOneAsync(idFilter, entity);

                    if (replaceResult.MatchedCount == 0)
                    {
                        // Add error to validation result, if entity wasn't found.
                        Log.Error($"No entity with id {entity.Id} was found.");
                        validationResult.Errors.Add(typeof(TEntity).Name, string.Format(Resources.Resources.Error_NoEntityWithIdFound, entity.Id));
                    }
                }
                else
                {
                    // Add error to validation result, if entity already exists.
                    validationResult.Errors.Add(typeof(TEntity).Name, Resources.Resources.Error_EntityAlreadyExists);
                }

                ts.Complete();

                Log.Info($"Updated {replaceResult?.ModifiedCount ?? 0} entity with {entity.Id} in database.");
            }

            return validationResult;
        }

        /// <summary>
        /// Deletes an entity in MongDB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="id"></param>
        public async Task<ValidationResultList> DeleteAsync(string id)
        {
            ValidationResultList validationResult = new ValidationResultList();

            using (TransactionScope ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Create filter, that filters for ObjectId in MongoDB.
                FilterDefinition<TEntity> idFilter = MongoHelper.GetFilter<TEntity>(Constants._id, new ObjectId(id));

                // Delete the entry with the appropriate MongoDB id.
                DeleteResult deleteResult = await _collection.DeleteOneAsync(idFilter);

                if (deleteResult.DeletedCount == 0)
                {
                    // Add error to validation result, if entity wasn't deleted.
                    Log.Error($"No entity with id {id} was deleted.");
                    validationResult.Errors.Add(typeof(TEntity).Name, string.Format(Resources.Resources.Error_NoEntityWithIdDeleted, id));
                }

                ts.Complete();

                Log.Info($"Deleted {deleteResult.DeletedCount} entity with {id} in database.");
            }

            return validationResult;
        }

        /// <summary>
        /// Uses specific filter.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IList<TEntity>> GetByFilterAsync(TFilter filter)
        {
            FilterDefinition<TEntity> queryFilter = ApplyFilter(filter, _collection);

            // Get documents from MongoDB according to filter.
            List<TEntity> documentList = await _collection.Find(queryFilter, _findOptions).ToListAsync();
            IList<TEntity> entityList = MongoHelper<TEntity>.DeserializeBsonDocumentsToEntities(documentList);
            return entityList;
        }
    }
}