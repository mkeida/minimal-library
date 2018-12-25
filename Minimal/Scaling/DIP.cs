using System;
using System.Drawing;

namespace Minimal.Scaling
{
    /// <summary>
    /// Device Independent Pixel class helps to achieve better viewing experience
    /// across all possible Windows DPI types. 
    /// </summary>
    public class DIP
    {
        /// <summary>
        /// Set DIP
        /// </summary>
        public static int Set(int i)
        {
            // For every 24 DPI there is 0.25 pixel increase. Thats 0.01 pixel for 1 DPI. 
            return Convert.ToInt32(i * GetScalingFactor() * 0.0104166666666667 * 0.8);
        }

        /// <summary>
        /// Gets scaling factor without control's Graphics object reference
        /// </summary>
        /// <returns></returns>
        private static float GetScalingFactor()
        {
            // Gets booth DPI dimensions
            float dpiX, dpiY;

            // Create new graphics object
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            // Return DPI-x
            return dpiX;
        }
    }
}
