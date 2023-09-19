using Game.Catalog.Contracts;
using Game.Common;
using Game.Inventory.Service.Entities;
using MassTransit;


namespace Game.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {   
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.Itemid);

            if(item == null)
            {
               return;
            }
            else
            {
                await repository.RemoveAsync(message.Itemid);
            }
        }
    }
}