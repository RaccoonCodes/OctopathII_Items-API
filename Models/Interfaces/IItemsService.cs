using OctopathII_Items.DTO;

namespace OctopathII_Items.Models.Interfaces
{
    public interface IItemsService
    {
        Task<RestDTO<Item[]>> GetItemsAsync(RequestDTO<Item> restDTO,string base_url, string rel, string action);
        Task<Item?> PutItemAsync(Item item);
        Task<Item?> GetInfoAsync(string name);
    }
}
