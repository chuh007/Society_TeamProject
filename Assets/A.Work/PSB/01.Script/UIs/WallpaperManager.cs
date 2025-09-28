using System.Runtime.InteropServices;
using UnityEngine;

namespace Code.Scripts.UI
{
    public class WallpaperManager : MonoBehaviour
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, System.Text.StringBuilder lpvParam, int fuWinIni);

        private const int SPI_GETDESKWALLPAPER = 0x0073;

        public static string GetWallpaperPath()
        {
            System.Text.StringBuilder path = new System.Text.StringBuilder(200);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, path.Capacity, path, 0);
            return path.ToString();
        }

    }
}
