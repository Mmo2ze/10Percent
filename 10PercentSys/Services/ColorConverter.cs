using System.Globalization;

namespace _10PercentSys.Services;

public static class ColorConverter
{
    public static (int R, int G, int B) HexToRgb(string hex)
    {
        // Remove '#' if present
        hex = hex.TrimStart('#');

        // Parse the hex values
        if (hex.Length == 6) // Standard HEX format (RRGGBB)
        {
            int r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return (r, g, b);
        }

        if (hex.Length == 3) // Short HEX format (RGB -> RRGGBB)
        {
            int r = int.Parse(new string(hex[0], 2), NumberStyles.HexNumber);
            int g = int.Parse(new string(hex[1], 2), NumberStyles.HexNumber);
            int b = int.Parse(new string(hex[2], 2), NumberStyles.HexNumber);
            return (r, g, b);
        }

        return (255, 255, 255);
    }
    public static bool IsHex(string hex)
    {
        // Remove '#' if present
        hex = hex.TrimStart('#');

        // Parse the hex values
        if (hex.Length == 6) // Standard HEX format (RRGGBB)
        {
            int r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return true;
        }

        if (hex.Length == 3) // Short HEX format (RGB -> RRGGBB)
        {
            int r = int.Parse(new string(hex[0], 2), NumberStyles.HexNumber);
            int g = int.Parse(new string(hex[1], 2), NumberStyles.HexNumber);
            int b = int.Parse(new string(hex[2], 2), NumberStyles.HexNumber);
            return true;
        }

        return true;
    }
}