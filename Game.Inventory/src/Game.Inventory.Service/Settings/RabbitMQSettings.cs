using MassTransit.Futures.Contracts;

namespace Game.Common.Settings
{
    public class RabbitMQSettings
    {
        public required string Host {get ; init;}
        public required string Username {get ; init;}
        public required string Password{get; init;}
        public required string VHost{get; init;}
    }
}