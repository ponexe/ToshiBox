using System.Numerics;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using ECommons;
using ECommons.Configuration;
using ECommons.DalamudServices;
using KamiToolKit;
using ToshiBox.Common;
using ToshiBox.Features;
using ToshiBox.IPC;
using ToshiBox.UI;

namespace ToshiBox
{
    public sealed class ToshiBox : IDalamudPlugin
    {
        public string Name => "ToshiBox";

        public ToshiBox(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>();

            ECommonsMain.Init(pluginInterface, this);
            System.EventInstance = new Events();
            System.Config = EzConfig.Init<Config>();

            System.AutoRetainerListingInstance = new AutoRetainerListing();
            System.AutoRetainerListingInstance.IsEnabled();

            System.AutoChestOpenInstance = new AutoChestOpen();
            System.AutoChestOpenInstance.IsEnabled();

            PandoraIPC.Init();

            System.NativeController = new NativeController(pluginInterface);
            System.NativeConfigWindow = new NativeConfigWindow
            {
                InternalName = "ToshiBox_Config",
                Title = "ToshiBox Config",
                Size = new Vector2(450.0f, 450.0f),
                Position = new Vector2(300.0f, 300.0f),
                NativeController = System.NativeController,
                RememberClosePosition = true
            };

            // System.MainWindow = new MainWindow();

            // Service.PluginInterface.UiBuilder.Draw += Service.WindowSystem.Draw;
            Service.PluginInterface.UiBuilder.OpenConfigUi += () => System.MainWindow.IsOpen = true;
            Service.PluginInterface.UiBuilder.OpenMainUi += () => System.MainWindow.IsOpen = true;

            Service.CommandManager.AddHandler("/toshibox", new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens main settings window"
            });
            Service.CommandManager.AddHandler("/toshi", new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens main settings window"
            });

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
            switch (command)
            {
                case "/toshibox":
                case "/toshi":
                    if (args.EqualsIgnoreCase("toggleshangriladida009"))
                    {
                        System.Config.AutoRetainerListingConfig.Enabled =
                            !System.Config.AutoRetainerListingConfig.Enabled;
                        System.AutoRetainerListingInstance.IsEnabled();
                        EzConfig.Save();
                    }
                    else if (args.EqualsIgnoreCase("colors"))
                    {
                        var ssb = new SeStringBuilder();
                        for (ushort i = 0; i <= 50; i++)
                        {
                            ssb.AddUiForeground($"Color ID {i} ", i);
                            ssb.AddText("\n");
                        }

                        Svc.Chat.Print(ssb.BuiltString);
                    }
                    else
                    {
                        OpenConfigWindow();
                    }

                    break;
            }
        }

        private void OpenConfigWindow() => System.NativeConfigWindow.Open();

        public void Dispose()
        {
            System.AutoRetainerListingInstance.Disable();
            // System.MainWindow.Dispose(); //TODO: Remove ImGUI
            System.NativeConfigWindow.Dispose();
            PandoraIPC.Dispose();

            Svc.Commands.RemoveHandler("/toshibox");
            Svc.Commands.RemoveHandler("/toshi");

            ECommonsMain.Dispose(); // LAST
        }
    }
}