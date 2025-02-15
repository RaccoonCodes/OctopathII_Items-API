using OctopathII_Items.DTO;

namespace OctopathII_Items.Models.Interfaces
{
    public interface IEquipmentService
    {
        Task<RestDTO<Equipment[]>> GetEquipmentAsync(RequestDTO<Equipment> restDTO, string base_url, string rel, string action);
        Task<Equipment?> PutEquipmentAsync(Equipment equipment);
        Task<Equipment?> GetInfoEquipment(string name);
    }
}
