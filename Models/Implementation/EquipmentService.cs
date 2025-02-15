using Microsoft.EntityFrameworkCore;
using OctopathII_Items.Data;
using OctopathII_Items.DTO;
using OctopathII_Items.Extensions;
using OctopathII_Items.Models.Interfaces;
using System.Linq.Dynamic.Core;

namespace OctopathII_Items.Models.Implementation
{
    public class EquipmentService : IEquipmentService
    {
        private readonly ApplicationDbContext _context;

        public EquipmentService (ApplicationDbContext context)
            => _context = context;

        //O(log n + k + p)
        public async Task<RestDTO<Equipment[]>> GetEquipmentAsync(RequestDTO<Equipment> restDTO, string base_url, string rel, string action)
        {
            var query = _context.Equipment.AsNoTracking().AsQueryable();
            return await GenericPaginatedData.GetPaginatedDataAsync(restDTO, query, base_url, rel, action);
        }


        public async Task<Equipment?> PutEquipmentAsync(Equipment equipment)
        {
            Equipment? updatedEquip = await _context.Equipment.FirstOrDefaultAsync(x => x.Name == equipment.Name);

            if(updatedEquip == null)
            {
                return null;
            }

            updatedEquip.Max_Hp = equipment.Max_Hp;
            updatedEquip.Max_SP = equipment.Max_SP;
            updatedEquip.Physical_Atk = equipment.Physical_Atk;
            updatedEquip.Elemental_Atk = equipment.Elemental_Atk;
            updatedEquip.Physical_Def = equipment.Physical_Def;
            updatedEquip.Elemental_Def = equipment.Elemental_Def;
            updatedEquip.Accuracy = equipment.Accuracy;
            updatedEquip.Speed = equipment.Speed;
            updatedEquip.Critical = equipment.Critical;
            updatedEquip.Evasion = equipment.Evasion;
            updatedEquip.Effect = equipment.Effect;
            updatedEquip.Buy_Price = equipment.Buy_Price;
            updatedEquip.Sell_Price = equipment.Sell_Price;
            updatedEquip.Equipment_Type = equipment.Equipment_Type;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the item.", ex);
            }
            return updatedEquip;
        }
        public async Task<Equipment?> GetInfoEquipment(string name)
        {
            return await _context.Equipment.AsNoTracking().Where(x => x.Name == name).Select(s => new Equipment
            {
                Name = s.Name,
                Max_Hp = s.Max_Hp,
                Max_SP = s.Max_SP,
                Physical_Atk = s.Physical_Atk,
                Elemental_Atk = s.Elemental_Atk,
                Physical_Def = s.Physical_Def,
                Elemental_Def = s.Elemental_Def,
                Accuracy = s.Accuracy,
                Speed = s.Speed,
                Critical = s.Critical,
                Evasion = s.Evasion,
                Effect = s.Effect,
                Buy_Price = s.Buy_Price,
                Sell_Price = s.Sell_Price,
                Equipment_Type = s.Equipment_Type
            }).FirstOrDefaultAsync();
        }



    }
}
