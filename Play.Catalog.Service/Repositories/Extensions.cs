using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service.Repositories
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddMongo(this WebApplicationBuilder builder)
        {

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));


            builder.Services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection("ServiceSettings").Get<ServiceSettings>(); // "Catalog" db
                var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase((serviceSettings.ServiceName));
            });

            return builder;
        }

        public static WebApplicationBuilder AddMongoRepository<T>(this WebApplicationBuilder builder, string collectionName) where T : IEntity
        {
            builder.Services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });

            return builder;
        }
    }
}
