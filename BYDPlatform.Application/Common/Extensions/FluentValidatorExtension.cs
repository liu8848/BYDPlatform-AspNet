using FluentValidation;
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

    /// <summary>
    ///     对实体列表进行校验，并输出校验成功列，校验失败列以及校验失败错误
    /// </summary>
    /// <param name="validator">校验器</param>
    /// <param name="entities">实体列表</param>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <returns>
    ///     Dictionary：
    ///     “success”：校验成功列
    ///     “error”：校验错误列
    ///     “failure”：校验错误信息列，与error对相应
    /// </returns>
    public static async Task<Dictionary<string, object>> ValidateList<T>(this IValidator<T> validator, List<T> entities)
    {
        var dic = new Dictionary<string, object>();
        List<T> insertList = [];
        List<T> errorList = [];
        List<List<ValidationFailure>> failureList = [];


        foreach (var entity in entities)
        {
            var validationResult = await validator.ValidateAsync(entity);
            if (validationResult.IsValid)
            {
                insertList.Add(entity);
            }
            else
            {
                IDictionary<string, string[]> stringsMap = validationResult.ToDictionary();
                errorList.Add(entity);
                failureList.Add(validationResult.Errors);
            }
        }

        dic.Add("success", insertList);
        dic.Add("error", errorList);
        dic.Add("failure", failureList);
        return dic;
    }
}