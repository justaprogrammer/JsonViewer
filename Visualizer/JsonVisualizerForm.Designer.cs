namespace EPocalipse.Json.Visualizer
{
    partial class JsonVisualizerForm
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
            this.Viewer = new EPocalipse.Json.Viewer.JsonViewer();
            this.SuspendLayout();
            // 
            // Viewer
            // 
            this.Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Viewer.Json = null;
            this.Viewer.Location = new System.Drawing.Point(0, 0);
            this.Viewer.Name = "Viewer";
            this.Viewer.Size = new System.Drawing.Size(875, 617);
            this.Viewer.TabIndex = 0;
            // 
            // JsonVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 617);
            this.Controls.Add(this.Viewer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JsonVisualizerForm";
            this.Text = "JsonVisualizerForm";
            this.ResumeLayout(false);

        }

        #endregion

        internal EPocalipse.Json.Viewer.JsonViewer Viewer;

    }
}