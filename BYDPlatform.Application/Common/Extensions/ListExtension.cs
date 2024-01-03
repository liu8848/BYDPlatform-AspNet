using BYDPlatform.Domain.DTOs.Base;

namespace BYDPlatform.Application.Common.Extensions;

public static class ListExtension
{
    public static List<T> ParseTo<T>(this List<IBaseCreateOrUpdateDto> list)
    {
        List<T> res = new();
        foreach (var dto in list) res.Add((T)dto);

        return res;
    }
}