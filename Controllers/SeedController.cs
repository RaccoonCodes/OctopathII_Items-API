using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OctopathII_Items.Data;
using OctopathII_Items.Models;
using OctopathII_Items.Models.Csv;
using System.Globalization;
using System.Security;

namespace OctopathII_Items.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public SeedController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
            => (_webHostEnvironment, _context) = (webHostEnvironment,context);

        [HttpGet]
        public async Task<ActionResult> ImportData()
        {
            if (!_webHostEnvironment.IsDevelopment())
            {
                throw new SecurityException("Not in Development: Action not allowed");
            }

            string messageItems = await ImportItems();
            string messageEquip = await ImportEquipment();

            return Ok(messageItems + "\n"+ messageEquip);

        }

        //Time Complexity = O(m + n) where m is number of records in CSV file and n is existing number item in DB
        private async Task<string> ImportItems()
        {
            if(await _context.Items.AnyAsync())
            {
                return "Database is already populated with items. Import skipped.";

            }

            //Setup
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };
            using var reader = new StreamReader(
                System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Source/octopath_items.csv"));
            
            using var csv = new CsvReader(reader, config);

            // Preventing duplicates based on Names T(x) = O(1) for lookup
           var existingItems = new HashSet<string>( await _context.Items.AsNoTracking().Select(x => x.Name)
               .ToListAsync(),StringComparer.OrdinalIgnoreCase);

            //Execute
            var skippedRows = 0;
            var newItems = new List<Item>();
            var localItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var record in csv.GetRecords<ItemRecord>())
            {
                if (string.IsNullOrEmpty(record.Name) || existingItems.Contains(record.Name) 
                    || localItems.Contains(record.Name))
                {
                    skippedRows++;
                    continue;
                }
                var item = new Item()
                {
                    Name = record.Name,
                    Description = record.Description,
                    Buy_Price = record.Buy_Price,
                    Sell_Price = record.Sell_Price,
                    Item_Type = record.Item_Type
                };

                newItems.Add(item);
                localItems.Add(record.Name);
            }

            if (newItems.Count != 0)
            {
                //Save
                using var transaction = _context.Database.BeginTransaction();
                await _context.Items.AddRangeAsync(newItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Data successfully populated for Items";
            }
            return "No valid items to import.";
        }

        //Time Complexity = O(m + n) where m is number of records in CSV file and n is existing number item in DB
        private async Task<string> ImportEquipment()
        {
            if (await _context.Equipment.AnyAsync())
            {
                return "Database is already populated with Equipment. Import skipped.";
            }
            //Setup
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };
            using var reader = new StreamReader(
                System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Source/octopath_equipment.csv"));

            using var csv = new CsvReader(reader, config);

            var existingItems = new HashSet<string>(await _context.Equipment.AsNoTracking().Select(x => x.Name)
                .ToListAsync(), StringComparer.OrdinalIgnoreCase);

            var skippedRows = 0;
            var newEquip = new List<Equipment>();
            var localEquip = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var record in csv.GetRecords<EquipmentRecord>())
            {
                if (string.IsNullOrEmpty(record.Name) || existingItems.Contains(record.Name)
                    || localEquip.Contains(record.Name))
                {
                    skippedRows++;
                    continue;
                }

                var equip = new Equipment()
                {
                    Name = record.Name,
                    Max_Hp = record.Max_Hp,
                    Max_SP = record.Max_SP,
                    Physical_Atk = record.Physical_Atk,
                    Elemental_Atk = record.Elemental_Atk,
                    Physical_Def = record.Physical_Def,
                    Elemental_Def = record.Elemental_Def,
                    Accuracy = record.Accuracy,
                    Speed = record.Speed,
                    Critical = record.Critical,
                    Evasion = record.Evasion,
                    Effect = record.Effect,
                    Buy_Price = record.Buy_Price,
                    Sell_Price = record.Sell_Price,
                    Equipment_Type = record.Equipment_Type
                };

                newEquip.Add(equip);
                localEquip.Add(record.Name);
            }

            if (newEquip.Count != 0)
            {
                //Save
                using var transaction = _context.Database.BeginTransaction();
                await _context.Equipment.AddRangeAsync(newEquip);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Data successfully populated for Equipment";
            }
            return "No valid Equipment to import.";

        }

    }
}
