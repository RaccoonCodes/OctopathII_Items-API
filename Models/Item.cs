using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopathII_Items.Models
{
    public class Item
    {
        public string Name {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Buy_Price { get; set; }
        public int Sell_Price { get; set; }
        public string Item_Type { get; set; } = string.Empty;

    }
}
