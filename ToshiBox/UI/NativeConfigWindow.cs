using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Slider;
using KamiToolKit.Nodes.TabBar;
using KamiToolKit.System;

namespace ToshiBox.UI;

public class NativeConfigWindow : NativeAddon
{
    private ScrollingAreaNode<TreeListNode>? scrollingAreaNode;
    private TreeListCategoryNode? autoRetainerCategory;
    private TreeListCategoryNode? autoChestCategory;

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        AttachNode(scrollingAreaNode = new ScrollingAreaNode<TreeListNode> {
			
            // Size and Position is the area that you want to be visible
            Position = ContentStartPosition,
            Size = ContentSize,
			
            // Content Height is how tall you want the entire scrolling area to be 
            ContentHeight = 2000.0f,
			
            // Sets how much the node should move for each tick of scroll (default 24)
            ScrollSpeed = 25,
			
            IsVisible = true,
        });
        
        var treeListNode = scrollingAreaNode.ContentAreaNode;
        
        treeListNode.AddCategoryNode(autoRetainerCategory = new TreeListCategoryNode {
            IsVisible = true,
            IsCollapsed = true,
            Label = "Auto Retainer Listing",
        });
        autoRetainerCategory.AddHeader("If you know, you know");
        
        var container = new VerticalListNode {
            Size = new Vector2(autoRetainerCategory.Width, 0.0f),
            IsVisible = true,
            FitContents = true,
        };
        
        var tabContainer = new ResNode {
            Size = new Vector2(autoRetainerCategory.Width, 28.0f), IsVisible = true,
        };

        var textNode1 = new TextNode {
            TextFlags = TextFlags.AutoAdjustNodeSize, Text = "First Tab Element", IsVisible = true,
        };
		
        var textNode2 = new TextNode {
            TextFlags = TextFlags.AutoAdjustNodeSize, Text = "Second Tab Element", IsVisible = false, 
        };
		
        var textNode3 = new TextNode {
            TextFlags = TextFlags.AutoAdjustNodeSize, Text = "Third Tab Element", IsVisible = false, 
        };
        
        // The tab bar itself is a very simple node, it only provides the ability to .AddTab with a label
        // and to set up onclick events when a tab is clicked
        var tabBar = new TabBarNode {
            Size = new Vector2(autoRetainerCategory.Width, 24.0f), 
            IsVisible = true,
        };

        tabBar.AddTab("First", () => {
            textNode1.IsVisible = true;
            textNode2.IsVisible = false;
            textNode3.IsVisible = false;
        });
		
        tabBar.AddTab("Second", () => {
            textNode1.IsVisible = false;
            textNode2.IsVisible = true;
            textNode3.IsVisible = false;
        });
		
        tabBar.AddTab("Third", () => {
            textNode1.IsVisible = false;
            textNode2.IsVisible = false;
            textNode3.IsVisible = true;
        });
		
        NativeController.AttachNode(textNode1, tabContainer);
        NativeController.AttachNode(textNode2, tabContainer);
        NativeController.AttachNode(textNode3, tabContainer);
		
        container.AddNode(tabBar);
        container.AddNode(tabContainer);
        autoRetainerCategory.AddNode(container);
        
        treeListNode.AddCategoryNode(autoChestCategory = new TreeListCategoryNode {
            IsVisible = true,
            IsCollapsed = true,
            Label = "Auto Chest",
        });
        autoChestCategory.AddHeader("Automatically open chests");
        
        var openDistanceContainer = GetContainer(autoChestCategory);
        var openDelayContainer = GetContainer(autoChestCategory);
		
        // Sliders let the user choose values between a set range
        var openDistanceSlider = new SliderNode {
            Size = new Vector2(200.0f, 32.0f),
            IsVisible = true,
            Min = 0,
            Max = 100,
            

            // Event that is called when the value changes
            // OnValueChanged = newValue => textNode.Text = $"Value: {newValue}"
        };
		
        openDistanceContainer.AddNode(openDistanceSlider);
        openDistanceContainer.AddNode(new TextNode {Text = "Distance (deciyalms)", TextFlags = TextFlags.AutoAdjustNodeSize,AlignmentType = AlignmentType.Left, Height = 32.0f});

        var delayContainer = GetContainer(autoChestCategory);
		
        // Sliders let the user choose values between a set range
        var openDelaySlider = new SliderNode {
            Size = new Vector2(200.0f, 32.0f),
            IsVisible = true,
            Min = 0,
            Max = 10,

            // Event that is called when the value changes
            // OnValueChanged = newValue => textNode.Text = $"Value: {newValue}"
        };
		
        openDelayContainer.AddNode(openDelaySlider);
        openDelayContainer.AddNode(new TextNode {Text = "Delay (second)", TextFlags = TextFlags.AutoAdjustNodeSize,AlignmentType = AlignmentType.Left, Height = 32.0f});
        
        autoChestCategory.AddNode(openDistanceContainer);
        autoChestCategory.AddNode(openDelayContainer);
    }
    
    protected override unsafe void OnHide(AtkUnitBase* addon) {
    }
    
    private static HorizontalFlexNode GetContainer(TreeListCategoryNode treeListCategoryNode) => new() {
        Width = treeListCategoryNode.Width,
        AlignmentFlags = FlexFlags.FitContentHeight | FlexFlags.CenterVertically,
        IsVisible = true,
        X = 14,
    };
    
    private static TextNode GetTextNode() => new() {
        TextFlags = TextFlags.AutoAdjustNodeSize,
        AlignmentType = AlignmentType.Left,
        Text = "No option selected",
        Height = 32.0f,
    };
}