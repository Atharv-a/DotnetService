using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;   
using Game.Common.Settings;

namespace Game.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
        {  
            //whenever a document is stored in the MongoDB as Guid it will be stored as a string
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            // Used to convert time into string instead of being presented as milliseconds
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            // contructing and registering services that will be used for dependency injection
            // binding configuration method to configuration section


            // binding configuration method to configuration section
            // creatind singleton of DB so everywhere in microservice a single instance is used to get access to DB; 
            // We did'nt  declared a explicit mapping of dependency as done for IItemRepository as whenever IMongoDatabase
            //will be encountered by ASP.NET core Dependency Injection will resolve the correct instance based on the types required by constructors or other injection points.
            services.AddSingleton(ServiceProvider =>
            {   
                // gets value attributes from appsettings.json and set them in serviceSettings and mongoDbSettings
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoConnectionString =Environment.GetEnvironmentVariable("Mongo_connection_string");

                var mongoClient = new MongoClient(mongoConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
            
            return services;
        }            

        public static IServiceCollection AddMongoRepositry<T>(this IServiceCollection services,string collectionName) where T:IEntity
        {
              //In this code, we mapped IRepository as it is not the return type of any dependency which will automatically be found and provided; rather, it is an instance of a class.
            //When you register a service with AddSingleton, you can provide a factory method that takes the IServiceProvider as a parameter, allowing you to resolve other services from the container.
            //The IServiceProvider interface in .NET Core's dependency injection system provides a way to retrieve service instances. 
            //You can think of it as a way to get services that have already been registered with the DI container.
            //ServiceProvider is used as an argument to the lambda function to potentially resolve other services. 
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {  
                var database =serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database,collectionName);
            });                 
            return services;
        }
    }
}