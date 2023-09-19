using Game.Common;
using Game.Inventory.Service.Dtos;
using Game.Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Game.Inventory.Service.Contollers
{  
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryItemRepository;
        private readonly IRepository<CatalogItem> catalogItemRepository;
        public ItemController(IRepository<InventoryItem> inventroyItemRepository,IRepository<CatalogItem> catalogItemRepository)
        {
            this.inventoryItemRepository = inventroyItemRepository;
            this.catalogItemRepository = catalogItemRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        { 
          if(userId == Guid.Empty)
          {
            return BadRequest();
          }      


          var InventoryItemEntities = await inventoryItemRepository.GetAllAsync(item =>item.UserId == userId);
          var itemIds = InventoryItemEntities.Select(item => item.CatalogItemId);
          var catalogItemEntities = await catalogItemRepository.GetAllAsync(item => itemIds.Contains(item.Id));

          var inventoryItemDtos = InventoryItemEntities.Select(InventoryItem =>
          {
               var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == InventoryItem.CatalogItemId);
               return InventoryItem.AsDto(catalogItem.Name,catalogItem.Description);
          });
          return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
           var inventoryItem = await inventoryItemRepository.GetAsync(
            item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);
            
            if(inventoryItem == null)
            {
               inventoryItem = new InventoryItem
               {
                 CatalogItemId = grantItemsDto.CatalogItemId,
                 UserId = grantItemsDto.UserId,
                 Quantity = grantItemsDto.Quantity,
                 AcquiredDate = DateTimeOffset.UtcNow
               };

               await inventoryItemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity+=grantItemsDto.Quantity;
                await inventoryItemRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
       
    }
}