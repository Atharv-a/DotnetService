using Game.Catalog.Contracts;
using Game.Common;
using Game.Inventory.Service.Entities;
using MassTransit;

namespace Game.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {   
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            var item= await repository.GetAsync(message.ItemId);
            
            if(item == null)
            {
                item = new CatalogItem{
                    Id = message.ItemId,
                    Name = message.Name,
                    Description =message.Description
                    };  
                await repository.CreateAsync(item);
            }
        }
    }
}