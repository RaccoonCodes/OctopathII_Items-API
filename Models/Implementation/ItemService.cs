using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using OctopathII_Items.Data;
using OctopathII_Items.DTO;
using OctopathII_Items.Extensions;
using OctopathII_Items.Models.Interfaces;
using System.Linq.Dynamic.Core;

namespace OctopathII_Items.Models.Implementation
{
    public class ItemService : IItemsService
    {
    private readonly ApplicationDbContext _context;
        public ItemService(ApplicationDbContext context)
            => _context = context;


        //O(log n + k + p)
        public async Task<RestDTO<Item[]>> GetItemsAsync(RequestDTO<Item> restDTO, string base_url, string rel, string action)
        {
            var query = _context.Items.AsNoTracking().AsQueryable();
            return await GenericPaginatedData.GetPaginatedDataAsync(restDTO, query, base_url, rel, action);
        }

        //O(lg n) for index
        public async Task<Item?> PutItemAsync(Item item)
        {
            Item? updatedItem = await _context.Items.FirstOrDefaultAsync(x => x.Name == item.Name); 
            
            if(updatedItem == null)
            {
                return null;
            }

            updatedItem.Description = item.Description;
            updatedItem.Buy_Price = item.Buy_Price;
            updatedItem.Sell_Price = item.Sell_Price;
            updatedItem.Item_Type = item.Item_Type;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the item.", ex);
            }

            return updatedItem;
        
        }
        //O(lg n)
        public async Task<Item?> GetInfoAsync(string name)
        {
            return await _context.Items.AsNoTracking().Where(x => x.Name == name).Select(s => new Item
            {
                Name = s.Name,
                Description = s.Description,
                Buy_Price = s.Buy_Price,
                Sell_Price = s.Sell_Price,
                Item_Type = s.Item_Type

            }).FirstOrDefaultAsync();
        }
    }
}
