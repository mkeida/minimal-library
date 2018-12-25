using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.Components.Items
{
    /// <summary>
    /// Represents single MChart record
    /// </summary>
    public struct ChartValue
    {
        /// <summary>
        /// X position
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y position
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ChartValue(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
