using Game.Common.MassTransit;
using Game.Common.MongoDB;
using Game.Inventory.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//constructing and registory dependency with IServiceProvider so that it can be  provided to any class asking for that dependency
builder.Services.AddMongo(builder.Configuration)
                .AddMongoRepositry<InventoryItem>("InventoryItems")
                .AddMongoRepositry<CatalogItem>("CatalogItems")
                .AddMassTransitWithRabbitMq(builder.Configuration);


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
