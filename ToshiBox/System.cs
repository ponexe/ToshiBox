using KamiToolKit;
using ToshiBox.Common;
using ToshiBox.Features;
using ToshiBox.UI;

namespace ToshiBox;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public static class System
{
    public static Config Config { get; set; }
    public static MainWindow MainWindow { get; set; }
    public static NativeController NativeController { get; set; }
    public static NativeConfigWindow NativeConfigWindow { get; set; }
    public static Events EventInstance { get; set; }
    public static AutoRetainerListing AutoRetainerListingInstance { get; set; }
    public static AutoChestOpen AutoChestOpenInstance { get; set; }
}