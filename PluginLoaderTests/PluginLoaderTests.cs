using Plugins;
using Xunit;
using PluginLoader;
namespace PluginLoaderTests;

public class PluginLoaderTests
{
    [Fact]
    public void PluginsAreSortedCorrectly()
    {
        var plugins = new List<Type>
        {
            typeof(Inheritor1),
            typeof(SecondPlugin),
            typeof(FirstPlugin),
            typeof(Inheritor2)
        };

        var result = Program.GetSortedPlugins(plugins);

        Assert.Equal(typeof(FirstPlugin), result[0]); 
        Assert.True(result.IndexOf(typeof(FirstPlugin)) < result.IndexOf(typeof(Inheritor1)));
        Assert.True(result.IndexOf(typeof(FirstPlugin)) < result.IndexOf(typeof(Inheritor2)));
    }
    [Fact]
    public void CyclicDependencyThrowsException()
    {
        var plugins = new List<Type>
        {
            typeof(PluginA),
            typeof(PluginB)
        };
        Assert.Throws<InvalidOperationException>(() =>
        {
            Program.GetSortedPlugins(plugins);
        });
    }
} 