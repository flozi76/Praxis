using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Duftfinder.Domain.Entities
{
    /// <summary>
    /// Entity for User.
    /// Bson attributes have to match attributes in mongoDb.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class User : Entity
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsInactive { get; set; }

        public bool IsSystemAdmin { get; set; }

        public Guid? VerifyAccountKey { get; set; } 

        public bool IsAccountVerified { get; set; }

        public Guid? PasswordResetKey { get; set; }

        public ObjectId RoleId { get; set; }
        
        [BsonIgnore]
        public Role Role { get; set; }

        public override string GetPrimaryName()
        {
            return nameof(Email);
        }

        public override string GetPrimaryValue()
        {
            return nameof(Email);
        }

        /// <summary>
        /// Map Id to string, in order to use irrespective of 
        /// MongoDB in web project.
        /// Return null, if new ObjectId. 
        /// </summary>
        /// <author>Anna Krebs</author>
        [BsonIgnore]
        public string RoleIdString
        {
            get
            {
                if (RoleId != ObjectId.Empty)
                {
                    return RoleId.ToString();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    RoleId = new ObjectId(value);
                }
            }
        }
    }
}
