using System.Reflection;

namespace task05;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(method => method.Name);
    }
    public IEnumerable<string> GetMethodParams(string methodName)
    {
        var method = _type.GetMethod(methodName);
        if (method == null)
        {
            return Enumerable.Empty<string>();
        }
        return method
            .GetParameters()
            .Select(parameter => parameter.Name!)
            .Append(method.ReturnType.Name);
    }
    public IEnumerable<string> GetAllFields()
    {
        return _type
            .GetFields(BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic)
            .Select(field => field.Name);
    }
    public IEnumerable<string> GetProperties()
    {
        return _type
            .GetProperties()
            .Select(property => property.Name);
    }
    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.IsDefined(typeof(T), false);
    }
}