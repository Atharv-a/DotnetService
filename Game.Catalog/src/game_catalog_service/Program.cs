using Game.Catalog.Service.Entities;
using Game.Common.MongoDB;
using Game.Common.Settings;
using Game.Common.MassTransit;

var builder = WebApplication.CreateBuilder(args);
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

// Add services to the container.

  //ASP.NETCORE BY default removes Async suffix in method names 
  // we change value for this attribute to false
  // so that it does not affect the CreatedAtAction in post methods which has to find name of(GetBYIdAsync) or similar names with Async suffix
  builder.Services.AddControllers(options => {
      options.SuppressAsyncSuffixInActionNames =false;   
    }
  );

//constructing and registory dependency with IServiceProvider so that it can be provided to any class asking for that dependency
builder.Services.AddMongo(builder.Configuration)
                .AddMongoRepositry<Item>("items")
                .AddMassTransitWithRabbitMq(builder.Configuration);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
    app.UseCors(Builder =>
    {
        Builder.WithOrigins(builder.Configuration["AllowedOrigin"])
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
    