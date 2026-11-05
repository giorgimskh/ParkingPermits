namespace ParkingPermits
{
    partial class SplashForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tmrAnimation = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            //
            // tmrAnimation
            //
            tmrAnimation.Interval = 15;
            tmrAnimation.Tick += tmrAnimation_Tick;
            //
            // SplashForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 90, 168);
            ClientSize = new Size(520, 320);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "SplashForm";
            Opacity = 0D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Parking Permits";
            TopMost = true;
            Load += SplashForm_Load;
            Paint += SplashForm_Paint;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer tmrAnimation;
    }
}
