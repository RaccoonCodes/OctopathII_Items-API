using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OctopathII_Items.Models
{
    public class EquipmentEntityTypeConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.HasKey(e => e.Name);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
            builder.HasIndex(e => e.Equipment_Type);
            builder.HasIndex(e => e.Sell_Price);
            builder.HasIndex(e => e.Physical_Atk);
            builder.HasIndex(e => e.Elemental_Atk);
        }
    }
}
