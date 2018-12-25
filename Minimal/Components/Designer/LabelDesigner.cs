using System.Collections;
using System.Windows.Forms.Design;

namespace Minimal.Components.Designer
{
    /// <summary>
    /// Label designer class
    /// </summary>
    partial class LabelDesigner : ControlDesigner
    {
        /// <summary>
        /// Filter properties
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            // Removes unnecessary inherited Label class properties
            properties.Remove("BackColor");
            properties.Remove("FlatStyle");
            properties.Remove("ForeColor");
            properties.Remove("FlatAppearance");
            properties.Remove("Image");
            properties.Remove("ImageAlign");
            properties.Remove("ImageIndex");
            properties.Remove("ImageKey");
            properties.Remove("ImageList");
            properties.Remove("TextImageRelation");
            properties.Remove("UseVisualStyleBackColor");
            properties.Remove("RightToLeft");
            properties.Remove("UseCompatibleTextRendering");
            properties.Remove("UseMnemonic");

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
            events.Remove("ForeColorChanged");

            // Push new events
            base.PreFilterEvents(events);
        }
    }
}
