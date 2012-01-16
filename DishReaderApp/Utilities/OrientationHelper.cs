using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Utilities
{
    public sealed class OrientationHelper
    {
        public static void HideSystemTrayWhenInLandscapeMode(PageOrientation orientation)
        {
            SystemTray.Opacity = ((orientation & PageOrientation.Landscape) == PageOrientation.Landscape) ? 0 : 1;
        }
    }
}
