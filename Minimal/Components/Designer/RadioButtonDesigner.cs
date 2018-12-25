using System.Collections;
using System.Windows.Forms.Design;

namespace Minimal.Components.Designer
{
    /// <summary>
    /// Radio-button designer class
    /// </summary>
    class RadioButtonDesigner : ControlDesigner
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
            properties.Remove("TextAlign");

            // Push new properties
            base.PreFilterProperties(properties);
        }

        /// <summary>
        /// Filter events
        /// </summary>
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
                return SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable;
            }
        }
    }
}
