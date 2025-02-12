using CsvHelper.Configuration.Attributes;

namespace OctopathII_Items.Models.Csv
{
    public class ItemRecord
    {
        [Name("Name")]
        public string Name { get; set; } = string.Empty;
        
        [Name("Description")]
        public string Description { get; set; } = string.Empty;

        [Name("Buy_Price_Leaves")]
        public int Buy_Price { get; set; }

        [Name("Sell_Price_Leaves")]
        public int Sell_Price { get; set; }

        [Name("Item_Type")]
        public string Item_Type { get; set; } = string.Empty;
    }
}
