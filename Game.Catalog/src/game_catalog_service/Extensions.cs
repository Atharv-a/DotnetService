using Game.Catalog.Service.Entities;
using Games.Catalog.Service.Dtos;

namespace Game.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id,item.Name,
            item.Description,item.Price,item.CreatedDate);
        }
    }
}