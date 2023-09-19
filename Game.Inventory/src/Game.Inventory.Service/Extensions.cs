using Game.Inventory.Service.Dtos;
using Game.Inventory.Service.Entities;

namespace Game.Inventory.Service
{
    public static class Extensions{
        public static InventoryItemDto AsDto(this InventoryItem item,string name,string description)
        {
            return new InventoryItemDto(item.CatalogItemId,name,description,item.Quantity,item.AcquiredDate);
        }
    }
}