using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ToshiBox.Common;
using ToshiBox.Features;
using ToshiBox.IPC;
using ToshiBox.UI;

namespace ToshiBox
{
    public sealed class ToshiBox : IDalamudPlugin
    {
        private readonly WindowSystem _windowSystem = new("ToshiBox");
        public AutoChestOpen AutoChestOpenInstance;
        public string Name => "ToshiBox";

        public ToshiBox(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();

            System.EventInstance = new Events();
            System.Config = EzConfig.Init<Config>();
            // Initialize ECommons and config
            ECommonsMain.Init(pluginInterface, this);

            System.AutoRetainerListingInstance = new AutoRetainerListing();
            System.AutoRetainerListingInstance.IsEnabled();

            AutoChestOpenInstance = new AutoChestOpen();
            AutoChestOpenInstance.IsEnabled();

            PandoraIPC.Init();

            System.MainWindow = new MainWindow();

            Service.PluginInterface.UiBuilder.Draw += _windowSystem.Draw;
            Service.PluginInterface.UiBuilder.OpenConfigUi += () => System.MainWindow.IsOpen = true;
            Service.PluginInterface.UiBuilder.OpenMainUi += () => System.MainWindow.IsOpen = true;

            Svc.Commands.AddHandler("/toshibox", new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens main settings window"
            });
            Svc.Commands.AddHandler("/toshi", new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens main settings window"
            });
        }

        public void OnCommand(string command, string args)
        {
            if (string.Equals(args, "toggleshangriladida009"))
            {
                System.Config.AutoRetainerListingConfig.Enabled = !System.Config.AutoRetainerListingConfig.Enabled;
                System.AutoRetainerListingInstance.IsEnabled();
                EzConfig.Save();
            }

            if (string.Equals(args, "colors"))
            {
                var ssb = new SeStringBuilder();
                for (ushort i = 0; i <= 50; i++) {
                    ssb.AddUiForeground($"Color ID {i} ", i);
                    ssb.AddText("\n");
                }
                Svc.Chat.Print(ssb.BuiltString);
            }
            else
            {
                System.MainWindow.IsOpen = !System.MainWindow.IsOpen;
            }
        }

        public void Dispose()
        {
            System.AutoRetainerListingInstance.Disable();
            System.MainWindow.Dispose();
            PandoraIPC.Dispose();

            Svc.Commands.RemoveHandler("/toshibox");
            Svc.Commands.RemoveHandler("/toshi");

            ECommonsMain.Dispose(); // LAST
        }
    }
}
