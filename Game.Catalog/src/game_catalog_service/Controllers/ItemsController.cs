using System;
using Game.Catalog.Service;
using Game.Catalog.Service.Entities;
using Game.Common;
using Games.Catalog.Service.Dtos;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Game.Catalog.Contracts;

namespace Games.Catalog.Service.Controllers
{
   [ApiController]
   [Route("items")]
   public class ItemController : ControllerBase
   {
     
     // repository layer between application logic and actual database
     private readonly IRepository<Item> itemRepository ;
     private readonly IPublishEndpoint publishEndpoint; 
    // Dependency Injection of ItemRepository so that we can make changes in our database
     public ItemController(IRepository<Item> itemRepository,IPublishEndpoint publishEndpoint)
     {
          this.itemRepository = itemRepository;
          this.publishEndpoint = publishEndpoint;
     } 
     
     //Get all items
     [HttpGet]
     public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
     {   
        var items = (await itemRepository.GetAllAsync()).Select(item => item.AsDto());

        return Ok(items);
     }

     //Get one item by id
     [HttpGet("{id}")]
     public async Task<ActionResult<ItemDto>>  GetByIdAsync(Guid id)
     {  

        var item = await itemRepository.GetAsync(id);
        if(item == null)
        {
         return NotFound();
        }
        return item.AsDto();
     }
     
     // Create a new post
     [HttpPost]
     public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto){
         
         var item = new Item{
            
            Id = Guid.NewGuid(),
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
         };
         
         await itemRepository.CreateAsync(item);
         
         await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
         
         return CreatedAtAction(nameof(GetByIdAsync),new{id=item.Id},item);
     }
     
     // Update a existing post
     [HttpPut("{id}")]
     public async Task<IActionResult> PutAsync(Guid id,UpdateItemDto updateItemDto)
     {
       var existingItem = await itemRepository.GetAsync(id);
       if(existingItem == null)
       {
         return NotFound();
       }
       existingItem.Name = updateItemDto.Name;
       existingItem.Description = updateItemDto.Description;
       existingItem.Price = updateItemDto.Price;

       await itemRepository.UpdateAsync(existingItem);

       await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));
       
       return NoContent();
     }
     
     // Delete a existing post
     [HttpDelete("{id}")]
     public async Task<IActionResult> DeleteAsync(Guid id)
     { 
       var item = await itemRepository.GetAsync(id);

       if(item == null)
       {
          return NotFound();
       }

       await itemRepository.RemoveAsync(id);
       
       await publishEndpoint.Publish(new CatalogItemDeleted(id));

       return NoContent();
     }
   }
}