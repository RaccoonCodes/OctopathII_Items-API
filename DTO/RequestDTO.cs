using OctopathII_Items.Attribute;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OctopathII_Items.DTO
{
    public class RequestDTO<T>
    {
        [DefaultValue(0)] //for swagger
        public int PageIndex { get; set; } = 0;

        [DefaultValue(15)]
        [Range(1, 100)]
        public int PageSize { get; set; } = 15;

        [DefaultValue("Name")]
        public string? SortColumn { get; set; } = "Name";

        [DefaultValue("ASC")]
        [SortOrderValidator]
        public string SortOrder { get; set; } = "ASC";

        [DefaultValue(null)]
        public string? FilterQuery { get; set; } = null;

        // Manual validation method to check SortColumn against T's properties
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SortColumn != null)
            {
                var properties = typeof(T).GetProperties()
                    .Select(p => p.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!properties.Contains(SortColumn))
                {
                    yield return new ValidationResult(
                        $"SortColumn '{SortColumn}' is not a valid property of {typeof(T).Name}.",
                        new[] { nameof(SortColumn) });
                }
            }

        }
    }
}
