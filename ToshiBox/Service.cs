using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace ToshiBox;

public class Service
{
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; }
    [PluginService] public static WindowSystem WindowSystem { get; set; }
}