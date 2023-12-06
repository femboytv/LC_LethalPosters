using System.IO;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalPosters;

internal class ConfigBinder
{
    private static Plugin _plugin;
    private static ManualLogSource _logger;
    
    public static void Init(Plugin plugin, ManualLogSource logger)
    {
        _plugin = plugin;
        _logger = logger;
        _plugin.PosterFolders.Do(BindExternalPluginConfigEntry);
    }

    static void BindExternalPluginConfigEntry(string pluginPosterFolder)
    {
        var pluginName = pluginPosterFolder.Split(Path.DirectorySeparatorChar)[^2];
        
        var pluginConfigEntry = _plugin.Config.Bind(pluginName, "Enabled", true, $"Enable or disable {pluginName}");
        var movePluginPosterFolderTo = $"{pluginPosterFolder}{(pluginConfigEntry.Value ? "" : ".Disabled")}";
        if (pluginPosterFolder.Equals(movePluginPosterFolderTo)) return;

        try
        {
            Directory.Move(pluginPosterFolder, $"{pluginPosterFolder}{(pluginConfigEntry.Value ? "" : ".Disabled")}");
        }
        catch (IOException)
        {
            _logger.LogWarning($"Couldn't rename {pluginName}'s posters folder due to an error.");
        }
    }
}