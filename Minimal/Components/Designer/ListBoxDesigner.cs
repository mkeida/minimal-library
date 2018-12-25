using System.Collections;
using System.Windows.Forms.Design;

namespace Minimal.Components.Designer
{
    /// <summary>
    /// List-box designer class
    /// </summary>
    class ListBoxDesigner : ControlDesigner
    {
        /// <summary>
        /// Filter properties
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            // Removes unnecessary inherited Button class properties
            properties.Remove("BackColor");
            properties.Remove("BackgroundImage");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("FlatAppearance");
            properties.Remove("FlatStyle");
            properties.Remove("Image");
            properties.Remove("ImageAlign");
            properties.Remove("ImageIndex");
            properties.Remove("ImageKey");
            properties.Remove("ImageList");
            properties.Remove("FlatStyle");
            properties.Remove("RightToLeft");
            properties.Remove("TextImageRelation");
            properties.Remove("UseVisualStyleBackColor");
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
            events.Remove("BackgroundImageChanged");
            events.Remove("BackgroundImageLayoutChanged");
            events.Remove("RightToLeftChanged");

            // Push new events
            base.PreFilterEvents(events);
        }
    }
}
