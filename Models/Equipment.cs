using Microsoft.EntityFrameworkCore;

namespace OctopathII_Items.Models
{
    public class Equipment
    {
        public string Name { get; set; } = string.Empty;
        public int Max_Hp { get; set; }
        public int Max_SP {  get; set; }
        public int Physical_Atk { get; set; }
        public int Elemental_Atk {  get; set; }
        public int Physical_Def { get; set; }
        public int Elemental_Def { get; set; }
        public int Accuracy { get; set; }
        public int Speed { get; set; }
        public int Critical {  get; set; }
        public int Evasion { get; set; }
        public string Effect { get; set; } = string.Empty;
        public int Buy_Price { get; set; }
        public int Sell_Price { get; set; }
        public string Equipment_Type { get; set; } = string.Empty;

    }
}
