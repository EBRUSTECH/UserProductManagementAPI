using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserProduct.Core.Dtos;

namespace UserProduct.Api.Extensions
{
    public static class ModelStateExtension
    {
        public static IEnumerable<Error> GetErrors(this ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(e => e.Value!.ValidationState == ModelValidationState.Invalid)
                .SelectMany(e => e.Value!.Errors, (key, error) => new Error(key.Key, error.ErrorMessage));

            return errors;
        }
    }
}
