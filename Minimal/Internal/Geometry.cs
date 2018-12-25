using System;
using System.Drawing;

namespace Minimal.Internal
{
    /// <summary>
    /// Internal help class. Simplifies work with geometry shapes.
    /// </summary>
    internal class Geometry
    {
        /// <summary>
        /// Returns diagonal length of rectangle
        /// </summary>
        public static double GetRectangleDiagonal(Rectangle r)
        {
            return 2 * Math.Sqrt(Math.Pow(r.Width, 2) + Math.Pow(r.Height, 2));
        }

        /// <summary>
        /// Returns distance between two points
        /// </summary>
        public static double GetDistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
