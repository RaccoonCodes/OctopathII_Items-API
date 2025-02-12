using CsvHelper.Configuration.Attributes;

namespace OctopathII_Items.Models.Csv
{
    public class EquipmentRecord
    {
        [Name("Name")]
        public string Name { get; set; } = string.Empty;
        [Name("Maximum_HP")]
        public int Max_Hp { get; set; }
        [Name("Maximum_SP")]
        public int Max_SP { get; set; }
        [Name("Physical_Attack")]
        public int Physical_Atk { get; set; }
        [Name("Elemental_Attack")]
        public int Elemental_Atk { get; set; }
        [Name("Physical_Defense")]
        public int Physical_Def { get; set; }
        [Name("Elemental_Defense")]
        public int Elemental_Def { get; set; }
        [Name("Accuracy")]
        public int Accuracy { get; set; }
        [Name("Speed")]
        public int Speed { get; set; }
        [Name("Critical")]
        public int Critical { get; set; }
        [Name("Evasion")]
        public int Evasion { get; set; }
        [Name("Effect")]
        public string Effect { get; set; } = string.Empty;
        [Name("Buy_Price_Leaves")]
        public int Buy_Price { get; set; }
        [Name("Sell_Price_Leaves")]
        public int Sell_Price { get; set; }
        [Name("Equipment_Type")]
        public string Equipment_Type { get; set; } = string.Empty;
    }
}
