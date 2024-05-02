namespace Evently.Api.Extensions;

internal static class ConfigurationExtensions
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder configuration, string[] modules)
    {
        foreach (var module in modules)
        {
            configuration.AddJsonFile($"modules.{module}.json", false, true);
            configuration.AddJsonFile($"modules.{module}.Development.json", true, true);
        }
    }
}
