using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OctopathII_Items.Models
{
    public class Item
    {
        [Required(ErrorMessage = "Please enter Name for the item")]
        public string Name {  get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter Description for the item")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter buy price for the item")]
        public int Buy_Price { get; set; }
        [Required(ErrorMessage = "Please enter sell price for the item")]
        public int Sell_Price { get; set; }
        [Required(ErrorMessage = "Please enter type for the item")]
        public string Item_Type { get; set; } = string.Empty;

    }
}
