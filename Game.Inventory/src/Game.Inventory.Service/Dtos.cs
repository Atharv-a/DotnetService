using System.ComponentModel.DataAnnotations;

namespace Game.Inventory.Service.Dtos
{
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, string Name, String Description, int Quantity, DateTimeOffset AcquiredDate);
    public record CatalogItemDto(Guid Id,[Required]string Name,string Description);
    
} 