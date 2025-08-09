using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using ECommons.Configuration;
using ECommons.Logging;

namespace ToshiBox.UI
{
    public class MainWindow : Window
    {
        private readonly WindowSystem _windowSystem;
        private enum SelectedFeature
        {
            None,
            AutoRetainerListing,
            AutoChestOpen,
        }

        private SelectedFeature _selectedFeature = SelectedFeature.None;

        public MainWindow() : base("ToshiBox Settings")
        {
            _windowSystem = new WindowSystem(Service.PluginInterface.InternalName);
            
            Service.PluginInterface.UiBuilder.Draw += _windowSystem.Draw;
        }

        #region Draw Method

        public override void Draw()
        {
            // Set the overall container to a fixed size big enough for both panels
            ImGui.BeginChild("ToshiBox_MainChild", new Vector2(600, 300), false);

            // Left panel fixed width
            float leftWidth = 250f;

            ImGui.BeginChild("LeftPanel", new Vector2(leftWidth, 0), true);
            ImGui.TextColored(ImGuiColors.DalamudWhite, "Features");
            ImGui.Separator();
            DrawFeatureList();
            ImGui.EndChild();

            ImGui.SameLine();

            // Right panel fills the rest of the available width manually calculated
            var availWidth = 600 - leftWidth - ImGui.GetStyle().ItemSpacing.X; // total width - left panel - spacing
            if (availWidth < 0) availWidth = 300; // fallback

            ImGui.BeginChild("RightPanel", new Vector2(availWidth, 0), true);
            ImGui.TextColored(ImGuiColors.DalamudWhite, "Settings");
            ImGui.Separator();
            DrawSettingsPanel();
            ImGui.EndChild();

            ImGui.EndChild();
        }

        #endregion

        #region Feature List

