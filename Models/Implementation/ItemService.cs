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
            if(restDTO.PageIndex < 0 || restDTO.PageSize <= 0)
            {
                return new RestDTO<Item[]>()
                {
                    Data = []
                };
            }
            var query = _context.Items.AsNoTracking().AsQueryable();
            
            if (!string.IsNullOrEmpty(restDTO.FilterQuery))
            {
                if (!string.IsNullOrEmpty(restDTO.SortColumn))
                {
                    query = query.Where($"{restDTO.SortColumn}.Contains(@0)", restDTO.FilterQuery);
                }
                else
                {
                    query = query.Where(q => q.Name.Contains(restDTO.FilterQuery));
                }
            }

            var recordCount = await query.CountAsync();

            if(recordCount == 0)
            {
                return new RestDTO<Item[]>
                {
                    Data = Array.Empty<Item>(),
                    PageIndex = restDTO.PageIndex,
                    PageSize = restDTO.PageSize,
                    RecordCount = recordCount,
                    Message = "No Records found for this input",
                    Links = new List<LinkDTO>()
                };
            }

            var totalPages = (int)Math.Ceiling(recordCount / (double)restDTO.PageSize);

            Item[]? result = await query.OrderBy($"{restDTO.SortColumn} {restDTO.SortOrder}")
                            .Skip(restDTO.PageIndex * restDTO.PageSize)
                            .Take(restDTO.PageSize)
                            .ToArrayAsync();

            var links = PaginationHelper.GeneratePaginationLinks(base_url, rel, action,
               restDTO.PageIndex, restDTO.PageSize, totalPages, 
               new Dictionary<string, string> {
                   { "SortColumn", restDTO.SortColumn ?? string.Empty },
                   { "SortOrder", restDTO.SortOrder ?? string.Empty },
                   { "FilterQuery", restDTO.FilterQuery ?? string.Empty }
                      }
            );

            return new RestDTO<Item[]>
            {
                Data = result,
                PageIndex = restDTO.PageIndex,
                PageSize = restDTO.PageSize,
                RecordCount = recordCount,
                TotalPages = totalPages,
                Message = "Successful retrieval",
                Links = links
            };

        }

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
        public async Task<Item?> GetInfoAsync(string name)
        {
            return await _context.Items.Where(x => x.Name == name).Select(s => new Item
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
