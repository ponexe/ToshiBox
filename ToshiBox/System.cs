using ToshiBox.Common;
using ToshiBox.Features;
using ToshiBox.UI;

namespace ToshiBox;

public static class System
{
    public static Config Config { get; set; }
    public static MainWindow MainWindow { get; set; }
    public static Events EventInstance { get; set; }
    public static AutoRetainerListing AutoRetainerListingInstance { get; set; }
    public static AutoChestOpen AutoChestOpenInstance { get; set; }
}