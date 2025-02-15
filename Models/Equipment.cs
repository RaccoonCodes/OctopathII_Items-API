using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OctopathII_Items.Models
{
    public class Equipment
    {
        [Required(ErrorMessage ="Please enter Name for equipment")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage ="Please enter Max HP for equipment")]
        public int Max_Hp { get; set; }
        [Required(ErrorMessage = "Please enter Max SP for equipment")]
        public int Max_SP {  get; set; }
        [Required(ErrorMessage = "Please enter Physical Attack for equipment")]
        public int Physical_Atk { get; set; }
        [Required(ErrorMessage = "Please enter Elemental Attack for equipment")]
        public int Elemental_Atk {  get; set; }
        [Required(ErrorMessage = "Please enter Physical Defense for equipment")]
        public int Physical_Def { get; set; }
        [Required(ErrorMessage = "Please enter Elemental Defense for equipment")]
        public int Elemental_Def { get; set; }
        [Required(ErrorMessage = "Please enter Accuracy for equipment")]
        public int Accuracy { get; set; }
        [Required(ErrorMessage = "Please enter Speed for equipment")]
        public int Speed { get; set; }
        [Required(ErrorMessage = "Please enter Critical for equipment")]
        public int Critical {  get; set; }
        [Required(ErrorMessage = "Please enter Evasion for equipment")]
        public int Evasion { get; set; }
        [Required(ErrorMessage = "Please enter Effect for equipment")]
        public string Effect { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter Buy Price for equipment")]
        public int Buy_Price { get; set; }
        [Required(ErrorMessage = "Please enter Sell Price for equipment")]
        public int Sell_Price { get; set; }
        [Required(ErrorMessage = "Please enter Equipment Type for equipment")]
        public string Equipment_Type { get; set; } = string.Empty;

    }
}
