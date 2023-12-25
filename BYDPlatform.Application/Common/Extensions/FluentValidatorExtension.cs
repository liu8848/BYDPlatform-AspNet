using FluentValidation.Results;

namespace BYDPlatform.Application.Common.Extensions;

public static class FluentValidatorExtension
{
    public static string? GetValidationErrorMessage(this IEnumerable<ValidationFailure> failures)
    {
        var failureDict = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        return string.Join(";", failureDict.Select(kv => kv.Key + ": " + string.Join(' ', kv.Value.ToArray())));
    }
}