﻿using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Duftfinder.Database.Mongo
{
	/// <summary>
	///     Creates connection to the database.
	/// </summary>
	/// <author>Anna Krebs</author>
	/// <seealso> href="http://www.c-sharpcorner.com/article/simple-crud-operation-using-asp-net-mvc-and-mongodb/">c-sharpcorner</seealso>
	public class MongoContext
	{
		public MongoContext(IConfiguration configuration)
		{
			//string mongoDatabaseName = "duftfinder";
			var mongoDatabaseName = configuration["MongoDatabaseName"];
			//string connectionString = "mongodb://localhost:27017";
			//string connectionString = "mongodb://duftfinder:27017";
			var connectionString = configuration["MongoConnectionString"];

			// Create client settings & connect to mongoDB.
			var client = new MongoClient(connectionString);
			Database = client.GetDatabase(mongoDatabaseName);
		}

		public IMongoDatabase Database { get; set; }
	}
}