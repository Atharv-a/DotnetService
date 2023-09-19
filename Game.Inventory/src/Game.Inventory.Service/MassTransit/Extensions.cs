using MassTransit;
using Game.Common.Settings;
using System.Reflection;
using System.Security.Authentication;

namespace Game.Common.MassTransit
{
     public static class Extensions
     {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services,IConfiguration configuration)
        {

            //Adding Mass Transit Classes
            services.AddMassTransit(configure =>  // configure is configurator which will be used to set up how MassTransit operates
            {   
            
            configure.AddConsumers(Assembly.GetEntryAssembly());
            //specifying type of transport that will be used
            //context - This is typically the IBusRegistrationContext, which provides access to various services related to the bus being configured 
                //and might be needed while setting up the endpoints.
            //This is the primary configuration object for RabbitMQ. You'll use this to set details like the host address, credentials, etc.
                configure.UsingRabbitMq((context,configurator) => 
                {  
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    //get instance of RabbitMq settings
                    var RabbitMqHost = Environment.GetEnvironmentVariable("RabbitMqHost");
                    var RabbitMqVHost = Environment.GetEnvironmentVariable("RabbitMqVHost");
                    var RabbitMqPass = Environment.GetEnvironmentVariable("RabbitMqPass");

                    configurator.Host(RabbitMqHost, 5671,RabbitMqVHost, h =>
                    {
                        h.Username(RabbitMqVHost);
                        h.Password(RabbitMqPass);

                        h.UseSsl(s =>
                        {
                            s.Protocol = SslProtocols.Tls12;
                        });
                    });

                 //This sets up the endpoints MassTransit will use when communicating with RabbitMQ. 
                //The KebabCaseEndpointNameFormatter is being used to format the endpoint names based on the provided service name in a kebab-case style
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName,false));
                //IF for any reason consumer is not able reading a message it will try three times
                //and will have five second deplay after each attempt
                    configurator.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            // Mass transit will now automatically add an IHostedService and builder.Services.AddMassTransitHostedService() is no longer needed
            return services;
        }
     }
}