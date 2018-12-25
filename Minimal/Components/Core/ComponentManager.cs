using System;
using System.Windows.Forms;

namespace Minimal.Components.Core
{
    /// <summary>
    /// Manages components
    /// </summary>
    public class ComponentManager
    {
        /// <summary>
        /// Registers new M component
        /// </summary>
        public static void RegisterComponent(IMComponent imComponent)
        {
            // Is IMComponents component object initialized?
            if (imComponent.Component == null)
                // Component not initialized - create new instance of MComponent
                imComponent.Component = new MComponent();

            // Add component to component collection, if it is not there yet
            if (!M.IMComponents.Contains(imComponent))
                M.IMComponents.Add(imComponent);

            // Is component also a Control?
            if (imComponent is Control)
            {
                // Cast IMComponent to control
                Control control = imComponent as Control;

                // Hooks up important control events
                // Control's handle-created event
                control.HandleCreated += (object handleCreatedSender, EventArgs handleCreatedArgs) =>
                {
                    // Set parent form
                    imComponent.Component.ParentForm = control.FindForm();

                    // Handles control's source theme
                    // Check if control has set own theme
                    if (imComponent.Component.CustomTheme != null)
                    {
                        // Set custom theme as source theme
                        imComponent.Component.SourceTheme = imComponent.Component.CustomTheme;
                    }
                    else
                    {
                        // Control don't have its own theme
                        // Is controls parent form M-Form?
                        if (control.FindForm() is MForm)
                        {
                            // Cast control's parent form to MForm
                            MForm f = (MForm)control.FindForm();

                            // Set parent's M-Form theme as source theme
                            imComponent.Component.SourceTheme = f.Theme;
                        }
                        else
                        {
                            // Control's parent form is not MForm type
                            // Set source theme as application wide theme
                            imComponent.Component.SourceTheme = M.ApplicationWideTheme;
                        }
                    }

                    // Component theme-changed event
                    // Must be hooked in handle created!
                    imComponent.Component.ThemeChanged += (object themeChangedSender, ThemeChangedEventArgs themeChangedArgs) =>
                    {
                        // Handles control's source theme
                        // Check if control has set own theme
                        if (imComponent.Component.CustomTheme != null)
                        {
                            // Set custom theme as source theme
                            imComponent.Component.SourceTheme = imComponent.Component.CustomTheme;
                        }
                        else
                        {
                            // Control don't have its own theme
                            // Is controls parent form M-Form?
                            if (control.FindForm() is MForm)
                            {
                                // Cast control's parent form to MForm
                                MForm f = (MForm)control.FindForm();

                                // Set parent's M-Form theme as source theme
                                imComponent.Component.SourceTheme = f.Theme;
                            }
                            else
                            {
                                // Control's parent form is not MForm type
                                // Set source theme as application wide theme
                                imComponent.Component.SourceTheme = M.ApplicationWideTheme;
                            }
                        }

                        // Redraw control
                        control.Invalidate();

                        // Redraw component
                        imComponent.Component.Outdated = true;
                    };
                };

                // Control's handle-destroyed event
                control.HandleDestroyed += (object sender, EventArgs e) =>
                {
                    // Remove component from component collection
                    M.IMComponents.Remove(imComponent);
                };

                // Control's parent changed
                control.ParentChanged += (object sender, EventArgs e) =>
                {
                    // Check if custom theme is not set
                    if (imComponent.Component.CustomTheme == null)
                    {
                        // Control don't have its own theme
                        // Is controls parent form M-Form?
                        if (control.FindForm() is MForm)
                        {
                            // Cast control's parent form to MForm
                            MForm f = (MForm)control.FindForm();

                            // Set parent's M-Form theme as source theme
                            imComponent.Component.SourceTheme = f.Theme;
                        }
                        else
                        {
                            // Control's parent form is not MForm type
                            // Set source theme as application wide theme
                            imComponent.Component.SourceTheme = M.ApplicationWideTheme;
                        }
                    }

                    // Redraw control
                    control.Invalidate();

                    // Redraw component
                    imComponent.Component.Outdated = true;

                    // Change parent form
                    imComponent.Component.ParentForm = control.FindForm();
                };
            }
        }
    }
}
