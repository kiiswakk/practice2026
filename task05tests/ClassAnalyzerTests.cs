namespace task05tests;
using task05;
using Xunit;


public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }

    public string MethodWithParams(string name, int age) 
    { 
        return name; 
    }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
        Assert.Contains("PublicField", fields);
    }

    [Fact]
    public void HasAttribute_ReturnsTrue()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        bool result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(result);
    }
    [Fact]
    public void HasAttribute_ReturnsFalse()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        bool result = analyzer.HasAttribute<SerializableAttribute>();

        Assert.False(result);
    }  
    [Fact]
    public void GetProperties_ReturnsProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }  

    [Fact]
    public void GetMethodParams_ReturnsReturnTypeAndParameters()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetMethodParams("MethodWithParams").ToList();
      
        Assert.Contains("String", methods);
        Assert.Contains("name", methods);
        Assert.Contains("age", methods);
    }
}
