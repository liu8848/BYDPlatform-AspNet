using System.Dynamic;

namespace BYDPlatform.Application.Common.Interfaces;

public interface IDataShaper<T>
{
    IEnumerable<ExpandoObject> ShapeDate(IEnumerable<T> entities, string fieldString);
    ExpandoObject ShapeDate(T entity, string fieldString);
}