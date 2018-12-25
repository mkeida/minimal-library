using System;

namespace Minimal.Internal
{
    /// <summary>
    /// Internal help class. Helps with animations.
    /// </summary>
    internal class Animation
    {
        /// <summary>
        /// Gradual slowdown
        /// </summary>
        public static int CosinusMotion(double t, int distance)
        {
            if (t < 1)
            {
                return (int)(((-Math.Cos(t * Math.PI) / 2 + 0.5) * distance) + t);
            }

            return distance;
        }
    }
}
