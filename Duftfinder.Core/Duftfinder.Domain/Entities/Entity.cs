using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Base entity with global properties.
    /// </summary>
    /// <author>Anna Krebs</author>
    public abstract class Entity
    {
        /// <summary>
        /// The Id in the MongoDB collection.
        /// </summary>
        /// <author>Anna Krebs</author>
        [BsonId]
        public ObjectId ObjectId { get; set; }

        /// <summary>
        /// Map Id to string, in order to use irrespective of 
        /// MongoDB in web project.
        /// Return null, if new ObjectId. 
        /// </summary>
        /// <author>Anna Krebs</author>
        [BsonIgnore]
        public string Id
        {
            get
            {
                if (ObjectId != ObjectId.Empty)
                {
                    return ObjectId.ToString();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    ObjectId = new ObjectId(value);
                }
            }
        }

        /// <summary>
        /// Gets name of the primary property.
        /// E.g. in order to check for duplicates in DB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        public abstract string GetPrimaryName();

        /// <summary>
        /// Gets value of the primary property.
        /// E.g. in order to check for duplicates in DB.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        public abstract string GetPrimaryValue();

    }
}
