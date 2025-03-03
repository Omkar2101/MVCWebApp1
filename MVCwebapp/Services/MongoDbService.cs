using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MVCWebApp.Models;

namespace MVCWebApp.Services
{
    // MongoDbService.cs
    public class MongoDbService
    {
        private readonly IMongoDatabase _visitorDatabase;
        private readonly IMongoDatabase _userDatabase;
        private readonly IMongoDatabase _imageDatabase;
        private readonly GridFSBucket _gridFsBucket;
        private readonly string _visitorCollectionName;
        private readonly string _userCollectionName;
        private readonly string _imageCollectionName;
        private readonly MongoClient _mongoClient;

        public MongoDbService(IConfiguration config)
        {
            // Get configuration values
            string connectionString = config["MongoDB:VisitorDB:ConnectionString"];
            string visitorDatabaseName = config["MongoDB:VisitorDB:DatabaseName"];
            string userDatabaseName = config["MongoDB:UserDB:DatabaseName"];
            string imageDatabaseName = config["MongoDB:ImageDB:DatabaseName"];

            _visitorCollectionName = config["MongoDB:VisitorDB:VisitorCollectionName"];
            _userCollectionName = config["MongoDB:UserDB:UserCollectionName"];
            _imageCollectionName = config["MongoDB:ImageDB:ImageCollectionName"];

            // Validate configuration
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("MongoDB connection string is missing!");
            if (string.IsNullOrEmpty(visitorDatabaseName) || string.IsNullOrEmpty(_visitorCollectionName))
                throw new ArgumentNullException("VisitorDB configuration is missing!");
            if (string.IsNullOrEmpty(userDatabaseName) || string.IsNullOrEmpty(_userCollectionName))
                throw new ArgumentNullException("UserDB configuration is missing!");

            // Create single MongoDB client
            _mongoClient = new MongoClient(connectionString);

            // Get database references
            _visitorDatabase = _mongoClient.GetDatabase(visitorDatabaseName);
            _userDatabase = _mongoClient.GetDatabase(userDatabaseName);
            _imageDatabase = _mongoClient.GetDatabase(imageDatabaseName);

            // ✅ Initialize GridFSBucket
            _gridFsBucket = new GridFSBucket(_imageDatabase);

            Console.WriteLine("MongoDB connection initialized successfully");
        }

        // Generic method to get any collection from visitor database
        public IMongoCollection<T> GetVisitorCollection<T>(string collectionName)
        {
            return _visitorDatabase.GetCollection<T>(collectionName);
        }

        // Generic method to get any collection from user database
        public IMongoCollection<T> GetUserCollection<T>(string collectionName)
        {
            return _userDatabase.GetCollection<T>(collectionName);
        }

        public IMongoCollection<T> GetImageCollection<T>(string collectionName)
        {
            return _imageDatabase.GetCollection<T>(collectionName);
        }

        public GridFSBucket GetGridFSBucket()
        {
            return _gridFsBucket;
        }

      
    }
}