using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Duftfinder.Database.Helpers
{
    /// <summary>
    /// Contains helper stuff for MongoDB.
    /// </summary>
    /// <author>Anna Krebs</author>
    public static class MongoHelper
    {
        // FindOptions are used when finding a document in MongoDB. 
        // CollationStrength.Primary is used in order to perform comparison invariant of case.
        private static readonly FindOptions FindOptions = new FindOptions { Collation = new Collation(Constants.de, strength: CollationStrength.Primary) };

        /// <summary>
        /// Creates a Bson filter to get by specific field in collection.
        /// fieldName: The field in the collection that is filtered for. (e.g. Name)
        /// filterValue: The value in the collection thtat is filtered for. (e.g. Pfefferminze)
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="fieldName"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static FilterDefinition<TEntity> GetFilter<TEntity>(string fieldName, ObjectId filterValue)
        {
            return Builders<TEntity>.Filter.Eq(fieldName, filterValue);
        }
    }

    public static class MongoHelper<TEntity> where TEntity : Entity
    {
        // FindOptions are used when finding a document in MongoDB. 
        // CollationStrength.Primary is used in order to perform comparison invariant of case.
        private static readonly FindOptions FindOptions = new FindOptions { Collation = new Collation(Constants.de, strength: CollationStrength.Primary) };

        /// <summary>
        /// Converts a list of BsonDocuments to a list of TEntity.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="documentList"></param>
        /// <returns></returns>
        public static IList<TEntity> DeserializeBsonDocumentsToEntities(List<TEntity> documentList)
        {
            IList<TEntity> entityList = new List<TEntity>();
            foreach (TEntity document in documentList)
            {
                entityList.Add(BsonSerializer.Deserialize<TEntity>(document.ToBsonDocument()));
            }

            return entityList;
        }

        /// <summary>
        /// Checks duplicates in MongoDB, according to the primary property of the entity. 
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="primaryName"></param>
        /// <param name="primaryValue"></param>
        /// <param name="collection"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static async Task<bool> IsDuplicate(string primaryName, string primaryValue, IMongoCollection<TEntity> collection, ObjectId? objectId)
        {
            // Don't check for duplicates, if no primaryName is set.
            if (string.IsNullOrEmpty(primaryName) || string.IsNullOrEmpty(primaryValue))
            {
                return false;
            }

            // Filter to check duplicate of entity. primaryValue has to be exact match in collection. 
            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(primaryName, primaryValue.Trim());
            List<TEntity> documentList = await collection.Find(filter, FindOptions).ToListAsync();

            // Entity doesn't exist is database.
            if (documentList.Count == 0)
            {
                return false;
            }

            // The entity itself was updated and therefore already exists in database.
            if (documentList.Count == 1)
            {
                IList<TEntity> entityList = DeserializeBsonDocumentsToEntities(documentList);
                bool isSameUpdateEntity = entityList.SingleOrDefault()?.ObjectId == objectId;

                if (isSameUpdateEntity)
                {
                    return false;
                }
            }

            // Entity is already exists in database.
            return true;
        }
    }
}
