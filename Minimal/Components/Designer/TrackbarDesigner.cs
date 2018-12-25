using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Minimal.Components.Designer
{
    /// <summary>
    /// Track-bar designer class
    /// </summary>
    class TrackbarDesigner : ControlDesigner
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TrackbarDesigner()
        {
            AutoResizeHandles = true;
        }

        /// <summary>
        /// Filter properties
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            // Removes unnecessary inherited Button class properties
            properties.Remove("BackColor");
            properties.Remove("BackgroundImage");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("RightToLeft");

            // Push new properties
            base.PreFilterProperties(properties);
        }

        /// <summary>
        /// Filter events
        /// </summary>
        /// <param name="events"></param>
        protected override void PreFilterEvents(IDictionary events)
        {
            // Removes unnecessary inherited Button class events
            events.Remove("BackColorChanged");
            events.Remove("BackgroundImageChanged");
            events.Remove("BackgroundImageLayoutChanged");
            events.Remove("RightToLeftChanged");
            events.Remove("Scroll");

            // Push new events
            base.PreFilterEvents(events);
        }

        /// <summary>
        /// Overrides selection rules
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable;
            }
        }
    }
}