        private void DrawFeatureList()
        {
            float columnWidth = ImGui.GetColumnWidth();
            float checkboxWidth = ImGui.GetFrameHeight();
            float spacing = ImGui.GetStyle().ItemSpacing.X;
            float selectableWidth = columnWidth - checkboxWidth - spacing;
            var darkerBg = new Vector4(0.15f, 0.15f, 0.15f, 1.0f);

            void DrawFeature(string label, string checkboxId, ref bool enabled, SelectedFeature feature,
                Action onToggle)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, darkerBg);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 4));
                ImGui.BeginChild(label + "_Group", new Vector2(columnWidth, 40), true,
                    ImGuiWindowFlags.None);

                ImGui.Checkbox(checkboxId, ref enabled);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    onToggle();
                    EzConfig.Save();
                }

                ImGui.SameLine();

                bool selected = _selectedFeature == feature;
                if (ImGui.Selectable(label, selected, ImGuiSelectableFlags.None,
                        new Vector2(selectableWidth, 0)))
                {
                    _selectedFeature = feature;
                    PluginLog.Debug($"Selected feature changed to: {_selectedFeature}");
                }

                ImGui.EndChild();
                ImGui.PopStyleVar();
                ImGui.PopStyleColor();
                ImGui.Spacing();
            }

            // Always draw both features regardless of enabled state
            {
                bool enabled = System.Config.AutoRetainerListingConfig.Enabled;
                DrawFeature("Auto Retainer Listing", "##AutoRetainerEnabled", ref enabled,
                    SelectedFeature.AutoRetainerListing,
                    () =>
                    {
                        System.Config.AutoRetainerListingConfig.Enabled = enabled;
                        System.AutoRetainerListingInstance.IsEnabled();
                    });
            }

            {
                bool enabled = System.Config.AutoChestOpenConfig.Enabled;
                DrawFeature("Auto Chest Open", "##AutoChestOpenEnabled", ref enabled,
                    SelectedFeature.AutoChestOpen,
                    () =>
                    {
                        System.Config.AutoChestOpenConfig.Enabled = enabled;
                        System.AutoChestOpenInstance.IsEnabled();
                    });
            }
        }

        #endregion

        #region Settings Panel

        private void DrawSettingsPanel()
        {
            ImGui.Text($"Selected feature: {_selectedFeature}"); // Debug UI text

            switch (_selectedFeature)
            {
                case SelectedFeature.AutoRetainerListing:
                    DrawAutoRetainerListingSettings();
                    break;
                case SelectedFeature.AutoChestOpen:
                    DrawAutoChestOpenSettings();
                    break;
                default:
                    ImGui.TextColored(ImGuiColors.DalamudGrey, "Select a feature on the left to see its settings.");
                    break;
            }
        }

        #endregion

        #region AutoRetainerListing Settings

        private void DrawAutoRetainerListingSettings()
        {
            if (!System.Config.AutoRetainerListingConfig.Enabled)
            {
                ImGui.TextColored(ImGuiColors.DalamudGrey, "Enable the feature to adjust settings.");
                return;
            }

            ImGui.PushItemWidth(250f);

            int priceReduction = System.Config.AutoRetainerListingConfig.PriceReduction;
            if (ImGui.InputInt("Price Reduction", ref priceReduction))
            {
                System.Config.AutoRetainerListingConfig.PriceReduction = Math.Max(0, priceReduction);
                EzConfig.Save();
            }

            int lowestPrice = System.Config.AutoRetainerListingConfig.LowestAcceptablePrice;
            if (ImGui.InputInt("Lowest Acceptable Price", ref lowestPrice))
            {
                System.Config.AutoRetainerListingConfig.LowestAcceptablePrice = Math.Max(0, lowestPrice);
                EzConfig.Save();
            }

            int maxReduction = System.Config.AutoRetainerListingConfig.MaxPriceReduction;
            if (ImGui.InputInt("Max Price Reduction (0 = no limit)", ref maxReduction))
            {
                System.Config.AutoRetainerListingConfig.MaxPriceReduction = Math.Max(0, maxReduction);
                EzConfig.Save();
            }

            bool separateNQHQ = System.Config.AutoRetainerListingConfig.SeparateNQAndHQ;
            if (ImGui.Checkbox("Separate NQ and HQ", ref separateNQHQ))
            {
                System.Config.AutoRetainerListingConfig.SeparateNQAndHQ = separateNQHQ;
                EzConfig.Save();
            }

            ImGui.PopItemWidth();
        }

        #endregion

        #region AutoChestOpen Settings

        private void DrawAutoChestOpenSettings()
        {
            // Temporarily force enabled = true for testing so settings show:
            bool enabled = true; // _config.AutoChestOpenConfig.Enabled;
            if (!enabled)
            {
                ImGui.TextColored(ImGuiColors.DalamudGrey, "Enable the feature to adjust settings.");
                return;
            }

            ImGui.PushItemWidth(250f);

            float distance = System.Config.AutoChestOpenConfig.Distance;
            if (ImGui.SliderFloat("Distance (yalms)", ref distance, 0f, 3f, "%.1f"))
            {
                distance = (float)Math.Round(distance * 10f) / 10f;
                System.Config.AutoChestOpenConfig.Distance = distance;
                EzConfig.Save();
            }

            float delay = System.Config.AutoChestOpenConfig.Delay;
            if (ImGui.SliderFloat("Delay (seconds)", ref delay, 0f, 2f, "%.1f"))
            {
                delay = (float)Math.Round(delay * 10f) / 10f;
                System.Config.AutoChestOpenConfig.Delay = delay;
                EzConfig.Save();
            }

            bool openInHighEnd = System.Config.AutoChestOpenConfig.OpenInHighEndDuty;
            if (ImGui.Checkbox("Open Chests in High End Duties", ref openInHighEnd))
            {
                System.Config.AutoChestOpenConfig.OpenInHighEndDuty = openInHighEnd;
                EzConfig.Save();
            }

            bool closeLootWindow = System.Config.AutoChestOpenConfig.CloseLootWindow;
            if (ImGui.Checkbox("Close Loot Window After Opening", ref closeLootWindow))
            {
                System.Config.AutoChestOpenConfig.CloseLootWindow = closeLootWindow;
                EzConfig.Save();
            }

            ImGui.PopItemWidth();
        }

        #endregion
        
        #region MainWindow Dispose

        public void Dispose()
        {
            Service.PluginInterface.UiBuilder.Draw -= _windowSystem.Draw;
        }
        #endregion
    }
}