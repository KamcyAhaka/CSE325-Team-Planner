using System.ComponentModel.DataAnnotations;

namespace TeamProjectPlanner.Services;

/// <summary>Runs the DataAnnotations rules declared on a model, so forms and services share one set of rules.</summary>
public static class EntityValidator
{
    /// <summary>Validates the entity and throws an <see cref="AppValidationException"/> carrying every failed rule.</summary>
    public static void EnsureValid(object entity)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(entity);

        if (Validator.TryValidateObject(entity, context, results, validateAllProperties: true))
        {
            return;
        }

        throw new AppValidationException(results
            .Select(result => result.ErrorMessage ?? "A value is invalid.")
            .ToList());
    }
}
