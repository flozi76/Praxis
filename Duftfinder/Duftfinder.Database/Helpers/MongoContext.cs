using System.Configuration;
using System.Reflection;
using Duftfinder.Domain.Helpers;
using log4net;
using MongoDB.Driver;

namespace Duftfinder.Database.Helpers
{
    /// <summary>
    /// Creates connection to the database.
    /// </summary>
    /// <author>Anna Krebs</author>
    /// <seealso> href="http://www.c-sharpcorner.com/article/simple-crud-operation-using-asp-net-mvc-and-mongodb/">c-sharpcorner</seealso> 
    public class MongoContext
    {
        public IMongoDatabase Database { get; set; }

        public MongoContext()
        {
            // Get connection string from Web.config.
            string mongoDatabaseName = ConfigurationManager.AppSettings[Constants.MongoDatabaseName]; // appharbor_3ddx5860
            string connectionString = ConfigurationManager.AppSettings[Constants.MongoConnectionString]; // mongodb://admin:tfuD1820@ds141766.mlab.com:41766/appharbor_3ddx5860 

            // Create client settings & connect to mongoDB.
            MongoClient client = new MongoClient(connectionString);
            Database = client.GetDatabase(mongoDatabaseName);
        }
    }
}
