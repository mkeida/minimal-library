namespace Demo
{
    partial class Test
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Minimal.Components.Core.MComponent mComponent1 = new Minimal.Components.Core.MComponent();
            Minimal.Themes.Theme theme1 = new Minimal.Themes.Theme();
            Minimal.Themes.ThemeColor themeColor1 = new Minimal.Themes.ThemeColor();
            Minimal.Themes.ThemeColor themeColor2 = new Minimal.Themes.ThemeColor();
            Minimal.Themes.ThemeColor themeColor3 = new Minimal.Themes.ThemeColor();
            Minimal.Themes.ThemeColor themeColor4 = new Minimal.Themes.ThemeColor();
            Minimal.Themes.ThemeColor themeColor5 = new Minimal.Themes.ThemeColor();
            Minimal.Themes.ThemeColor themeColor6 = new Minimal.Themes.ThemeColor();
            this.mButton1 = new Minimal.Components.MButton();
            this.SuspendLayout();
            // 
            // mButton1
            // 
            this.mButton1.Accent = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(136)))), ((int)(((byte)(229)))));
            this.mButton1.CapitalText = true;
            this.mButton1.ClickEffect = Minimal.ClickEffect.Ripple;
            mComponent1.Accent = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(136)))), ((int)(((byte)(229)))));
            mComponent1.CustomTheme = null;
            mComponent1.Outdated = true;
            mComponent1.ParentForm = this;
            theme1.COMPONENT_BACKGROUND = themeColor1;
            theme1.COMPONENT_BORDER = themeColor2;
            theme1.COMPONENT_FILL = themeColor3;
            theme1.COMPONENT_FOREGROUND = themeColor4;
            theme1.COMPONENT_HIGHLIGHT = themeColor5;
            theme1.DARK_BASED = false;
            theme1.FORM_BACKGROUND = themeColor6;
            mComponent1.SourceTheme = theme1;
            this.mButton1.Component = mComponent1;
            this.mButton1.CustomColor = false;
            this.mButton1.CustomTheme = null;
            this.mButton1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.mButton1.Location = new System.Drawing.Point(286, 157);
            this.mButton1.Name = "mButton1";
            this.mButton1.Size = new System.Drawing.Size(130, 36);
            this.mButton1.TabIndex = 0;
            this.mButton1.Text = "Toggle theme";
            this.mButton1.Type = Minimal.ButtonType.Raised;
            this.mButton1.Click += new System.EventHandler(this.mButton1_Click);
            // 
            // Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 353);
            this.Controls.Add(this.mButton1);
            this.Name = "Test";
            this.Text = "Test";
            this.ResumeLayout(false);

        }

        #endregion

        private Minimal.Components.MButton mButton1;
    }
}