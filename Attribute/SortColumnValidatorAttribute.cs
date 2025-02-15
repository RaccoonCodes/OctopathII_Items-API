using System.ComponentModel.DataAnnotations;

namespace OctopathII_Items.Attribute
{
    //UNUSED! Originally meant to be used for RequestDTO but changed its type from being strongly to
    //Generic which its type is needed to be known at compiled time. So this operation is moved to
    //RequestDTO file

    public class SortColumnValidatorAttribute : ValidationAttribute
    {
        private readonly Type _entityType;
        public SortColumnValidatorAttribute(Type entityType) : base("Value must match an existing column.")
            => (_entityType) = (entityType);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (_entityType != null)
            {
                var strValue = value as string;
                //checks that it is not null or empty and that it ensures that EntityType matches at least one
                //property strValue
                var property = _entityType.GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(strValue, StringComparison.OrdinalIgnoreCase));

                if (property != null && (property.PropertyType == typeof(string) || property.PropertyType == typeof(int)))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
