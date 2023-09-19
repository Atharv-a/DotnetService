using MassTransit.Futures.Contracts;

namespace Game.Common.Settings
{
    public class MongoDbSettings
    {   
       
        // public string Host {get; init;}
        // public int Port {get; init;}
        public  required string Username{get; init;}
        public  required string Password{get; init;}
        public string ConnectionString => $"mongodb+srv://{Username}:{Password}@cluster.rfcnoje.mongodb.net/?retryWrites=true&w=majority";

    }
}