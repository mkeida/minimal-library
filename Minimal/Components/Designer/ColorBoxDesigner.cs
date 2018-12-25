using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace Minimal.Components.Designer
{
    /// <summary>
    /// Color-box designer class
    /// </summary>
    class ColorBoxDesigner : ControlDesigner
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ColorBoxDesigner()
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
            properties.Remove("BorderStyle");
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
                return SelectionRules.Moveable;
            }
        }
    }
}
